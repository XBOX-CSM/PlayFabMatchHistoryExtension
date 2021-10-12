output "connection_strings" {
  sensitive = true
  value     = azurerm_cosmosdb_account.cosmosdb_account.connection_strings
}