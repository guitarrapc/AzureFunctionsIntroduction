data "azurerm_builtin_role_definition" "contributor" {
  name = "Contributor"
}

# Current
data "azurerm_subscription" "current" {}

# Will not work with MSI.
# Uncomment when you want to treat with localmachine.
#data "azurerm_client_config" "current" {}

# resource group
data "azurerm_resource_group" "current" {
  name = "function-v1"
}

# key vault secret
# data "azurerm_key_vault_secret" "FUNCTION_APP_EVENTTRIGGER_SLACKWEBHOOKURL" {
#   name      = "${azurerm_key_vault_secret.FUNCTION_APP_EVENTTRIGGER_SLACKWEBHOOKURL.name}"
#   vault_uri = "${azurerm_key_vault.this.vault_uri}"
# }


# data "azurerm_key_vault_secret" "FUNCTION_APP_SLACKINCOMINGWEBHOOKURL" {
#   name      = "${azurerm_key_vault_secret.FUNCTION_APP_SLACKINCOMINGWEBHOOKURL.name}"
#   vault_uri = "${azurerm_key_vault.this.vault_uri}"
# }

