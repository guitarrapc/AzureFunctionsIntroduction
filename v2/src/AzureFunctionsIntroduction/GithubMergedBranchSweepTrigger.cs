using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using AzureFunctionsIntroduction.Features.Github;
using Newtonsoft.Json;

namespace AzureFunctionsIntroduction
{
    public static class GithubMergedBranchSweepTrigger
    {
        private static readonly string tokenHeader = "X-Github-AccessToken";

        [FunctionName("GithubMergedBranchSweepTrigger")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"{nameof(GithubMergedBranchSweepTrigger)} : C# HTTP trigger function processed a request.");

            var token = req.Headers.Where(x => x.Key == tokenHeader)
                .SelectMany(x => x.Value)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(token))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, $"Missing AccessToken from Header : {tokenHeader}");
            }

            var json = await req.Content.ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<GithubMergedBranchSweepRequest>(json);

            // Validation
            if (request.DaysPast < 0)
                return req.CreateResponse(HttpStatusCode.BadRequest, $"{nameof(request.DaysPast)} element must be larger equal than 1.");
            if (request.PrPageCount < 0)
                return req.CreateResponse(HttpStatusCode.BadRequest, $"{nameof(request.PrPageCount)} element must be larger equal than 1.");
            if (request.PrPageSize < 0)
                return req.CreateResponse(HttpStatusCode.BadRequest, $"{nameof(request.PrPageSize)} element must be larger equal than 1.");

            try
            {
                // Execute
                var sweeper = new GithubMergedBranchSweeper(request.Owner, token, request.Repository)
                {
                    DaysPast = request.DaysPast,
                    DryRun = request.DryRun,
                    ExcludeBranches = request.ExcludeBranches,
                    PrPageCount = request.PrPageCount,
                    PrPageSize = request.PrPageSize,
                };
                Action<SweeptargetBranchResult[]> action = branches => log.Info($@"DryRun - {nameof(GithubMergedBranchSweepTimer)}: remove candidates are following.
{branches.Select(x => x.BranchName).ToJoinedString(Environment.NewLine)}");
                var results = await sweeper.SweepAsync(log, action);

                // Response
                var response = new SweepResponse() { DryRun = request.DryRun, Value = results };
                var responseJson = JsonConvert.SerializeObject(response);
                return req.CreateResponse(HttpStatusCode.OK, responseJson);
            }
            catch (Exception ex)
            {
                log.Error($"{ex.Message} {ex.GetType().FullName} {ex.StackTrace}");
                return req.CreateResponse(HttpStatusCode.InternalServerError, $"Error when running sweep. {ex.GetType()} {ex.Message}");
            }
        }

        public class GithubMergedBranchSweepRequest
        {
            public string Owner { get; set; }
            public bool DryRun { get; set; } = true;
            public int DaysPast { get; set; } = 1;
            public string Repository { get; set; }
            public string[] ExcludeBranches { get; set; }
            public int PrPageSize { get; set; } = 100;
            public int? PrPageCount { get; set; } = 20;
        }

        public class SweepResponse
        {
            public bool DryRun { get; set; }
            public SweeptargetBranchResult[] Value { get; set; }

            public string ToChatworkInfoString(string owner, string repository, int daysPast, bool usePRUrl)
            {
                var text = Value.GroupBy(x => x.CreatedBy)
                .Select(y =>
                {
                    var createdBy = y.First().CreatedBy;
                    var repositoryName = y.First().RepositoryName;
                    var mergedAtes = y.Select(x => x.MergedAt.Value.ToLocalTime().ToString("d")).ToArray();
                    var branchNames = Value.Select(x => x.BranchName).ToArray();
                    var mergedAt = new MarkdownFormat("MergedAt", Value.Select(x => x.MergedAt.Value.ToLocalTime().ToString("d")).ToArray());
                    var branchName = new MarkdownFormat("BranchName", Value.Select(x => x.BranchName).ToArray());
                    var prId = new MarkdownFormat("PrId", Value.Select(x => x.PullrequestId.ToString()).ToArray());
                    var values = mergedAt.Values.Zip(branchName.Values, (m, b) => new { Output = $"{m}: {b}", BranchName = b })
                    .Zip(prId.Values, (x, p) =>
                    {
                        var link = $"https://github.com/{owner}/{repository}";
                        if (usePRUrl && p != "0")
                        {
                            link += $"/pull/{p}";
                        }
                        else
                        {
                            link += $"/branches/all?utf8={Uri.EscapeUriString("✓")}&query={Uri.EscapeUriString(x.BranchName)}";
                            if (link.EndsWith("%20"))
                            {
                                link = link.Substring(0, link.Length - 3);
                            }
                        }
                        return $"{x.Output} ( {link} )";
                    });

                    var result = $@"[info][title]{repositoryName}: {createdBy} ({daysPast}日経過のブランチ数: {values.Count()}件)[/title]{values.Select(x => x).ToJoinedString(Environment.NewLine)}[/info]";
                    return result;
                })
                .ToArray();
                return text.ToJoinedString(Environment.NewLine);
            }

            public string ToMarkdownString()
            {
                var text = Value.GroupBy(x => x.CreatedBy)
                .Select(y =>
                {
                    var createdByes = Value.Select(x => x.CreatedBy).ToArray();
                    var mergedAtes = Value.Select(x => x.MergedAt.Value.ToLocalTime().ToString("d")).ToArray();
                    var branchNames = Value.Select(x => x.BranchName).ToArray();
                    var repositoryNames = Value.Select(x => x.RepositoryName).ToArray();

                    var created = new MarkdownFormat("CreatedBy", Value.Select(x => x.CreatedBy).ToArray());
                    var mergedAt = new MarkdownFormat("MergedAt", Value.Select(x => x.MergedAt.Value.ToLocalTime().ToString("d")).ToArray());
                    var branchName = new MarkdownFormat("BranchName", Value.Select(x => x.BranchName).ToArray());
                    var repositoryName = new MarkdownFormat("RepositoryName", Value.Select(x => x.RepositoryName).ToArray());
                    var values = created.Values
                        .Zip(mergedAt.Values, (c, m) => $"| {c} | {m}")
                        .Zip(branchName.Values, (x, b) => $"{x} | {b}")
                        .Zip(repositoryName.Values, (x, r) => $"{x} | {r} |");

                    var result = $@"| {created.Column} | {mergedAt.Column} | {branchName.Column} | {repositoryName.Column} |
| {created.TableSpacer} | {mergedAt.TableSpacer} | {branchName.TableSpacer} | {repositoryName.TableSpacer} |
{values.Select(x => x).ToJoinedString(Environment.NewLine)}";
                    return result;
                })
                .ToArray();
                return text.ToJoinedString(Environment.NewLine);
            }

            private struct MarkdownFormat
            {
                public string Column { get; set; }
                public string[] Values { get; set; }
                public string TableSpacer { get; set; }

                public MarkdownFormat(string column, string[] values)
                {
                    var colLength = column.Length;
                    var valueMaxLength = values.Select(x => x.Length).Max();
                    var max = valueMaxLength > colLength ? valueMaxLength : colLength;
                    TableSpacer = Enumerable.Range(1, max).Select(x => "-").ToJoinedString();

                    Values = values.Select(x =>
                    {
                        var valueSpacer = "";
                        if (max > x.Length)
                        {
                            valueSpacer = Enumerable.Range(1, max - x.Length).Select(_ => " ").ToJoinedString();
                        }
                        return x + valueSpacer;
                    })
                    .ToArray();

                    var colSpacer = "";
                    if (max > colLength)
                    {
                        colSpacer = Enumerable.Range(1, max - colLength).Select(x => " ").ToJoinedString();
                    }
                    Column = column + colSpacer;
                }
            }
        }
    }
}