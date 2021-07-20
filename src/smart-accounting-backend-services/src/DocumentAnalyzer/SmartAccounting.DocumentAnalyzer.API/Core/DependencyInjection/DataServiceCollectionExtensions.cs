using Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using SmartAccounting.DocumentAnalyzer.API.Application.Repositories;
using SmartAccounting.DocumentAnalyzer.API.Configuration;
using SmartAccounting.DocumentAnalyzer.API.Infrastructure.Repositories;

namespace SmartAccounting.DocumentAnalyzer.API.Core.DependencyInjection
{
    internal static class DataServiceCollectionExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {

            services.AddSingleton(implementationFactory =>
            {
                var cosmoDbConfiguration = implementationFactory.GetRequiredService<ICosmosDbServiceConfiguration>();
                CosmosClient cosmosClient = new CosmosClient(cosmoDbConfiguration.ConnectionString);
                CosmosDatabase database = cosmosClient.CreateDatabaseIfNotExistsAsync(cosmoDbConfiguration.DatabaseName)
                                                       .GetAwaiter()
                                                       .GetResult();

                database.CreateContainerIfNotExistsAsync(
                    cosmoDbConfiguration.InvoiceContainerName,
                    cosmoDbConfiguration.InvoiceContainerPartitionKeyPath,
                    400)
                    .GetAwaiter()
                    .GetResult();

                return cosmosClient;
            });

            services.AddTransient<IUserInvoiceRepository, UserInvoiceRepository>();

            return services;
        }
    }
}
