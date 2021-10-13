# Configure the Azure provider
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 2.48"
    }
  }

  backend "azurerm" {}
}

provider "azurerm" {
  features {}
}

locals {
  prefix = "pfmatchhistory"
}

resource "azurerm_resource_group" "rg" {
  name     = "${var.prefix}-rg"
  location = var.location
}

resource "azurerm_storage_account" "storage" {
  name                     = replace("${var.prefix}storage", "-", "")
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_kind             = "StorageV2"
  account_tier             = "Standard"
  account_replication_type = "LRS"
  access_tier              = "Hot"
}

resource "azurerm_storage_queue" "queue" {
  name                 = "player-finished-match-queue"
  storage_account_name = azurerm_storage_account.storage.name
}

module "cosmosdb" {
  source         = "./cosmosdb"
  resource_group = azurerm_resource_group.rg
  tags           = var.tags
  prefix         = var.prefix
  container_name = "match"
  partition_key = "/playerEntityId"
}


resource "azurerm_app_service_plan" "app_service_plan" {
  name                = "${var.prefix}-app-service-plan"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  kind                = "FunctionApp"

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_function_app" "function_eventingestor" {
  name                       = "${var.prefix}-eventingestor-function"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  app_service_plan_id        = azurerm_app_service_plan.app_service_plan.id
  storage_account_name       = azurerm_storage_account.storage.name
  storage_account_access_key = azurerm_storage_account.storage.primary_access_key

  enabled                = true
  enable_builtin_logging = true
  https_only             = true
  version                 = "~3"

  connection_string {
    name  = "CosmosDb"
    type  = "Custom"
    value = module.cosmosdb.connection_strings[0]
  }

  site_config {
    ftps_state = "Disabled"
    //    ip_restriction = []
    min_tls_version = "1.2"
  }
}

resource "azurerm_function_app" "function_publicapi" {
  name                       = "${var.prefix}-publicapi-function"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  app_service_plan_id        = azurerm_app_service_plan.app_service_plan.id
  storage_account_name       = azurerm_storage_account.storage.name
  storage_account_access_key = azurerm_storage_account.storage.primary_access_key

  enabled                = true
  enable_builtin_logging = true
  https_only             = true
  version                 = "~3"

  connection_string {
    name  = "CosmosDb"
    type  = "Custom"
    value = module.cosmosdb.connection_strings[0]
  }

  site_config {
    ftps_state = "Disabled"
    //    ip_restriction = []
    min_tls_version = "1.2"
  }
}
