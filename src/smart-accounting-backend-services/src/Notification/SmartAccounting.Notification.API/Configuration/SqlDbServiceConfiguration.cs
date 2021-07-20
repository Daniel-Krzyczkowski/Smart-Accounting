using Microsoft.Extensions.Options;

namespace SmartAccounting.Notification.API.Configuration
{
    internal interface ISqlDbServiceConfiguration
    {
        string ConnectionString { get; set; }
    }

    internal class SqlDbServiceConfiguration : ISqlDbServiceConfiguration
    {
        public string ConnectionString { get; set; }
    }

    internal class SqlDbDataServiceConfigurationValidation : IValidateOptions<SqlDbServiceConfiguration>
    {
        public ValidateOptionsResult Validate(string name, SqlDbServiceConfiguration options)
        {
            if (string.IsNullOrEmpty(options.ConnectionString))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.ConnectionString)} configuration parameter for the Azure SQL is required");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
