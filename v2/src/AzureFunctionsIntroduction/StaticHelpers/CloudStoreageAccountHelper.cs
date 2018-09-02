using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionsIntroduction
{
    public static class CloudStoreageAccountHelper
    {
        public static CloudTable GetTableReference(string connectionString, string tableName)
        {
            return CloudStorageAccount.Parse(connectionString).CreateCloudTableClient().GetTableReference(tableName);
        }

        public static async Task<T> RetrieveAsync<T>(this CloudTable cloudTable, string partitionKey, string rowKey) where T : TableEntity
        {
            var tableResult = await cloudTable.ExecuteAsync(TableOperation.Retrieve<T>(partitionKey, rowKey));
            return (T)tableResult.Result;
        }
    }
}
