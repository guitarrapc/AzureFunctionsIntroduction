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
    eventtrigger_slackchannel            = "azurefunctions"
    eventtriggerSlackwebhookurlSecretUri = "${azurerm_key_vault.this.vault_uri}secrets/${azurerm_key_vault_secret.FUNCTION_APP_EVENTTRIGGER_SLACKWEBHOOKURL.name}"
    KeyVaultSecretUri                    = "${azurerm_key_vault.this.vault_uri}secrets/${azurerm_key_vault_secret.test.name}"
    slackIncomingWebhookUrlSecretUri     = "${azurerm_key_vault.this.vault_uri}secrets/${azurerm_key_vault_secret.FUNCTION_APP_SLACKINCOMINGWEBHOOKURL.name}"

    # eventtrigger_slackwebhookurl = "${data.azurerm_key_vault_secret.FUNCTION_APP_EVENTTRIGGER_SLACKWEBHOOKURL.value}"
    # SlackIncomingWebhookUrl      = "${data.azurerm_key_vault_secret.FUNCTION_APP_SLACKINCOMINGWEBHOOKURL.value}"
  }

  identity = {
    type = "SystemAssigned"
  }

  depends_on = [
    "azurerm_key_vault_secret.FUNCTION_APP_EVENTTRIGGER_SLACKWEBHOOKURL", 
    "azurerm_key_vault_secret.test", 
    "azurerm_key_vault_secret.FUNCTION_APP_SLACKINCOMINGWEBHOOKURL",
  ]

  tags = "${merge(
        var.common_tags,
        local.common_tags,
    )}"
}
