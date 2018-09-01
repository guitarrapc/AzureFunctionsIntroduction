resource "azurerm_storage_account" "function" {
  name                     = "${replace("${local.function_name}", "-", "")}"
  location                 = "${local.location}"
  resource_group_name      = "${data.azurerm_resource_group.current.name}"
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = "${merge(
        var.common_tags,
        local.common_tags,
    )}"
}

resource "azurerm_app_service_plan" "function" {
  name                = "${local.function_name}-serviceplan"
  location            = "${local.location}"
  resource_group_name = "${data.azurerm_resource_group.current.name}"
  kind                = "FunctionApp"

  sku {
    tier = "Dynamic"
    size = "Y1"
  }

  tags = "${merge(
        var.common_tags,
        local.common_tags,
    )}"
}

resource "azurerm_function_app" "function" {
  name                      = "${local.function_name}-function"
  location                  = "${local.location}"
  resource_group_name       = "${data.azurerm_resource_group.current.name}"
  app_service_plan_id       = "${azurerm_app_service_plan.function.id}"
  storage_connection_string = "${azurerm_storage_account.function.primary_connection_string}"

  https_only = true
  version    = "beta"

  app_settings {
    # As of 2.0.1-beta.26 a worker runtime setting is required.
    FUNCTIONS_WORKER_RUNTIME                       = "dotnet"
    eventtrigger_slackchannel                      = "azurefunctions"
    key_vault_eventtriggerSlackwebhookurlSecretUri = "${azurerm_key_vault.this.vault_uri}secrets/${local.vault_secret_name_FUNCTION_APP_EVENTTRIGGER_SLACKWEBHOOKURL}"
    key_vault_secret_uri                           = "${azurerm_key_vault.this.vault_uri}secrets/${local.vault_secret_name_test}"
    key_vault_slackIncomingWebhookUrlSecretUri     = "${azurerm_key_vault.this.vault_uri}secrets/${local.vault_secret_name_FUNCTION_APP_SLACKINCOMINGWEBHOOKURL}"
    key_vault_sas_blob_item_connection_string      = "${azurerm_key_vault.this.vault_uri}secrets/${local.vault_secret_name_FUNCTION_SAS_BLOB_ITEM_CONNECTION_STRING}"
    sas_blob_item_primary_endpoint                 = "${azurerm_storage_account.blob.primary_blob_endpoint}"
    sas_blob_item_container                        = "${azurerm_storage_container.blob.name}"
    sas_blob_item_name                             = "${azurerm_storage_blob.blob.name}"

    # Incase you want set secret directly from KeyVault.
    # eventtrigger_slackwebhookurl = "${data.azurerm_key_vault_secret.FUNCTION_APP_EVENTTRIGGER_SLACKWEBHOOKURL.value}"
    # SlackIncomingWebhookUrl      = "${data.azurerm_key_vault_secret.FUNCTION_APP_SLACKINCOMINGWEBHOOKURL.value}"
  }

  identity = {
    type = "SystemAssigned"
  }

  depends_on = [
    "azurerm_key_vault.this",
  ]

  tags = "${merge(
        var.common_tags,
        local.common_tags,
    )}"
}
