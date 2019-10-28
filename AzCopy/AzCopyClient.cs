using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.DataMovement;

namespace AzCopy
{
    public static class AzCopyClient
    {
        public static async Task CopyBlob(string sourceConnection, string destinationConnection, string containerName, string blobName)
        {
            var sourceAccount = CloudStorageAccount.Parse(sourceConnection);
            var destinationAccount = CloudStorageAccount.Parse(destinationConnection);

            var sourceClient = sourceAccount.CreateCloudBlobClient();
            var destinationClient = destinationAccount.CreateCloudBlobClient();

            var destContainer = destinationClient.GetContainerReference(containerName);
            destContainer.CreateIfNotExists();
            
            var sourceBlob = sourceClient.GetContainerReference(containerName).GetBlobReference(blobName);
            var destBlob = destContainer.GetBlobReference(blobName);

            var transferContext = new SingleTransferContext
            {
                ShouldOverwriteCallbackAsync = TransferContext.ForceOverwrite
            };

            await TransferManager.CopyAsync(sourceBlob, destBlob, true,null, transferContext);
        }

        public static async Task UploadBlob(string connection, string container, string blobName, string blobBody)
        {
            var account = CloudStorageAccount.Parse(connection);
            var client = account.CreateCloudBlobClient();
            var containerRef = client.GetContainerReference(container);
            containerRef.CreateIfNotExists();
            var blob = containerRef.GetBlockBlobReference(blobName);

            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(blobBody);
                writer.Flush();
                stream.Position = 0;
                
                var transferContext = new SingleTransferContext
                {
                    ShouldOverwriteCallbackAsync = TransferContext.ForceOverwrite
                };
                
                await TransferManager.UploadAsync(stream, blob,null,transferContext);
            }
        }
    }
}
