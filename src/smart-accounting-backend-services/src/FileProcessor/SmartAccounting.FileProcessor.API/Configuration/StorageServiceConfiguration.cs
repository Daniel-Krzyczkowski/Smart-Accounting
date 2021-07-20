using Microsoft.Extensions.Options;

namespace SmartAccounting.FileProcessor.API.Configuration
{
    internal interface IStorageServiceConfiguration
    {
        string Key { get; set; }
        string AccountName { get; set; }
        string ConnectionString { get; set; }
    }

    internal class StorageServiceConfiguration : IStorageServiceConfiguration
    {
        public string Key { get; set; }
        public string AccountName { get; set; }
        public string ConnectionString { get; set; }
    }

    internal class StorageServiceConfigurationValidation : IValidateOptions<StorageServiceConfiguration>
    {
        public ValidateOptionsResult Validate(string name, StorageServiceConfiguration options)
        {
            if (string.IsNullOrEmpty(options.Key))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Key)} configuration parameter for the Azure Storage Account is required");
            }

            if (string.IsNullOrEmpty(options.AccountName))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.AccountName)} configuration parameter for the Azure Storage Account is required");
            }

            if (string.IsNullOrEmpty(options.ConnectionString))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.ConnectionString)} configuration parameter for the Azure Storage Account is required");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
