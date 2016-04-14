#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

public static string Test()
{
    return "External function loaded!";
}

public static void Test2(TraceWriter log)
{
    log.Verbose($"External function2 loaded!");
}