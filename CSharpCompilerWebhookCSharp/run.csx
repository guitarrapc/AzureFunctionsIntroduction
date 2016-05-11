#load "..\CSharpScripting.csx"

#r "MyExtensions.dll"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MyExtensions;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("Charp Compiler service Webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);
    
    if (data.code == null)
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "missing <code> property."
        });
    }

    // CSharp Code評価
    string code = data.code;
    log.Info($"{nameof(code)} : {code}");
    var resultText = await EvaluateCSharpAsync(code);
    log.Info(resultText);

    return req.CreateResponse(HttpStatusCode.OK, new {
        body = resultText,
    });
}