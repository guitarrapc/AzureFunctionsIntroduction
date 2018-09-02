using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFunctionsIntroduction.Domains.Entities;
using AzureFunctionsIntroduction.Features;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AzureFunctionsIntroduction
{
    public static class SasUrlRequestWebhookCSharp
    {
        private static HttpClient client = new HttpClient();
        private static KeyVaultClient kvClient = null;
        private static SecretBundle storageAccountConnectionStringBundle = null;

        [FunctionName("SasUrlRequestWebhookCSharp")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var request = await req.Content.ReadAsAsync<SasRequest>();

            // Run
            var response = await GetSasUrl(request);
            return req.CreateResponse<AssetBundleInfomationResponse>(HttpStatusCode.OK, response);
        }

        private static async Task<AssetBundleInfomationResponse> GetSasUrl(SasRequest request)
        {
            // key reference to table storage and return assetname.
            var asset = await GetBlobNameAsync(request?.Partition, request?.Key);

            // Obtain connection string from key vault
            if (kvClient == null || storageAccountConnectionStringBundle == null)
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback), client);
                storageAccountConnectionStringBundle = await kvClient.GetSecretAsync(AppSettings.EnvKeyVaultSasBlobItemConnectionString);
            }

            // Create SAS Token for every request
            var sasTokenReqeust = new SasTokenRequest(storageAccountConnectionStringBundle.Value);
            var sasToken = await sasTokenReqeust.GenerateDefaultSasTokenAsync(AppSettings.EnvSasBlobItemContainer, asset);
            var response = new AssetBundleInfomationResponse(new Uri(AppSettings.EnvSasBlobPrimaryEndpoint), AppSettings.EnvSasBlobItemContainer, asset, sasToken);

            return response;
        }

        private static async Task<string> GetBlobNameAsync(string partition, string key)
        {
            // Obtain ConnectionString from KeyVault
            var connectionString = await KeyVaultHelper.GetSecretValueAsync(AppSettings.EnvKeyVaultTableStorageConnectionString);
            // TableReference
            var table = CloudStoreageAccountHelper.GetTableReference(connectionString, AppSettings.EnvTableStorageTableName);

            if (string.IsNullOrWhiteSpace(partition))
            {
                partition = AppSettings.EnvTableStorageTableDefaultPartition;
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                key = AppSettings.EnvTableStorageTableDefaultAssetName;
            }

            var entity = await table.RetrieveAsync<AssetEntity>(partition, key);
            return entity.AssetName;
        }
    }

    public class SasRequest
    {
        public string Partition { get; set; }
        /// <summary>
        /// key == assetName
        /// </summary>
        public string Key { get; set; }
    }

    public class AssetBundleInfomationResponse
    {
        public string Id => Guid.NewGuid().ToString();
        public string SasUrl { get; }
        public string AssetName { get; }

        public AssetBundleInfomationResponse(Uri baseUrl, string container, string assetName, string sasToken)
        {
            SasUrl = new Uri(baseUrl, $"{container}/{assetName}") + sasToken;
            AssetName = assetName;
        }
    }
}
