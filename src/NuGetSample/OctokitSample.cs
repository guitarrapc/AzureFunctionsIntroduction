using Octokit;

namespace NuGetSample
{
    public class OctokitSample
    {
        public static string NugetTest()
        {
            var hoge = typeof(GitHubClient).Assembly.FullName;
            return hoge;
        }
    }
}