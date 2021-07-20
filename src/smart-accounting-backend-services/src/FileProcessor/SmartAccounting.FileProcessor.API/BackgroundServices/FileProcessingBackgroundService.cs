using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartAccounting.FileProcessor.API.Application.IntegrationEvents;
using SmartAccounting.FileProcessor.API.BackgroundServices.Channels;
using SmartAccounting.FileProcessor.API.Infrastructure.IntegrationEvents;
using SmartAccounting.FileProcessor.API.Infrastructure.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAccounting.FileProcessor.API.BackgroundServices
{
    internal class FileProcessingBackgroundService : BackgroundService
    {
        private readonly ILogger<FileProcessingBackgroundService> _logger;
        private readonly FileProcessingChannel _fileProcessingChannel;
        private readonly IStorageService _storageService;
        private readonly IServiceProvider _serviceProvider;

        public FileProcessingBackgroundService(
            ILogger<FileProcessingBackgroundService> logger,
            FileProcessingChannel boundedMessageChannel,
            IStorageService storageService,
            IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileProcessingChannel = boundedMessageChannel ?? throw new ArgumentNullException(nameof(boundedMessageChannel));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var fileProcessingChannelElement in _fileProcessingChannel.ReadAllAsync())
            {
                try
                {
                    await using var stream = File.OpenRead(fileProcessingChannelElement.FilePath);
                    var fileName = $"{Guid.NewGuid()}-{Path.GetFileName(fileProcessingChannelElement.FilePath)}";
                    var userIdentity = fileProcessingChannelElement.UserId;

                    await _storageService.UploadBlobAsync(stream, fileName, userIdentity);
                    _logger.LogInformation($"File {fileName} successfully processed");

                    var fileUrl = await _storageService.GetBlobUrl(fileName, userIdentity);

                    var fileSuccessfullyUploadedIntegrationEvent = new FileSuccessfullyUploadedIntegrationEvent()
                    {
                        CreationDate = DateTime.UtcNow,
                        Id = Guid.NewGuid(),
                        FileUrl = fileUrl,
                        UserId = userIdentity
                    };

                    using var scope = _serviceProvider.CreateScope();
                    var fileProcessorIntegrationEventService = scope.ServiceProvider.GetRequiredService<IFileProcessorIntegrationEventService>();
                    await fileProcessorIntegrationEventService.AddAndSaveEventAsync(fileSuccessfullyUploadedIntegrationEvent);
                    await fileProcessorIntegrationEventService.PublishEventsThroughEventBusAsync(fileSuccessfullyUploadedIntegrationEvent);
                }

                finally
                {
                    File.Delete(fileProcessingChannelElement.FilePath);
                }
            }
        }
    }
}
