using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartAccounting.Logging.Configuration;
using SmartAccounting.ProcessedDocument.API.Configuration;

namespace SmartAccounting.ProcessedDocument.API.Core.DependencyInjection
{
    internal static class ConfigurationServiceCollectionExtensions
    {
        public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<SwaggerConfiguration>(config.GetSection("SwaggerConfig"));
            services.AddSingleton<IValidateOptions<SwaggerConfiguration>, SwaggerConfigurationValidation>();
            var swaggerConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<SwaggerConfiguration>>().Value;
            services.AddSingleton<ISwaggerConfiguration>(swaggerConfiguration);

            services.Configure<ApplicationInsightsServiceConfiguration>(config.GetSection("ApplicationInsightsConfig"));
            services.AddSingleton<IValidateOptions<ApplicationInsightsServiceConfiguration>, ApplicationInsightsServiceConfigurationValidation>();
            var applicationInsightsServiceConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationInsightsServiceConfiguration>>().Value;
            services.AddSingleton<IApplicationInsightsServiceConfiguration>(applicationInsightsServiceConfiguration);

            services.Configure<StorageServiceConfiguration>(config.GetSection("BlobStorageConfig"));
            services.AddSingleton<IValidateOptions<StorageServiceConfiguration>, StorageServiceConfigurationValidation>();
            var storageServiceConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<StorageServiceConfiguration>>().Value;
            services.AddSingleton<IStorageServiceConfiguration>(storageServiceConfiguration);

            services.Configure<CosmosDbServiceConfiguration>(config.GetSection("CosmosDbConfig"));
            services.AddSingleton<IValidateOptions<CosmosDbServiceConfiguration>, CosmosDbDataServiceConfigurationValidation>();
            var cosmosDbDataServiceConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<CosmosDbServiceConfiguration>>().Value;
            services.AddSingleton<ICosmosDbServiceConfiguration>(cosmosDbDataServiceConfiguration);

            return services;
        }
    }
}
