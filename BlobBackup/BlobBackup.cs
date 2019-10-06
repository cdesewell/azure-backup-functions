using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzCopy;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BlobBackup
{
    public static class BlobBackup
    {
        [FunctionName("BackupBlob")]
        public static async Task BackupBlob([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log, ExecutionContext context)
        {
            var blob = eventGridEvent.Subject.Split("/blobs/")[1];
            var container = eventGridEvent.Subject.Split("/")[4];
            var storageAccount = eventGridEvent.Topic.Split("/")[8];

            log.LogInformation($"Backing up {blob} from container {container} in storage account {storageAccount}");

            var connection = GetConnection(storageAccount, context.FunctionAppDirectory);

            await AzCopyClient.CopyBlob(connection.OriginalConnection, connection.BackupConnection, container, blob);

            log.LogInformation($"Backed up {blob} from container {container} in storage account {storageAccount}");
        }

        public static (string OriginalConnection, string BackupConnection) GetConnection(string storageAccount, string configPath)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(configPath)
                .AddJsonFile($"originalStorage.json", optional: false)
                .AddJsonFile($"backupStorage.json", optional: false);

            var appConfig = builder.Build();

            var backupConnection = appConfig.GetValue<string>($"{storageAccount}backup");

            var originalConnection = appConfig.GetValue<string>($"{storageAccount}");

            return (originalConnection, backupConnection);
        }
    }
}