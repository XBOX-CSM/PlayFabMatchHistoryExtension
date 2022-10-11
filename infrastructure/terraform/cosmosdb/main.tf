resource "azurerm_cosmosdb_account" "cosmosdb_account" {
  name                          = "${var.prefix}-cosmosdb"
  location                      = var.resource_group.location
  resource_group_name           = var.resource_group.name
  offer_type                    = "Standard"
  kind                          = "GlobalDocumentDB"
  enable_free_tier              = false
  analytical_storage_enabled    = false
  public_network_access_enabled = true

  capabilities {
    name = "EnableServerless"
  }

  consistency_policy {
    consistency_level = "Strong"
  }

  geo_location {
    location          = var.resource_group.location
    failover_priority = 0
  }

  tags = var.tags
}

resource "azurerm_cosmosdb_sql_database" "db" {
  name                = "${var.prefix}-db"
  resource_group_name = azurerm_cosmosdb_account.cosmosdb_account.resource_group_name
  account_name        = azurerm_cosmosdb_account.cosmosdb_account.name
  # Must not set "throughput" for serverless
}

resource "azurerm_cosmosdb_sql_container" "match_container" {
  name                = var.container_name
  resource_group_name = azurerm_cosmosdb_account.cosmosdb_account.resource_group_name
  account_name        = azurerm_cosmosdb_account.cosmosdb_account.name
  database_name       = azurerm_cosmosdb_sql_database.db.name

  partition_key_path    = var.partition_key
  partition_key_version = 1
  # Must not set "throughput" for serverless

  indexing_policy {
    indexing_mode = "Consistent"

    included_path {
      path = "/*"
    }

    included_path {
      path = "/included/?"
    }

    excluded_path {
      path = "/excluded/?"
    }
  }

  #   unique_key {
  #     paths = ["/definition/idlong", "/definition/idshort"]
  #   }
}
