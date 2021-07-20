using Microsoft.Extensions.DependencyInjection;
using SmartAccounting.Notification.API.Configuration;

namespace SmartAccounting.Notification.API.Core.DependencyInjection
{
    internal static class RealTimeNotificationServiceCollectionExtensions
    {
        public static IServiceCollection AddRealTimeNotificationServices(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var signalRServiceConfiguration = serviceProvider.GetRequiredService<ISignalRServiceConfiguration>();

            services.AddSignalR()
                    .AddAzureSignalR(signalRServiceConfiguration.ConnectionString);

            return services;
        }
    }
}
