# PlayFabMatchHistoryExtension

# Prerequisites
* An [Azure Subscription](https://azure.microsoft.com/en-us/solutions/gaming/)
* [An Azure Service Principal](https://docs.microsoft.com/en-us/azure/active-directory/develop/howto-create-service-principal-portal)
* [Terraform](https://terraform.io)
* [.NET Core 3.1](https://dot.net)

# Development Prerequisites
* An [Azure Subscription](https://azure.microsoft.com/en-us/solutions/gaming/)
* [.NET Core 3.1 SDK](https://dot.net)
* CosmosDB, either on Azure, or using the [CosmosDB Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21)
* Azure BlobStorage, or use [Azurite](https://github.com/Azure/Azurite) as a local storage emulator.

# Deploying
## Terraform
### Environment Variables
| Name     | Description    |
|----------|----------|
| `AZURE_SUBSCRIPTION_ID` | Azure Subscription ID; `id` property when executing `az account show` |
| `AZURE_TENANT_ID` | Azure Tenant ID; `tenantId` property when executing `az account show` |
| `TF_VAR_pf_title_id` | A PlayFab Title ID to authenticate against |
| `TF_VAR_pf_developer_secret` | A PlayFab Developer Secret for the above Title |


### Init Terraform:

Before initializing, set up a copy of `example.backend.config.tf`, and configure an existing storage account and container to be used for Terraform state management.

Then, init:

    terraform init -backend-config='.\config\backend.local.config.tf'

### Apply
    
    terraform apply
