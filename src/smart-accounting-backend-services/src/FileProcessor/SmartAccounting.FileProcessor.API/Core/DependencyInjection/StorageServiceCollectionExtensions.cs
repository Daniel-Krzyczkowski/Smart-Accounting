using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using SmartAccounting.FileProcessor.API.Configuration;
using SmartAccounting.FileProcessor.API.Infrastructure.Services;

namespace SmartAccounting.FileProcessor.API.Core.DependencyInjection
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
