using Microsoft.Extensions.Logging;
using SmartAccounting.EventBus.Interfaces;
using SmartAccounting.Notification.API.Application.Model;
using SmartAccounting.Notification.API.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace SmartAccounting.Notification.API.Application.IntegrationEvents.EventHandlers
{
    internal class DocumentSuccessfullyAnalyzedEventHandler : IIntegrationEventHandler<DocumentSuccessfullyAnalyzedIntegrationEvent>
    {
        private readonly ILogger<DocumentSuccessfullyAnalyzedEventHandler> _logger;
        private readonly IRealTimeNotificationService _realTimeNotificationService;

        public DocumentSuccessfullyAnalyzedEventHandler(ILogger<DocumentSuccessfullyAnalyzedEventHandler> logger,
                                                        IRealTimeNotificationService realTimeNotificationService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _realTimeNotificationService = realTimeNotificationService
                                                            ?? throw new ArgumentNullException(nameof(realTimeNotificationService));
        }

        public async Task HandleAsync(DocumentSuccessfullyAnalyzedIntegrationEvent @event)
        {
            if (!string.IsNullOrEmpty(@event.UserId)
                                                      && !string.IsNullOrEmpty(@event.InvoiceId))
            {
                var documentProcessedNotification = new DocumentProcessedNotification
                {
                    InvoiceId = @event.InvoiceId,
                    UserId = @event.UserId
                };

                await _realTimeNotificationService.SendNotificationAsync(documentProcessedNotification);
            }
        }
    }
}
