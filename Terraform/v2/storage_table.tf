resource "azurerm_storage_account" "table" {
  name                     = "azurefunctiontable"
  resource_group_name      = "${data.azurerm_resource_group.current.name}"
  location                 = "${local.location}"
  account_tier             = "Standard"
  account_replication_type = "LRS"

  enable_https_traffic_only = true

  tags = "${merge(
        var.common_tags,
        local.common_tags,
    )}"
}

resource "azurerm_storage_table" "table" {
  name                 = "sampletable"
  resource_group_name  = "${data.azurerm_resource_group.current.name}"
  storage_account_name = "${azurerm_storage_account.table.name}"
}
