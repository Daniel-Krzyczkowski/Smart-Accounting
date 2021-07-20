using Microsoft.Extensions.Options;

namespace SmartAccounting.Notification.API.Configuration
{
    internal interface ISignalRServiceConfiguration
    {
        string ConnectionString { get; set; }
    }

    internal class SignalRServiceConfiguration : ISignalRServiceConfiguration
    {
        public string ConnectionString { get; set; }
    }

    internal class SignalRServiceConfigurationValidation : IValidateOptions<SignalRServiceConfiguration>
    {
        public ValidateOptionsResult Validate(string name, SignalRServiceConfiguration options)
        {
            if (string.IsNullOrEmpty(options.ConnectionString))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.ConnectionString)} configuration parameter for the Azure SignalR Service is required");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
