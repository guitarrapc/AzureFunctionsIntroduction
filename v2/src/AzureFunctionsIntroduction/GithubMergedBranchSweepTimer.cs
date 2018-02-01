using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using AzureFunctionsIntroduction.Features.Github;
using static AzureFunctionsIntroduction.StaticHelpers.EnvironmentHelper;

namespace AzureFunctionsIntroduction
{
    public static class GithubMergedBranchSweepTimer
    {
        #region Reference Environment Variables
        // github owner
        private static readonly string envOwner = $"GithubMergedBranchSweepTimer_Owner";
        // github access token
        private static readonly string envToken = $"GithubMergedBranchSweepTimer_GithubAccessToken";
        // dryrun
        private static readonly string envDryRun = $"GithubMergedBranchSweepTimer_DryRun";
        // remove target branches since merged
        private static readonly string envDaysPast = $"GithubMergedBranchSweepTimer_DaysPast";
        // target repositories
        private static readonly string envTargetRepositories = $"GithubMergedBranchSweepTimer_TargetRepositories";
        // exclude branches from removable
        private static readonly string envExcludeBranches = $"GithubMergedBranchSweepTimer_ExludeBranches";
        // pr check count = envPRPageSize * envPRPageCount
        private static readonly string envPRPageSize = $"GithubMergedBranchSweepTimer_PRPageSize";
        private static readonly string envPRPageCount = $"GithubMergedBranchSweepTimer_PRPageCount";
        #endregion

        [FunctionName("GithubMergedBranchSweepTimer")]
        public static async Task Run([TimerTrigger("0 0 0 * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"{nameof(GithubMergedBranchSweepTimer)} : C# Timer trigger function executed at: {DateTime.Now}");

            // get value from Environment variable
            var owner = GetStringEnvironmentVariable(envOwner, "");
            var token = GetStringEnvironmentVariable(envToken, "");
            var dryRun = GetBoolEnvironmentVariable(envDryRun, true);
            var daysPast = GetIntEnvironmentVariable(envDaysPast, 1);
            var targetRepositories = GetStringArrayEnvironmentVariable(envTargetRepositories, null);
            var excludeBranches = GetStringArrayEnvironmentVariable(envExcludeBranches, GithubMergedBranchSweeper.DefaultExcludeBranchRule);
            // max pageSize : 100
            // prPageSize * prPageCount = 2000 is pretty enough for 95 percentile
            var prPageSize = GetIntEnvironmentVariable(envPRPageSize, 100);
            int? prPageCount = GetIntEnvironmentVariable(envPRPageCount, 20); ;

            // Show log
            log.Info($@"{envOwner} : {owner}
{envToken} : {(!string.IsNullOrEmpty(token) ? "******" : "")}
{envDryRun} : {dryRun}
{envDaysPast} : {daysPast}
{envPRPageSize} : {prPageSize}
{envPRPageCount} : {prPageCount}
{envTargetRepositories} : {targetRepositories.ToJoinedString(",")}
{envExcludeBranches} : {excludeBranches.ToJoinedString(",")}");

            // Run
            try
            {
                foreach (var repository in targetRepositories)
                {
                    var sweeper = new GithubMergedBranchSweeper(owner, token, repository)
                    {
                        DaysPast = daysPast,
                        DryRun = dryRun,
                        ExcludeBranches = excludeBranches,
                        PrPageCount = prPageCount,
                        PrPageSize = prPageSize,
                    };
                    Action<SweeptargetBranchResult[]> action = branches => log.Info($@"DryRun - {nameof(GithubMergedBranchSweepTimer)}: remove candidates are following.
{branches.Select(x => x.BranchName).ToJoinedString(Environment.NewLine)}");
                    var results = await sweeper.SweepAsync(log, action);
                }
            }
            catch (Exception ex)
            {
                log.Error($"{ex.Message} {ex.GetType().FullName} {ex.StackTrace}");
                throw;
            }
        }
    }
}
