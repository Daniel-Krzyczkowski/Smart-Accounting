using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using SmartAccounting.ProcessedDocument.API.Configuration;
using SmartAccounting.ProcessedDocument.API.Infrastructure.Services;

namespace SmartAccounting.ProcessedDocument.API.Core.DependencyInjection
{
    internal static class StorageServiceCollectionExtensions
    {
        public static IServiceCollection AddStorageServices(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var storageConfiguration = serviceProvider.GetRequiredService<IStorageServiceConfiguration>();

            services.AddSingleton(implementationFactory =>
            {
                return new BlobServiceClient(storageConfiguration.ConnectionString);
            });

            services.AddTransient<IStorageService, StorageService>();

            return services;
        }
    }
}
