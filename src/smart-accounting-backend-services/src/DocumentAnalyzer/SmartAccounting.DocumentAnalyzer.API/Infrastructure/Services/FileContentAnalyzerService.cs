using Microsoft.Extensions.Logging;
using SmartAccounting.DocumentAnalyzer.API.Application.IntegrationEvents;
using SmartAccounting.DocumentAnalyzer.API.Application.Repositories;
using SmartAccounting.DocumentAnalyzer.API.Infrastructure.IntegrationEvents;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SmartAccounting.DocumentAnalyzer.API.Infrastructure.Services
{
    internal interface IFileContentAnalyzerService
    {
        Task ScanDocumentAndExtractContentAsync(string documentUrl, string userId);
    }

    internal class FileContentAnalyzerService : IFileContentAnalyzerService
    {
        private readonly ILogger<FileContentAnalyzerService> _logger;
        private readonly IFormRecognizerInvoiceScanner _formRecognizerInvoiceScanner;
        private readonly IUserInvoiceRepository _userInvoiceRepository;
        private readonly IStorageService _storageService;
        private readonly IDocumentAnalyzerEventService _documentAnalyzerEventService;

        public FileContentAnalyzerService(ILogger<FileContentAnalyzerService> logger,
                                          IFormRecognizerInvoiceScanner formRecognizerInvoiceScanner,
                                          IUserInvoiceRepository userInvoiceRepository,
                                          IStorageService storageService,
                                          IDocumentAnalyzerEventService documentAnalyzerEventService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _formRecognizerInvoiceScanner = formRecognizerInvoiceScanner
                                                    ?? throw new ArgumentNullException(nameof(formRecognizerInvoiceScanner));
            _userInvoiceRepository = userInvoiceRepository ?? throw new ArgumentNullException(nameof(userInvoiceRepository));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _documentAnalyzerEventService = documentAnalyzerEventService ?? throw new ArgumentNullException(nameof(documentAnalyzerEventService));
        }

        public async Task ScanDocumentAndExtractContentAsync(string documentUrl, string userId)
        {
            string filename = Path.GetFileName(new Uri(documentUrl).AbsolutePath);
            var sasToken = _storageService.GenerateSasTokenForBlob(userId, filename);
            var fileUrlWithSas = $"{documentUrl}?{sasToken}";

            var userInvoice = await _formRecognizerInvoiceScanner.ScanDocumentAndGetContentAsync(fileUrlWithSas);
            userInvoice.Id = Guid.NewGuid().ToString();
            userInvoice.UserId = userId;
            userInvoice.FileUrl = documentUrl;

            await _userInvoiceRepository.AddAsync(userInvoice);

            var documentSuccessfullyAnalyzedIntegrationEvent = new DocumentSuccessfullyAnalyzedIntegrationEvent
            {
                CreationDate = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                UserId = userInvoice.UserId,
                InvoiceId = userInvoice.Id
            };

            await _documentAnalyzerEventService.AddAndSaveEventAsync(documentSuccessfullyAnalyzedIntegrationEvent);
            await _documentAnalyzerEventService.PublishEventsThroughEventBusAsync(documentSuccessfullyAnalyzedIntegrationEvent);
        }
    }
}
