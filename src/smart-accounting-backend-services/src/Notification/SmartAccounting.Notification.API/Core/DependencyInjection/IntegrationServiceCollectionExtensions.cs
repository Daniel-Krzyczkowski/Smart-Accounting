using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartAccounting.EventBus;
using SmartAccounting.EventBus.Configuration;
using SmartAccounting.EventBus.Interfaces;
using SmartAccounting.EventLog;
using SmartAccounting.Notification.API.Application.IntegrationEvents;
using SmartAccounting.Notification.API.Application.IntegrationEvents.EventHandlers;
using SmartAccounting.Notification.API.Configuration;
using SmartAccounting.Notification.API.Infrastructure.Services;
using System;
using System.Reflection;

namespace SmartAccounting.Notification.API.Core.DependencyInjection
{
    internal static class IntegrationServiceCollectionExtensions
    {
        public static IServiceCollection AddIntegrationServices(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var eventBusConfiguration = serviceProvider.GetRequiredService<IEventBusConfiguration>();
            var sqlDbServiceConfiguration = serviceProvider.GetRequiredService<ISqlDbServiceConfiguration>();

            services.AddTransient<IRealTimeNotificationService, RealTimeNotificationService>();
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddTransient<IIntegrationEventHandler<DocumentSuccessfullyAnalyzedIntegrationEvent>, DocumentSuccessfullyAnalyzedEventHandler>();

            services.AddSingleton(implementationFactory =>
            {
                var serviceBusClient = new ServiceBusClient(eventBusConfiguration.ListenAndSendConnectionString);
                return serviceBusClient;
            });

            services.AddSingleton(implementationFactory =>
            {
                var serviceBusAdministrationClient = new ServiceBusAdministrationClient(eventBusConfiguration
                                                                                        .ListenAndSendConnectionString);
                return serviceBusAdministrationClient;
            });

            services.AddSingleton(implementationFactory =>
            {
                var serviceBusClient = implementationFactory.GetRequiredService<ServiceBusClient>();
                var serviceBusReceiver = serviceBusClient.CreateProcessor(eventBusConfiguration.TopicName,
                                                                          eventBusConfiguration.Subscription,
                                                                          new ServiceBusProcessorOptions());

                return serviceBusReceiver;
            });

            services.AddSingleton<IEventBus, AzureServiceBusEventBus>();
            serviceProvider = services.BuildServiceProvider();

            var azureServiceBusEventBus = serviceProvider.GetRequiredService<IEventBus>();
            azureServiceBusEventBus.SetupAsync()
                                   .GetAwaiter()
                                   .GetResult();

            azureServiceBusEventBus.SubscribeAsync<DocumentSuccessfullyAnalyzedIntegrationEvent,
                        IIntegrationEventHandler<DocumentSuccessfullyAnalyzedIntegrationEvent>>()
                        .GetAwaiter().GetResult();


            services.AddDbContext<EventLogContext>(options =>
            {
                options.UseSqlServer(sqlDbServiceConfiguration.ConnectionString,
                                     sqlServerOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                         sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                                     });
            });



            services.AddTransient<IEventLogService, EventLogService>();


            return services;
        }
    }
}
