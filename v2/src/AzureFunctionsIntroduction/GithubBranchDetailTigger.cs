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
                var sweeper = new GithubMergedBranchSweeper(request.Owner, token, request.Repository)
                {
                    ExcludeBranches = request.ExcludeBranches,
                };
                var branches = await sweeper.GetBranchesAsync();
                var results = await sweeper.GetBranchDetailsAsync(branches);

                // Response
                var response = new BranchResponse() { Count = results.Length, Owner = request.Owner, Repository = request.Repository, Value = results };
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
            public string Owner { get; set; }
            public string Repository { get; set; }
            public BranchDetailResult[] Value { get; set; }

            public string[] ToChatworkInfoString(int daysPast)
            {
                var values = Value.Where(x => x.LastDate < DateTime.Now.AddDays(-daysPast))
                .GroupBy(x => string.IsNullOrEmpty(x.Commiter) ? "unknown" : x.Commiter)
                .OrderByDescending(x => x.Count())
                .Select(x =>
                {
                    var text = x.Select(y => $"{y.LastDate.ToString("g")}: {y.Name} ({y.Url})").ToArray();
                    return $@"[info][title]{Repository}: {x.Key} ({daysPast}days past branches: {text.Count()}åè)[/title]{text.ToJoinedString(Environment.NewLine)}[/info]";
                })
                .ToArray();
                return values;
            }
        }
    }
}