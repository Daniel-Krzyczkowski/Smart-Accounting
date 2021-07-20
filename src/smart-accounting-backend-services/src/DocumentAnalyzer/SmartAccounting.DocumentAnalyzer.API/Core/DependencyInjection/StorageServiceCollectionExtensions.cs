using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using SmartAccounting.DocumentAnalyzer.API.Configuration;
using SmartAccounting.DocumentAnalyzer.API.Infrastructure.Services;

namespace SmartAccounting.DocumentAnalyzer.API.Core.DependencyInjection
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
