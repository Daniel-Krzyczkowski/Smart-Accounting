using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartAccounting.EventBus.Configuration;
using SmartAccounting.Logging.Configuration;
using SmartAccounting.Notification.API.Configuration;

namespace SmartAccounting.Notification.API.Core.DependencyInjection
{
    internal static class ConfigurationServiceCollectionExtensions
    {
        public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ApplicationInsightsServiceConfiguration>(config.GetSection("ApplicationInsightsConfig"));
            services.AddSingleton<IValidateOptions<ApplicationInsightsServiceConfiguration>, ApplicationInsightsServiceConfigurationValidation>();
            var applicationInsightsServiceConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationInsightsServiceConfiguration>>().Value;
            services.AddSingleton<IApplicationInsightsServiceConfiguration>(applicationInsightsServiceConfiguration);

            services.Configure<EventBusConfiguration>(config.GetSection("ServiceBusConfig"));
            services.AddSingleton<IValidateOptions<EventBusConfiguration>, EventBusConfigurationValidation>();
            var eventBusConfigurationConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<EventBusConfiguration>>().Value;
            services.AddSingleton<IEventBusConfiguration>(eventBusConfigurationConfiguration);

            services.Configure<SqlDbServiceConfiguration>(config.GetSection("SqlDbConfig"));
            services.AddSingleton<IValidateOptions<SqlDbServiceConfiguration>, SqlDbDataServiceConfigurationValidation>();
            var sqlDbServiceConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<SqlDbServiceConfiguration>>().Value;
            services.AddSingleton<ISqlDbServiceConfiguration>(sqlDbServiceConfiguration);

            services.Configure<SignalRServiceConfiguration>(config.GetSection("AzureSignalRConfig"));
            services.AddSingleton<IValidateOptions<SignalRServiceConfiguration>, SignalRServiceConfigurationValidation>();
            var signalRServiceConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<SignalRServiceConfiguration>>().Value;
            services.AddSingleton<ISignalRServiceConfiguration>(signalRServiceConfiguration);

            services.Configure<AzureAdB2cServiceConfiguration>(config.GetSection("AzureAdB2CConfig"));
            services.AddSingleton<IValidateOptions<AzureAdB2cServiceConfiguration>, AzureAdB2cServiceConfigurationValidation>();
            var azureAdB2cServiceConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<AzureAdB2cServiceConfiguration>>().Value;
            services.AddSingleton<IAzureAdB2cServiceConfiguration>(azureAdB2cServiceConfiguration);

            return services;
        }
    }
}
