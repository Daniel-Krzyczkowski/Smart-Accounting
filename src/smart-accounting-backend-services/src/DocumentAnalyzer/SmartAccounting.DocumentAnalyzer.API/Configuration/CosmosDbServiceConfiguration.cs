using Microsoft.Extensions.Options;

namespace SmartAccounting.DocumentAnalyzer.API.Configuration
{
    internal interface ICosmosDbServiceConfiguration
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string InvoiceContainerName { get; set; }
        string InvoiceContainerPartitionKeyPath { get; set; }
    }

    internal class CosmosDbServiceConfiguration : ICosmosDbServiceConfiguration
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string InvoiceContainerName { get; set; }
        public string InvoiceContainerPartitionKeyPath { get; set; }
    }

    internal class CosmosDbDataServiceConfigurationValidation : IValidateOptions<CosmosDbServiceConfiguration>
    {
        public ValidateOptionsResult Validate(string name, CosmosDbServiceConfiguration options)
        {
            if (string.IsNullOrEmpty(options.ConnectionString))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.ConnectionString)} configuration parameter for the Azure Cosmos DB is required");
            }

            if (string.IsNullOrEmpty(options.InvoiceContainerName))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.InvoiceContainerName)} configuration parameter for the Azure Cosmos DB is required");
            }

            if (string.IsNullOrEmpty(options.DatabaseName))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.DatabaseName)} configuration parameter for the Azure Cosmos DB is required");
            }

            if (string.IsNullOrEmpty(options.InvoiceContainerPartitionKeyPath))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.InvoiceContainerPartitionKeyPath)} configuration parameter for the Azure Cosmos DB is required");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
