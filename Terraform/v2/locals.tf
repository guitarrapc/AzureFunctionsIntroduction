locals {
  common_tags {
    environment = "function-v2"
  }
}

locals {
  location = "japanwest"
}

locals {
  function_name     = "function-v2"
  function_group_id = ""
}

locals {
  CLOUD_SHELL_SP_OBJECT_ID_LIST = [
    # cloud shell
    "e8b9e5e1-9148-4576-ad7a-2a0643df3155",
  ]
}

locals {
  vault_secret_name_test                                      = "test"
  vault_secret_name_FUNCTION_APP_EVENTTRIGGER_SLACKWEBHOOKURL = "functionappeventtriggerslackwebhookurl"
  vault_secret_name_FUNCTION_APP_SLACKINCOMINGWEBHOOKURL      = "functionappslackincomingwebhookurl"
  vault_secret_name_FUNCTION_SAS_BLOB_ITEM_CONNECTION_STRING  = "functionsasblobitemconnectionstring"
}
