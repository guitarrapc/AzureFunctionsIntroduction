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
        public static Dictionary<string, StringValues> GetQueryNameValuePairs(this HttpRequestMessage req)
        {
            return Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(req.RequestUri.Query);
        }

        public static HttpResponseMessage CreateResponse(this HttpRequestMessage res, HttpStatusCode statusCode, string value)
        {
            if (res == null) throw new ArgumentNullException(nameof(res));
            return new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent(value),
            };
        }

        public static HttpResponseMessage CreateResponse<T>(this HttpRequestMessage req, HttpStatusCode statusCode, T value) where T : class
        {
            if (req == null) throw new ArgumentNullException(nameof(req));

            return new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent(JsonSerializer.ToJsonString<T>(value)),
            };
        }

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
