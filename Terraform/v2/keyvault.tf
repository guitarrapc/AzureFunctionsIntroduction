locals {
  keyvaultname = "function-v2-kv"
}

resource "azurerm_key_vault" "this" {
  name                = "${local.keyvaultname}"
  location            = "${local.location}"
  resource_group_name = "${data.azurerm_resource_group.current.name}"

  sku {
    name = "standard"
  }

  tenant_id                   = "${var.TENANT_ID}"
  enabled_for_disk_encryption = true

  tags = "${merge(
        var.common_tags,
        local.common_tags,
    )}"
}

resource "azurerm_key_vault_access_policy" "this" {
  vault_name          = "${azurerm_key_vault.this.name}"
  resource_group_name = "${azurerm_key_vault.this.resource_group_name}"
  tenant_id           = "${azurerm_key_vault.this.tenant_id}"
  object_id           = "${var.SP_OBJECT_ID}"

  key_permissions = [
    "list",
    "create",
    "get",
  ]

  secret_permissions = [
    "list",
    "get",
    "set",
  ]

  certificate_permissions = [
    "get",
  ]
}

resource "azurerm_key_vault_access_policy" "current" {
  vault_name          = "${azurerm_key_vault.this.name}"
  resource_group_name = "${azurerm_key_vault.this.resource_group_name}"
  tenant_id           = "${data.azurerm_client_config.current.tenant_id}"
  object_id           = "${data.azurerm_client_config.current.service_principal_object_id}"

  key_permissions = [
    "list",
    "create",
    "get",
  ]

  secret_permissions = [
    "list",
    "get",
    "set",
  ]

  certificate_permissions = [
    "get",
  ]
}

resource "azurerm_key_vault_access_policy" "secret_readers" {
  vault_name          = "${azurerm_key_vault.this.name}"
  resource_group_name = "${azurerm_key_vault.this.resource_group_name}"
  tenant_id           = "${data.azurerm_client_config.current.tenant_id}"
  object_id           = "${azurerm_function_app.function.id}"
  key_permissions     = []

  secret_permissions = [
    "get",
    "list",
  ]

  certificate_permissions = []

  depends_on = ["azurerm_function_app.function"]
}

resource "azurerm_key_vault_secret" "test" {
  name      = "test"
  value     = "1234"
  vault_uri = "${azurerm_key_vault.this.vault_uri}"

  depends_on = ["azurerm_key_vault.this"]

  tags = "${merge(
        var.common_tags,
        local.common_tags,
    )}"
}

resource "azurerm_key_vault_secret" "FUNCTION_APP_EVENTTRIGGER_SLACKWEBHOOKURL" {
  name      = "functionappeventtriggerslackwebhookurl"
  value     = "${var.FUNCTION_APP_EVENTTRIGGER_SLACKWEBHOOKURL}"
  vault_uri = "${azurerm_key_vault.this.vault_uri}"

  depends_on = ["azurerm_key_vault.this"]

  tags = "${merge(
        var.common_tags,
        local.common_tags,
    )}"
}

resource "azurerm_key_vault_secret" "FUNCTION_APP_SLACKINCOMINGWEBHOOKURL" {
  name      = "functionappslackincomingwebhookurl"
  value     = "${var.FUNCTION_APP_SLACKINCOMINGWEBHOOKURL}"
  vault_uri = "${azurerm_key_vault.this.vault_uri}"

  depends_on = ["azurerm_key_vault.this"]

  tags = "${merge(
        var.common_tags,
        local.common_tags,
    )}"
}
