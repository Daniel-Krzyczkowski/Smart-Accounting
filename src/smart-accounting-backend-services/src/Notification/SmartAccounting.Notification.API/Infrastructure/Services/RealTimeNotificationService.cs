using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Logging;
using SmartAccounting.Notification.API.Application.Model;
using SmartAccounting.Notification.API.CommunicationHubs;
using SmartAccounting.Notification.API.Configuration;
using System;
using System.Threading.Tasks;

namespace SmartAccounting.Notification.API.Infrastructure.Services
{
    internal interface IRealTimeNotificationService
    {
        Task SendNotificationAsync(DocumentProcessedNotification documentProcessedNotification);
    }

    internal class RealTimeNotificationService : IRealTimeNotificationService
    {
        private readonly ILogger<RealTimeNotificationService> _logger;
        private readonly ISignalRServiceConfiguration _signalRServiceConfiguration;

        public RealTimeNotificationService(ILogger<RealTimeNotificationService> logger,
                                           ISignalRServiceConfiguration signalRServiceConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _signalRServiceConfiguration = signalRServiceConfiguration
                                                    ?? throw new ArgumentNullException(nameof(signalRServiceConfiguration));
        }

        public async Task SendNotificationAsync(DocumentProcessedNotification documentProcessedNotification)
        {
            using var hubServiceManager = new ServiceManagerBuilder()
                                                            .WithOptions(option =>
            {
                option.ConnectionString = _signalRServiceConfiguration.ConnectionString;
                option.ServiceTransportType = ServiceTransportType.Persistent;
            }).Build();

            var hubContext = await hubServiceManager.CreateHubContextAsync(DocumentNotificationHub.HubName);
            await hubContext
                    .Clients
                    .User(documentProcessedNotification.UserId)
                    .SendAsync(DocumentNotificationHub.HubMethodName, documentProcessedNotification);

            _logger.LogInformation($"Real time notification sent using SignalR Service to the user with ID: {documentProcessedNotification.UserId}");
            _logger.LogInformation($"Real time notification sent using SignalR Service with invoice ID: {documentProcessedNotification.InvoiceId}");
        }
    }
}
