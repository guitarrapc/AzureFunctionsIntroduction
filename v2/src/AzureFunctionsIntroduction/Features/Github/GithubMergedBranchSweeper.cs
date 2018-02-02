using Microsoft.Azure.WebJobs.Host;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AzureFunctionsIntroduction.Features.Github
{
    public class GithubMergedBranchSweeper
    {
        public string Owner { get; private set; }
        public bool DryRun { get; set; } = true;
        public int DaysPast { get; set; } = 1;
        public string Repository { get; private set; } = null;
        public string[] ExcludeBranches { get; set; } = null;
        public int PrPageSize { get; set; } = 100;
        public int? PrPageCount { get; set; } = 20;

        private string _token;
        private OctokitClient client;
        private string page = "https://github.com/{0}/{1}/branches/all?utf8=✓&query={2}";

        public static readonly string[] DefaultExcludeBranchRule = new[]
        {
            "^master$",
            "^development$",
            "^nightly/.+$",
            "^stable/.+$",
            "^staging/.+$",
            "^review/.+$",
        };

        public GithubMergedBranchSweeper(string owner, string token, string repository)
        {
            Owner = owner;
            _token = token;
            Repository = repository;

            client = new OctokitClient(_token, Owner, nameof(GithubMergedBranchSweeper));
        }

        /// <summary>
        /// Sweep Merges PullRequests
        /// </summary>
        /// <param name="log"></param>
        /// <param name="dryRunAction"></param>
        /// <returns></returns>
        public async Task<SweeptargetBranchResult[]> SweepAsync(TraceWriter log, Action<SweeptargetBranchResult[]> dryRunAction)
        {
            log.Info($"Starting sweep repository's branch. repositry : {Repository}");

            // guard invalid values
            if (string.IsNullOrEmpty(Owner)) throw new ArgumentNullException($"No target respositories set.");
            if (string.IsNullOrEmpty(_token)) throw new ArgumentNullException($"No target respositories set.");
            if (string.IsNullOrEmpty(Repository)) throw new ArgumentNullException($"Missing repository to be set.");
            if (PrPageSize == 0) throw new ArgumentOutOfRangeException($"You can't set PullRequest Page size to be 0");
            if (!PrPageCount.HasValue || PrPageCount.Value == 0) throw new ArgumentOutOfRangeException($"You can't set PullRequest Page count to be 0");

            var mergedNotDeletedBranches = await GetMergedNotDeletedBranchesAsync(Repository);
            if (!mergedNotDeletedBranches.Any())
            {
                log.Info($"0 sweep candidate branches found on repository : {Repository}");
                return null;
            }

            var candidateBranches = await GetDeleteCandidateBranchesAsync(Repository, mergedNotDeletedBranches);

            // Stop execute on Dryrun
            if (DryRun)
            {
                log.Info($"{nameof(DryRun)} option is enabled. Sweep not executed.");
                dryRunAction(candidateBranches);
                return candidateBranches;
            }

            // Delete Branches
            foreach (var item in candidateBranches)
            {
                // ブランチ削除
                log.Info($"Removing branch. {nameof(item.PullrequestId)}: {item.PullrequestId}, {nameof(item.BranchName)}: {item.BranchName}, {nameof(item.CreatedBy)}: {item.CreatedBy}, {nameof(item.MergedBy)}: {item.MergedBy}, {nameof(item.MergedAt)}: {item.MergedAt}, {nameof(item.Locked)}, {item.Locked}");
                await client.GitHubClient.Git.Reference.Delete(Owner, Repository, item.BranchDeleteRef);
            }
            return candidateBranches;
        }

        private async Task<SweeptargetBranchResult[]> GetMergedNotDeletedBranchesAsync(string repository)
        {
            var branchOptions = new ApiOptions() { PageSize = 100 };
            var branchClient = client.GitHubClient.Repository.Branch;
            var allBranches = await branchClient.GetAll(Owner, repository, branchOptions);
            var branches = allBranches;
            if (ExcludeBranches != null && ExcludeBranches.Any())
            {
                branches = allBranches.Where(x => ExcludeBranches.Any(y => Regex.IsMatch(x.Name, y, RegexOptions.IgnoreCase))).ToArray();
            }

            // HEAVY TASK!!!
            // PR history length relates to task heaviness. (2000 histories = 30sec or more)
            var prRequest = new PullRequestRequest() { State = ItemStateFilter.Closed, SortProperty = PullRequestSort.Updated };
            var prOptions = new ApiOptions() { PageSize = PrPageSize, PageCount = PrPageCount };
            var prClient = client.GitHubClient.PullRequest;
            var prs = await prClient.GetAllForRepository(Owner, repository, prRequest, prOptions);

            // no pull requests
            if (prs == null || !prs.Any()) return new SweeptargetBranchResult[] { };

            var mergedNotDeletedBranches = prs.Where(x => x.Merged).Select(x => new SweeptargetBranchResult
            {
                CreatedBy = x.User.Login,
                RepositoryName = x.Head?.Repository?.Name,
                RepositoryId = x.Head?.Repository?.Id,
                PullrequestId = x.Id,
                BranchSha = x.Head?.Sha,
                BranchName = x.Head?.Ref,
                BranchDeleteRef = $"heads/{x.Head?.Ref}",
                Title = x.Title,
                Merged = x.Merged,
                Locked = x.Locked,
                MergedBy = x.MergedBy?.Login,
                CreatedAt = x.CreatedAt,
                MergedAt = x.MergedAt,
                UpdatedAt = x.UpdatedAt,
                ClosedAt = x.ClosedAt,
            })
            .Where(x => branches.Any(y => y.Name == x.BranchName))
            .Where(x => DateTime.Now.AddDays(-1 * DaysPast) > x.MergedAt)
            .ToArray();

            return mergedNotDeletedBranches;
        }

        private async Task<SweeptargetBranchResult[]> GetDeleteCandidateBranchesAsync(string repository, SweeptargetBranchResult[] branches)
        {
            var refClient = client.GitHubClient.Git.Reference;

            // HEAVY TASK!!!
            // References length relates to task heaviness.
            var refs = await refClient.GetAll(Owner, repository);

            // Matching Branch and References
            var candidateBranches = branches.Select(pr => new
            {
                PR = pr,
                // filter branch ref by name.
                Ref = refs.Where(x => x.Object.Sha == pr.BranchSha)
                    .Where(x => x.Ref == $"refs/{pr.BranchDeleteRef}")
                    .FirstOrDefault()
            })
            .Where(x => x.Ref != null)
            .Select(x => x.PR)
            .ToArray();

            return candidateBranches;
        }

        /// <summary>
        /// Get list of Branch detail informations
        /// </summary>
        /// <param name="branches"></param>
        /// <param name="excludeBranches"></param>
        /// <returns></returns>
        public async Task<BranchDetailResult[]> GetBranchDetailsAsync(IReadOnlyList<Branch> branches)
        {
            var commits = await GetCommitsAsync(branches.Select(x => x.Commit.Sha).ToArray());
            var result = commits.Select(x => new { x.Sha, LastDate = x.Commit.Author.Date, Commiter = x.Committer?.Login })
            .Join(branches, x => x.Sha, x => x.Commit.Sha, (inner, outer) => new BranchDetailResult
            {
                Name = outer.Name,
                Protected = outer.Protected,
                Commiter = inner.Commiter,
                Url = Uri.EscapeUriString(string.Format(page, Owner, Repository, outer.Name)),
                LastDate = inner.LastDate,
                Sha = inner.Sha
            })
            .Distinct()
            .OrderByDescending(x => x.LastDate)
            .ToArray();

            return result;
        }

        /// <summary>
        /// Get Branches
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<Branch>> GetBranchesAsync(string[] excludeBranches)
        {
            var option = new ApiOptions() { PageSize = 100 };
            var branch = client.GitHubClient.Repository.Branch;
            var all = await branch.GetAll(Owner, Repository, option);
            var result = all;
            if (ExcludeBranches != null && ExcludeBranches.Any())
            {
                result = all.Where(x => excludeBranches.Any(y => Regex.IsMatch(x.Name, y, RegexOptions.IgnoreCase))).ToArray();
            }
            return result;
        }

        /// <summary>
        /// Get Commits
        /// </summary>
        /// <param name="shas"></param>
        /// <returns></returns>
        public async Task<GitHubCommit[]> GetCommitsAsync(IEnumerable<string> shas)
        {
            var option = new ApiOptions() { PageSize = 100 };
            var commit = client.GitHubClient.Repository.Commit;
            var tasks = shas.Select(x => commit.Get(Owner, Repository, x));
            var result = await Task.WhenAll(tasks);
            return result;
        }

        public class OctokitClient
        {
            public GitHubClient GitHubClient { get; set; }
            public string Owner { get; set; }

            public OctokitClient(string accessToken, string owner, string name)
            {
                GitHubClient = new GitHubClient(new ProductHeaderValue(name));
                GitHubClient.Credentials = new Credentials(accessToken);
                Owner = owner;
            }
        }
    }

    public class SweeptargetBranchResult
    {
        public string CreatedBy { get; set; }
        public string RepositoryName { get; set; }
        public long? RepositoryId { get; set; }
        public long PullrequestId { get; set; }
        public string BranchSha { get; set; }
        public string BranchName { get; set; }
        public string BranchDeleteRef { get; set; }
        public string Title { get; set; }
        public bool Merged { get; set; }
        public string MergedBy { get; set; }
        public bool Locked { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? MergedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }
    }

    public class BranchDetailResult
    {
        public string Name { get; set; }
        public bool Protected { get; set; }
        public string Commiter { get; set; }
        public string Url { get; set; }
        public DateTimeOffset LastDate { get; set; }
        public string Sha { get; set; }
    }
}