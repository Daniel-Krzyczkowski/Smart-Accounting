using Azure;
using Azure.Cosmos;
using Microsoft.Extensions.Logging;
using SmartAccounting.ProcessedDocument.API.Application.Model;
using SmartAccounting.ProcessedDocument.API.Application.Repositories;
using SmartAccounting.ProcessedDocument.API.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SmartAccounting.ProcessedDocument.API.Infrastructure.Repositories
{
    internal class UserInvoiceRepository : IUserInvoiceRepository
    {
        private readonly ILogger<UserInvoiceRepository> _logger;
        protected readonly ICosmosDbServiceConfiguration _cosmosDbConfiguration;
        protected readonly CosmosClient _client;

        public UserInvoiceRepository(ILogger<UserInvoiceRepository> logger,
                                      ICosmosDbServiceConfiguration cosmosDbConfiguration,
                                      CosmosClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cosmosDbConfiguration = cosmosDbConfiguration
                    ?? throw new ArgumentNullException(nameof(cosmosDbConfiguration));

            _client = client
                    ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task DeleteAsync(UserInvoice userInvoice)
        {
            try
            {
                CosmosContainer container = GetContainer();

                await container.DeleteItemAsync<UserInvoice>(userInvoice.Id, new PartitionKey(userInvoice.UserId));
            }
            catch (CosmosException ex)
            {
                _logger.LogError($"Entity with ID: {userInvoice.Id} was not removed successfully - error details: {ex.Message}");

                if (ex.Status != (int)HttpStatusCode.NotFound)
                {
                    throw;
                }
            }
        }

        public async Task<UserInvoice> GetAsync(UserInvoice userInvoice)
        {
            try
            {
                CosmosContainer container = GetContainer();

                ItemResponse<UserInvoice> entityResult = await container.ReadItemAsync<UserInvoice>(userInvoice.Id,
                                                                                                    new PartitionKey(userInvoice.UserId));
                return entityResult.Value;
            }
            catch (CosmosException ex)
            {
                _logger.LogError($"Entity with ID: {userInvoice.Id} was not retrieved successfully - error details: {ex.Message}");

                if (ex.Status != (int)HttpStatusCode.NotFound)
                {
                    throw;
                }

                return null;
            }
        }

        public async Task<IReadOnlyList<UserInvoice>> GetAllAsync(string userId)
        {
            try
            {
                CosmosContainer container = GetContainer();
                QueryDefinition queryDefinition = new QueryDefinition($"SELECT * FROM c where c.userId = @userId")
                                                      .WithParameter("@userId", userId);
                AsyncPageable<UserInvoice> queryResultSetIterator = container.GetItemQueryIterator<UserInvoice>(queryDefinition);
                List<UserInvoice> entities = new List<UserInvoice>();

                await foreach (var entity in queryResultSetIterator)
                {
                    entities.Add(entity);
                }

                return entities;

            }
            catch (CosmosException ex)
            {
                _logger.LogError($"Entities were not retrieved successfully - error details: {ex.Message}");

                if (ex.Status != (int)HttpStatusCode.NotFound)
                {
                    throw;
                }

                return null;
            }
        }

        protected CosmosContainer GetContainer()
        {
            var database = _client.GetDatabase(_cosmosDbConfiguration.DatabaseName);
            var container = database.GetContainer(_cosmosDbConfiguration.InvoiceContainerName);
            return container;
        }
    }
}
