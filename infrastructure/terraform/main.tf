﻿# Configure the Azure provider
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

locals {
  eventingestor_function_name = "${var.prefix}-eventingestor-function"
  publicapi_function_name = "${var.prefix}-publicapi-function"
}

resource "azurerm_application_insights" "apinsights_eventingestor" {
  name                = "${local.eventingestor_function_name}-appinsights"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
}

resource "azurerm_function_app" "function_eventingestor" {
  name                       = local.eventingestor_function_name
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
  
  connection_string {
    name  = "EventQueueStorage"
    type  = "Custom"
    value = azurerm_storage_account.storage.primary_connection_string
  }
  
  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY" = "${azurerm_application_insights.apinsights_eventingestor.instrumentation_key}"
  }

  site_config {
    ftps_state = "Disabled"
    //    ip_restriction = []
    min_tls_version = "1.2"
  }
}

resource "azurerm_application_insights" "apinsights_publicapi" {
  name                = "${local.publicapi_function_name}-appinsights"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
}

resource "azurerm_function_app" "function_publicapi" {
  name                       = local.publicapi_function_name
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

  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY" = "${azurerm_application_insights.apinsights_publicapi.instrumentation_key}"
  }
  
  site_config {
    ftps_state = "Disabled"
    //    ip_restriction = []
    min_tls_version = "1.2"
  }
}
