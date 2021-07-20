using Microsoft.Extensions.Logging;
using SmartAccounting.DocumentAnalyzer.API.Infrastructure.Services;
using SmartAccounting.EventBus.Interfaces;
using System;
using System.Threading.Tasks;

namespace SmartAccounting.DocumentAnalyzer.API.Application.IntegrationEvents.EventHandlers
{
    internal class FileSuccessfullyUploadedEventHandler : IIntegrationEventHandler<FileSuccessfullyUploadedIntegrationEvent>
    {
        private readonly ILogger<FileSuccessfullyUploadedEventHandler> _logger;
        private readonly IFileContentAnalyzerService _fileContentAnalyzerService;

        public FileSuccessfullyUploadedEventHandler(ILogger<FileSuccessfullyUploadedEventHandler> logger,
                                                    IFileContentAnalyzerService fileContentAnalyzerService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileContentAnalyzerService = fileContentAnalyzerService
                                                ?? throw new ArgumentNullException(nameof(fileContentAnalyzerService));
        }

        public async Task HandleAsync(FileSuccessfullyUploadedIntegrationEvent @event)
        {
            if (!string.IsNullOrEmpty(@event.FileUrl)
                                            && !string.IsNullOrEmpty(@event.UserId))
            {
                await _fileContentAnalyzerService.ScanDocumentAndExtractContentAsync(@event.FileUrl, @event.UserId);
            }
        }
    }
}
