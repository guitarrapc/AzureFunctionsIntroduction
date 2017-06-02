using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;

namespace ExternalProjectReferenceCSharp
{
    public class TestMethod
    {
        public static string Test()
        {
            return "External function loaded!";
        }

        public static void Test2(TraceWriter log)
        {
            log.Verbose($"External function2 loaded!");
        }
    }
}