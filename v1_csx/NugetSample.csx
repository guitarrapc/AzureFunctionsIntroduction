#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using Octokit;
using Octokit.Internal;

public static string NugetTest()
{
     var hoge = typeof(Octokit.GitHubClient).Assembly.FullName;
     return hoge;
}