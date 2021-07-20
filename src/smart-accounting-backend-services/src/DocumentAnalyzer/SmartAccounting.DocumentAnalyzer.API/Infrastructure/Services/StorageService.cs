using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
using SmartAccounting.DocumentAnalyzer.API.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SmartAccounting.DocumentAnalyzer.API.Infrastructure.Services
{
    internal interface IStorageService
    {
        Task DownloadBlobIfExistsAsync(Stream stream, string blobName, string containerName);
        Task UploadBlobAsync(Stream stream, string blobName, string containerName);
        Task DeleteBlobIfExistsAsync(string blobName, string containerName);
        Task<bool> DoesBlobExistAsync(string blobName, string containerName);
        Task<string> GetBlobUrlAsync(string blobName, string containerName);
        string GenerateSasTokenForBlob(string containerName, string blobName);
    }

    internal class StorageService : IStorageService
    {
        private readonly IStorageServiceConfiguration _storageServiceConfiguration;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<StorageService> _logger;
        public StorageService(IStorageServiceConfiguration storageServiceConfiguration,
                              BlobServiceClient blobServiceClient,
                              ILogger<StorageService> logger)
        {
            _storageServiceConfiguration = storageServiceConfiguration
                    ?? throw new ArgumentNullException(nameof(storageServiceConfiguration));
            _blobServiceClient = blobServiceClient
                    ?? throw new ArgumentNullException(nameof(blobServiceClient));
            _logger = logger
                    ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task DeleteBlobIfExistsAsync(string blobName, string containerName)
        {
            try
            {
                var container = await GetBlobContainer(containerName);
                var blockBlob = container.GetBlobClient(blobName);
                await blockBlob.DeleteIfExistsAsync();
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Document {blobName} was not deleted successfully - error details: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DoesBlobExistAsync(string blobName, string containerName)
        {
            try
            {
                var container = await GetBlobContainer(containerName);
                var blockBlob = container.GetBlobClient(blobName);
                var doesBlobExist = await blockBlob.ExistsAsync();
                return doesBlobExist.Value;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Document {blobName} existence cannot be verified - error details: {ex.Message}");
                throw;
            }
        }

        public async Task DownloadBlobIfExistsAsync(Stream stream, string blobName, string containerName)
        {
            try
            {
                var container = await GetBlobContainer(containerName);
                var blockBlob = container.GetBlobClient(blobName);

                await blockBlob.DownloadToAsync(stream);

            }

            catch (RequestFailedException ex)
            {
                _logger.LogError($"Cannot download document {blobName} - error details: {ex.Message}");
                if (ex.ErrorCode != "404")
                {
                    throw;
                }
            }
        }

        public async Task<string> GetBlobUrlAsync(string blobName, string containerName)
        {
            try
            {
                var container = await GetBlobContainer(containerName);
                var blockBlob = container.GetBlobClient(blobName);

                string blobUrl = blockBlob.Uri.AbsoluteUri;
                return blobUrl;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Url for document {blobName} was not found - error details: {ex.Message}");

                if (ex.ErrorCode != "404")
                {
                    throw;
                }

                return null;
            }
        }

        public async Task UploadBlobAsync(Stream stream, string blobName, string containerName)
        {
            try
            {
                Debug.Assert(stream.CanSeek);
                stream.Seek(0, SeekOrigin.Begin);
                var container = await GetBlobContainer(containerName);

                BlobClient blob = container.GetBlobClient(blobName);
                await blob.UploadAsync(stream);
            }

            catch (RequestFailedException ex)
            {
                _logger.LogError($"Document {blobName} was not uploaded successfully - error details: {ex.Message}");
                throw;
            }
        }

        private async Task<BlobContainerClient> GetBlobContainer(string containerName)
        {
            try
            {
                BlobContainerClient container = _blobServiceClient
                                .GetBlobContainerClient(containerName);

                await container.CreateIfNotExistsAsync();

                return container;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Cannot find blob container: {containerName} - error details: {ex.Message}");
                throw;
            }
        }

        public string GenerateSasTokenForBlob(string containerName, string blobName)
        {
            BlobSasBuilder builder = new BlobSasBuilder();
            builder.BlobContainerName = containerName;
            builder.ContentDisposition = "inline";
            builder.ContentType = "image/png";
            builder.BlobName = blobName;
            builder.Resource = "b";
            builder.SetPermissions(BlobSasPermissions.Read);
            builder.StartsOn = DateTimeOffset.UtcNow;
            builder.ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(90);
            var sasToken = builder
                        .ToSasQueryParameters(new StorageSharedKeyCredential(_storageServiceConfiguration.AccountName,
                                                                             _storageServiceConfiguration.Key))
                        .ToString();
            return sasToken;
        }
    }
}
