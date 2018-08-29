terraform {
  required_version = "= 0.11.7"

  backend "azurerm" {
    storage_account_name = "terraformguitarrapc"
    container_name       = "azurefunctionsintroduction"
    key                  = "azure/v1-attribute/terraform.tfstate"
    resource_group_name  = "terraform"
  }
}
