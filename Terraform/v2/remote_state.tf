# data "terraform_remote_state" "global" {
#   backend = "azurerm"
#   config {
#     storage_account_name = "terraformguitarrapc"
#     container_name = "azurefunctionsintroduction"
#     key = "azure/v2/terraform.tfstate"
#     resource_group_name = "terraform"
#   }
# }

