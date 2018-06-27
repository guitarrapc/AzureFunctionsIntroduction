using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionsIntroduction
{
    public static class EnvironmentHelper
    {
        public static string Get(string key, string defaultValue)
        {
            return Environment.GetEnvironmentVariable(key) ?? defaultValue;
        }

        public static string[] Get(string key, string[] defaultValue)
        {
            return Environment.GetEnvironmentVariable(key)?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries) ?? defaultValue;
        }

        public static int Get(string key, int defaultValue)
        {
            return int.TryParse(Environment.GetEnvironmentVariable(key), out var result)
                ? result
                : defaultValue;
        }

        public static bool Get(string key, bool defaultValue)
        {
            return bool.TryParse(Environment.GetEnvironmentVariable(key), out var result)
                ? result
                : defaultValue;
        }
    }
}
