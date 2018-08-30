resource "azurerm_storage_account" "blob" {
  name                     = "azurefunctionblob"
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

resource "azurerm_storage_container" "blob" {
  name                  = "assets"
  resource_group_name   = "${data.azurerm_resource_group.current.name}"
  storage_account_name  = "${azurerm_storage_account.blob.name}"
  container_access_type = "private"
}

resource "azurerm_storage_blob" "blob" {
  name = "sample"

  resource_group_name    = "${data.azurerm_resource_group.current.name}"
  storage_account_name   = "${azurerm_storage_account.blob.name}"
  storage_container_name = "${azurerm_storage_container.blob.name}"

  type = "block"
  size = 512
}
