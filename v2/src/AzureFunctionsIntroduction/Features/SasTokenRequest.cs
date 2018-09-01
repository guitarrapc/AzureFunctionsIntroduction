using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureFunctionsIntroduction.Features
{
    public class SasTokenRequest
    {
        public static TimeSpan Begin { get; private set; } = TimeSpan.FromMinutes(-10);
        public static TimeSpan Expiry { get; private set; }

        private CloudBlobClient client;

        public SasTokenRequest(string storageAccountConnectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            client = storageAccount.CreateCloudBlobClient();
        }

        /// <summary>
        /// BlobのSAS Token署名を生成します。Blobが空の場合、Container SAS Token署名を生成します。
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<string> GenerateDefaultSasTokenAsync(string container, string blob, SharedAccessBlobPermissions permission = SharedAccessBlobPermissions.Read)
        {
            Expiry = TimeSpan.FromMinutes(30);
            return await GenerateSasTokenAsync(client.GetContainerReference(container), blob, permission);
        }

        /// <summary>
        /// BlobのSAS Token署名を生成します。Blobが空の場合、Container SAS Token署名を生成します。
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<string> GenerateShortSasTokenAsync(string container, string blob, SharedAccessBlobPermissions permission = SharedAccessBlobPermissions.Read)
        {
            Expiry = TimeSpan.FromMinutes(5);
            return await GenerateSasTokenAsync(client.GetContainerReference(container), blob, permission);
        }

        /// <summary>
        /// BlobのSAS Token署名を生成します。Blobが空の場合、Container SAS Token署名を生成します。
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<string> GenerateLongSasTokenAsync(string container, string blob, SharedAccessBlobPermissions permission = SharedAccessBlobPermissions.Read)
        {
            Expiry = TimeSpan.FromDays(30);
            return await GenerateSasTokenAsync(client.GetContainerReference(container), blob, permission);
        }

        /// <summary>
        /// BlobのSAS Token署名を生成します。Blobが空の場合、Container SAS Token署名を生成します。
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<string> GenerateExpireTokenAsync(string container, string blob, SharedAccessBlobPermissions permission = SharedAccessBlobPermissions.Read)
        {
            Expiry = TimeSpan.FromMinutes(-5);
            return await GenerateSasTokenAsync(client.GetContainerReference(container), blob, permission);
        }

        private async Task<string> GenerateSasTokenAsync(CloudBlobContainer container, string blob, SharedAccessBlobPermissions permission, string policyName = "")
        {
            return string.IsNullOrWhiteSpace(blob)
                ? await GenerateContainerSasTokenAsync(container, permission)
                : await GenerateBlobSasTokenAsync(container, blob, permission);
        }

        private async Task<string> GenerateBlobSasTokenAsync(CloudBlobContainer container, string blobName, SharedAccessBlobPermissions permission, string policyName = "")
        {
            string sasBlobToken;
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference(blobName);
            if (string.IsNullOrWhiteSpace(policyName))
            {
                var policy = CreateAdhocPolicy(permission);
                sasBlobToken = blob.GetSharedAccessSignature(policy);
            }
            else
            {
                sasBlobToken = blob.GetSharedAccessSignature(null, policyName);
            }
            return sasBlobToken;
        }

        private async Task<string> GenerateContainerSasTokenAsync(CloudBlobContainer container, SharedAccessBlobPermissions permission, string policyName = "")
        {
            string sasBlobToken;
            await container.CreateIfNotExistsAsync();
            if (string.IsNullOrWhiteSpace(policyName))
            {
                var policy = CreateAdhocPolicy(permission);
                sasBlobToken = container.GetSharedAccessSignature(policy);
            }
            else
            {
                sasBlobToken = container.GetSharedAccessSignature(null, policyName);
            }
            return sasBlobToken;
        }

        private SharedAccessBlobPolicy CreateAdhocPolicy(SharedAccessBlobPermissions permission)
        {
            var policy = new SharedAccessBlobPolicy()
            {
                SharedAccessStartTime = DateTime.UtcNow + Begin,
                SharedAccessExpiryTime = DateTime.UtcNow + Expiry,
                Permissions = permission,
            };
            return policy;
        }
    }
}
