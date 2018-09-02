using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionsIntroduction
{
    internal static class KeyVaultHelper
    {
        private static HttpClient client = new HttpClient();
        private static KeyVaultClient kvClient = null;
        private static readonly Dictionary<string, SecretBundle> memo = new Dictionary<string, SecretBundle>();

        public static async Task<string> GetSecretValueAsync(string key, bool useMemo = true)
        {
            if (kvClient == null)
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback), client);
            }

            if (useMemo && memo.TryGetValue(key, out var bundle))
            {
                return bundle.Value;
            }
            else
            {
                bundle = (await kvClient?.GetSecretAsync(key));
                return bundle.Value;
            }
        }
    }
}
