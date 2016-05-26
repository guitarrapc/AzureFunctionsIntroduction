#r "Newtonsoft.Json"

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

// ref : To find .NET Framework versions by querying the registry in code (.NET Framework 4.5 and later)
// https://msdn.microsoft.com/en-us/library/hh925568.aspx#net_d
private static readonly Dictionary<int, System.Version> _dotNetFrameworkVersionDictionary = new Dictionary<int, System.Version>
{
    {378389, new Version(4,5,0)}, // .NET Framework 4.5
    {378675, new Version(4,5,1)}, // .NET Framework 4.5.1 installed with Windows 8.1
    {378758, new Version(4,5,1)}, // .NET Framework 4.5.1 installed on Windows 8, Windows 7 SP1, or Windows Vista SP2
    {379893, new Version(4,5,2)}, // .NET Framework 4.5.2
    {393295, new Version(4,6,0)}, // .NET Framework 4.6 installed with Windows 10
    {393297, new Version(4,6,0)}, // .NET Framework 4.6 installed on all other Windows OS versions
    {394254, new Version(4,6,1)}, // .NET Framework 4.6.1 installed on Windows 10
    {394271, new Version(4,6,1)}, // .NET Framework 4.6.1 installed on all other Windows OS versions
    {394747, new Version(4,6,2)}, // .NET Framework 4.6.2 Preview installed on Windows 10 Insider Preview Build 14295
    {394748, new Version(4,6,2)}, // .NET Framework 4.6.2 Preview installed on all other Windows OS versions
};

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($".NET Framework Version Response was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    log.Info(jsonContent);

    if (data.registryValue == null)
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, new
        {
            error = "Empty registryValue requested."
        });
    }

    int registryValue = 0;
    if (!int.TryParse(data.registryValue.ToString(), out registryValue))
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, new
        {
            error = "invalid registryValue requested. registryValue must be int."
        });
    }

    try
    {
        Version version = null;
        _dotNetFrameworkVersionDictionary.TryGetValue((int)registryValue, out version);

        if (version == null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest, new
            {
                error = $"Could not detect version for requested. {nameof(registryValue)} : {registryValue}"
            });
        }

        // 終了
        return req.CreateResponse(HttpStatusCode.OK, new
        {
            Version = version.ToString(),
        });
    }
    catch (System.Exception ex)
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, new
        {
            Body = "{ex.Message}{Environment.NewLine}{ex.StackTrace}",
        });
    }
}

