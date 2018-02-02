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
    public static class GithubBranchDetailTigger
    {
        private static readonly string tokenHeader = "X-Github-AccessToken";

        [FunctionName("GithubBranchDetailTigger")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"{nameof(GithubBranchDetailTigger)} : C# HTTP trigger function processed a request.");

            var token = req.Headers.Where(x => x.Key == tokenHeader)
                .SelectMany(x => x.Value)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(token))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, $"Missing AccessToken from Header : {tokenHeader}");
            }

            var json = await req.Content.ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<GithubBranchDetailRequest>(json);

            // Validation
            if (string.IsNullOrEmpty(token))
                return req.CreateResponse(HttpStatusCode.BadRequest, $"{nameof(token)} query element must be included.");
            if (string.IsNullOrEmpty(request.Owner))
                return req.CreateResponse(HttpStatusCode.BadRequest, $"{nameof(request.Owner)} posted json element must be included.");
            if (string.IsNullOrEmpty(request.Repository))
                return req.CreateResponse(HttpStatusCode.BadRequest, $"{nameof(request.Repository)} posted json element must be included.");

            try
            {
                // Execute
                var sweeper = new GithubMergedBranchSweeper(request.Owner, token, request.Repository);
                var branches = await sweeper.GetBranchesAsync(request.ExcludeBranches);
                var results = await sweeper.GetBranchDetailsAsync(branches);

                // Response
                var response = new BranchResponse() { Count = results.Length, Value = results };
                var responseJson = JsonConvert.SerializeObject(response);
                return req.CreateResponse(HttpStatusCode.OK, responseJson);
            }
            catch (Exception ex)
            {
                log.Error($"{ex.Message} {ex.GetType().FullName} {ex.StackTrace}");
                return req.CreateResponse(HttpStatusCode.InternalServerError, $"Error when running sweep. {ex.GetType()} {ex.Message}");
            }
        }

        public class GithubBranchDetailRequest
        {
            public string Owner { get; set; }
            public string Repository { get; set; }
            public string[] ExcludeBranches { get; set; }
        }

        public class BranchResponse
        {
            public int Count { get; set; }
            public BranchDetailResult[] Value { get; set; }
        }
    }
}