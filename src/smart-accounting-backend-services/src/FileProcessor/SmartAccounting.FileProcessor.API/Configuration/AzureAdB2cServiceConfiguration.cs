using Microsoft.Extensions.Options;

namespace SmartAccounting.FileProcessor.API.Configuration
{
    internal interface IAzureAdB2cServiceConfiguration
    {
        string Instance { get; set; }
        string ClientId { get; set; }
        string Domain { get; set; }
        string SignUpSignInPolicyId { get; set; }
    }

    internal class AzureAdB2cServiceConfiguration : IAzureAdB2cServiceConfiguration
    {
        public string Instance { get; set; }
        public string ClientId { get; set; }
        public string Domain { get; set; }
        public string SignUpSignInPolicyId { get; set; }
    }

    internal class AzureAdB2cServiceConfigurationValidation : IValidateOptions<AzureAdB2cServiceConfiguration>
    {
        public ValidateOptionsResult Validate(string name, AzureAdB2cServiceConfiguration options)
        {
            if (string.IsNullOrEmpty(options.Instance))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Instance)} configuration parameter for the Azure AD B2C is required");
            }

            if (string.IsNullOrEmpty(options.ClientId))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.ClientId)} configuration parameter for the Azure AD B2C is required");
            }

            if (string.IsNullOrEmpty(options.Domain))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Domain)} configuration parameter for the Azure AD B2C is required");
            }

            if (string.IsNullOrEmpty(options.SignUpSignInPolicyId))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.SignUpSignInPolicyId)} configuration parameter for the Azure AD B2C is required");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
