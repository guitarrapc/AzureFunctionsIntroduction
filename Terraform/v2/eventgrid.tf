locals {
  eventgrid_name = "eventgrid-function-v2"
}

resource "azurerm_resource_group" "eventgridtest" {
  name     = "${local.eventgrid_name}"
  location = "${local.location}"

  tags = "${merge(
    var.common_tags,
    local.common_tags,
    map(
      "Name", "${local.eventgrid_name}"
    )
  )}"
}

# No azurerm_eventgrid_subscription resource, yet.
# resource "azurerm_eventgrid_topic" "eventgridtest" {
#   name                = "${local.eventgrid_name}"
#   location            = "${local.location}"
#   resource_group_name = "${azurerm_resource_group.eventgridtest.name}"


#   tags = "${merge(
#     var.common_tags,
#     local.common_tags,
#     map(
#       "Name", "${local.eventgrid_name}"
#     )
#   )}"
# }

