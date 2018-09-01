using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctionsIntroduction
{
    public static class AppSettings
    {
        public static string EnvKeyVaultSecretUri => "key_vault_secret_uri";
        public static string EnvKeyVaultSasBlobItemConnectionString => "key_vault_sas_blob_item_connection_string";
        public static string EnvSasBlobPrimaryEndpoint => "sas_blob_item_primary_endpoint";
        public static string EnvSasBlobItemContainer => "sas_blob_item_container";
        public static string EnvSasBlobItemName => "sas_blob_item_name";
    }
}
