using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MiniBiggy {

    public class AzureBlobStorage : IDataStore {

        private CloudStorageAccount _storageAccount;
        private CloudBlobContainer _cloudBlobContainer;

        public string ConnectionString { get; }
        public string ContainerName { get; set; }
        public string BlobName { get; set; }
        public int VersionsToKeep { get; }

        public AzureBlobStorage(string connectionString, string containerName, string blobName, int versionsToKeep = 100) {
            ConnectionString = connectionString;
            ContainerName = containerName;
            BlobName = blobName;
            VersionsToKeep = versionsToKeep;
            EnsureCloudStorageAccount();
            EnsureContainer();
        }

        private void EnsureCloudStorageAccount() {
            if (!CloudStorageAccount.TryParse(ConnectionString, out _storageAccount)) {
                throw new StorageException("Could not parse storage connectionstring");
            }
        }

        private void EnsureContainer() {
            var client = _storageAccount.CreateCloudBlobClient();
            _cloudBlobContainer = client.GetContainerReference(ContainerName);
            _cloudBlobContainer.CreateIfNotExistsAsync().Wait();
            var blob = _cloudBlobContainer.GetBlockBlobReference(BlobName);
            if(!blob.ExistsAsync().Result) {
                blob.UploadFromByteArrayAsync(new byte[0], 0, 0).Wait();
            }
        }

        public async Task<byte[]> ReadAllAsync() {
            var mem = new MemoryStream();
            var block = _cloudBlobContainer.GetBlockBlobReference(BlobName);
            await block.DownloadToStreamAsync(mem);
            return mem.ToArray();
        }

        public async Task WriteAllAsync(byte[] list) {
            var blob = _cloudBlobContainer.GetBlockBlobReference(BlobName);
            await blob.SnapshotAsync();
            await blob.UploadFromByteArrayAsync(list, 0, list.Length);
            await DeleteOldSnapshots(blob);
        }

        private async Task DeleteOldSnapshots(CloudBlockBlob blob) {
            var snapshots = await blob.Container.ListBlobsSegmentedAsync(null, true, BlobListingDetails.Snapshots, null, null, null, null);
            var snaps = snapshots.Results
                                 .Cast<CloudBlockBlob>()
                                 .Where(x => x.IsSnapshot && x.Name == BlobName)
                                 .OrderByDescending(x => x.Properties.LastModified)
                                 .Skip(VersionsToKeep)
                                 .ToList();
            foreach(var snap in snaps) {
                await snap.DeleteAsync();
            }
        }
    }
}
