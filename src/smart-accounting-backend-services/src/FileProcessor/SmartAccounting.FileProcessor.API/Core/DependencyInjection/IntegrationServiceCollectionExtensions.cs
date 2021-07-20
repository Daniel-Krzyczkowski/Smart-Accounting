using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartAccounting.EventBus;
using SmartAccounting.EventBus.Configuration;
using SmartAccounting.EventBus.Interfaces;
using SmartAccounting.EventLog;
using SmartAccounting.FileProcessor.API.Configuration;
using SmartAccounting.FileProcessor.API.Infrastructure.IntegrationEvents;
using System;
using System.Reflection;

namespace SmartAccounting.FileProcessor.API.Core.DependencyInjection
{
    internal static class IntegrationServiceCollectionExtensions
    {
        public static IServiceCollection AddIntegrationServices(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var eventBusConfiguration = serviceProvider.GetRequiredService<IEventBusConfiguration>();
            var sqlDbServiceConfiguration = serviceProvider.GetRequiredService<ISqlDbServiceConfiguration>();

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

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
            services.AddTransient<IFileProcessorIntegrationEventService, FileProcessorIntegrationEventService>();


            return services;
        }
    }
}
