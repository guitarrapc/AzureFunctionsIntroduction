using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Utf8Json;

namespace AzureFunctionsIntroduction
{
    public static class HttpRequestMessageExtensions
    {
        // Hack : GetQueryNameValuePairs not yet umplemented with AzureFunctions v2 default.
        public static Dictionary<string, StringValues> GetQueryNameValuePairs(this HttpRequestMessage req)
        {
            return Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(req.RequestUri.Query);
        }

        // Hack : existing CreateResponse throw exception. Issue : https://github.com/Azure/azure-webjobs-sdk/issues/1492
        /// <summary>
        /// Custom CreateResponse to for sugar syntax...
        /// </summary>
        /// <param name="res"></param>
        /// <param name="statusCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static HttpResponseMessage CreateResponse(this HttpRequestMessage res, HttpStatusCode statusCode, string value)
        {
            if (res == null) throw new ArgumentNullException(nameof(res));
            return new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent(value),
            };
        }

        /// <summary>
        /// Custom CreateResponse to for sugar syntax...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="req"></param>
        /// <param name="statusCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static HttpResponseMessage CreateResponse<T>(this HttpRequestMessage req, HttpStatusCode statusCode, T value) where T : class
        {
            if (req == null) throw new ArgumentNullException(nameof(req));

            return new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent(JsonSerializer.ToJsonString<T>(value)),
            };
        }

        /// <summary>
        /// Custom CreateErrorResponse to for sugar syntax... no concider for HttpError
        /// </summary>
        /// <param name="req"></param>
        /// <param name="statusCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage req, HttpStatusCode statusCode, string value)
        {
            if (req == null) throw new ArgumentNullException(nameof(req));

            return new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent(value),
            };
        }
    }
}
