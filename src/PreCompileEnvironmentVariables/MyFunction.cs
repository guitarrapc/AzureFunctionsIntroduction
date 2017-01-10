using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PreCompileEnvironmentVariables
{
    public class MyFunction
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req)
        {
            Console.WriteLine($"Webhook was triggered!");

            var appKey = "FooKey";
            var appValue = Environment.GetEnvironmentVariable(appKey);
            Console.WriteLine($"App Setting. Key : {appKey}, Value : {appValue}");

            string jsonContent = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(jsonContent);

            if (data.first == null || data.last == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "Please pass first/last properties in the input object"
                });
            }

            return req.CreateResponse(HttpStatusCode.OK, new
            {
                greeting = $"Hello {data.first} {data.last}!"
            });
        }
    }
}
