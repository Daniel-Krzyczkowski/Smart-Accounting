using Microsoft.Extensions.Logging;
using SmartAccounting.EventBus;
using SmartAccounting.EventBus.Interfaces;
using SmartAccounting.EventLog;
using System;
using System.Threading.Tasks;

namespace SmartAccounting.DocumentAnalyzer.API.Infrastructure.IntegrationEvents
{
    internal interface IDocumentAnalyzerEventService
    {
        Task PublishEventsThroughEventBusAsync(IntegrationEvent @event);
        Task AddAndSaveEventAsync(IntegrationEvent @event);
    }

    public class DocumentAnalyzerEventService : IDocumentAnalyzerEventService
    {
        private readonly ILogger<DocumentAnalyzerEventService> _logger;
        private readonly IEventBus _eventBus;
        private readonly IEventLogService _eventLogService;

        public DocumentAnalyzerEventService(ILogger<DocumentAnalyzerEventService> logger,
                                                    IEventBus eventBus,
                                                    IEventLogService eventLogService)
        {
            _logger = logger;
            _eventBus = eventBus;
            _eventLogService = eventLogService;
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent @event)
        {
            await _eventLogService.SaveEventAsync(@event);
        }

        public async Task PublishEventsThroughEventBusAsync(IntegrationEvent @event)
        {
            try
            {
                await _eventLogService.MarkEventAsInProgressAsync(@event.Id);
                await _eventBus.PublishAsync(@event);
                await _eventLogService.MarkEventAsPublishedAsync(@event.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Publishing integration event failed: '{IntegrationEventId}'", @event.Id);

                await _eventLogService.MarkEventAsFailedAsync(@event.Id);
            }
        }
    }
}
