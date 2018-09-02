using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFunctionsIntroduction.Domains.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsIntroduction
{
    public static class StorageTableWebhook
    {
        [FunctionName("StorageTableWebhook")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Obtain ConnectionString from KeyVault
            var connectionString = await KeyVaultHelper.GetSecretValueAsync(EnvironmentHelper.GetOrDefault(AppSettings.EnvKeyVaultTableStorageConnectionString, ""));

            // TableReference
            var table = CloudStoreageAccountHelper.GetTableReference(connectionString, EnvironmentHelper.GetOrDefault(AppSettings.EnvTableStorageTableName, ""));
            var entity = await table.RetrieveAsync<AssetEntity>(EnvironmentHelper.GetOrDefault(AppSettings.EnvTableStorageTableDefaultPartition, ""), EnvironmentHelper.GetOrDefault(AppSettings.EnvTableStorageTableDefaultAssetName, ""));
            log.LogInformation($"{entity.PartitionKey}\t{entity.RowKey}\t{entity.Timestamp}\t{entity.AssetName}");
            return req.CreateResponse(HttpStatusCode.OK, entity.AssetName);
        }
    }
}
