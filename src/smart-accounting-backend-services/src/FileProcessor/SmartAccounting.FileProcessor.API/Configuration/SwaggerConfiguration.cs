using Microsoft.Extensions.Options;

namespace SmartAccounting.FileProcessor.API.Configuration
{
    internal interface ISwaggerConfiguration
    {
        string Root { get; set; }
        string ServiceName { get; set; }
        string ServiceVersion { get; set; }
    }

    internal class SwaggerConfiguration : ISwaggerConfiguration
    {
        public string Root { get; set; }
        public string ServiceName { get; set; }
        public string ServiceVersion { get; set; }
    }

    internal class SwaggerConfigurationValidation : IValidateOptions<SwaggerConfiguration>
    {
        public ValidateOptionsResult Validate(string name, SwaggerConfiguration options)
        {
            if (string.IsNullOrEmpty(options.Root))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Root)} configuration parameter for the Swagger is required");
            }

            if (string.IsNullOrEmpty(options.ServiceName))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.ServiceName)} configuration parameter for the Swagger is required");
            }

            if (string.IsNullOrEmpty(options.ServiceVersion))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.ServiceVersion)} configuration parameter for the Swagger is required");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
