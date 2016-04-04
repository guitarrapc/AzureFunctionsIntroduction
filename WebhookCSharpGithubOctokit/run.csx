#r "Newtonsoft.Json"

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using Octokit;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Verbose($"Webhook was triggered!");
    
    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    if (data.owner == null || data.repository == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Please pass owner/repository properties in the input object"
        });
    }

    log.Verbose(typeof(Octokit.GitHubClient).Assembly.FullName);
    string owner = data.owner;
    string repository = data.repository;
    
    var client = new GitHubClient(new ProductHeaderValue("AzureFunctions"));
    var issues = await client.Repository.Commit.GetAll(owner, repository);
    var commits = issues.Select(x => x.Commit.Tree.Sha).ToArray();

    return req.CreateResponse(HttpStatusCode.OK, new {
        CommitSha = commits,
    });
}
