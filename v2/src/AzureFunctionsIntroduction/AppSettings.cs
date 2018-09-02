using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctionsIntroduction
{
    public static class AppSettings
    {
        public static string EnvKeyVaultSecretUri => EnvironmentHelper.GetOrDefault("key_vault_secret_uri", "");
        public static string EnvKeyVaultSasBlobItemConnectionString => EnvironmentHelper.GetOrDefault("key_vault_sas_blob_item_connection_string", "");
        public static string EnvSasBlobPrimaryEndpoint => EnvironmentHelper.GetOrDefault("sas_blob_item_primary_endpoint", "");
        public static string EnvSasBlobItemContainer => EnvironmentHelper.GetOrDefault("sas_blob_item_container", "");
        public static string EnvSasBlobItemName => EnvironmentHelper.GetOrDefault("sas_blob_item_name", "");
        public static string EnvKeyVaultTableStorageConnectionString => EnvironmentHelper.GetOrDefault("key_vault_table_storage_connection_string", "");
        public static string EnvTableStorageTableName => EnvironmentHelper.GetOrDefault("table_storage_asset_table_name", "");
        public static string EnvTableStorageTableDefaultPartition => EnvironmentHelper.GetOrDefault("table_storage_asset_table_default_parition", "");
        public static string EnvTableStorageTableDefaultAssetName => EnvironmentHelper.GetOrDefault("table_storage_asset_table_default_assetname", "");
    }
}
