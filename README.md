# Azure Storage Container Backup Function
This is a v2 function for replicating blobs to backup containers. In the pre terraform Azure subscription we implemented event grid triggers to fire the function from blob creation. The function then performs a lookup to find the corresponding storage account and copy the blob across.

## Pre-requisites
The function requires two json configuration files to map storage accounts to their corresponding backup accounts. These files are not to be checked into source control. The currently deployed configurations can be downloaded from 1password. They should look like the below example and be located at the root of the BlobBackup project:

origionalStorage.json
```
{
  "productionStorageAccount": "connectionString"
}

```
backupStorage.json
```
{
  "productionStorageAccountbackup": "connectionString"
}
```

# Azure Cosmos DB Backup Function
This is a v2 function for replicating cosmos documents to blob storage. This function subscribes directly to the snapshots production cosmos (the only one needing a back at time of writing) and does not use event grid.

## Pre-requisites
The function requires a local.settings.json file for local development, this looks something like:

```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "CosmosConnection": "",
    "CosmosBackupConnection": ""
  }
}
```
The properties in the ```Values``` block have to set as application settings in the azure function app.

# Deployments
There is no CI pipeline for these functions as they rarely have to be changed, usually an update is deployed/published straight from visual studio to the function app named CoyoteBlobBackup. The function app can be found in the PaaS resource group.