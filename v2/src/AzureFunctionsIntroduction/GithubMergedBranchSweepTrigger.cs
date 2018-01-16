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
                Action<SweeptargetBranch[]> action = branches => log.Info($@"DryRun - {nameof(GithubMergedBranchSweepTimer)}: remove candidates are following.
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
    }

    public class SweepResponse
    {
        public bool DryRun { get; set; }
        public SweeptargetBranch[] Value { get; set; }
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
}