using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzCopy;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Document = Microsoft.Azure.Documents.Document;

namespace BlobBackup
{
    public static class CosmosBackup
    {
        [FunctionName("BackupCosmos")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "",
            collectionName: "",
            ConnectionStringSetting = "CosmosConnection",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionName = "backupLease")]IReadOnlyList<Document> documents, ILogger log)
        {
            var connection = GetEnvironmentVariable("CosmosBackupConnection");

            foreach (var document in documents)
            {
                log.LogInformation("Backing up document " + document.Id);

                await AzCopyClient.UploadBlob(connection,"",document.Id, document.ToString());

                log.LogInformation("Backed up document " + document.Id);
            }
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
