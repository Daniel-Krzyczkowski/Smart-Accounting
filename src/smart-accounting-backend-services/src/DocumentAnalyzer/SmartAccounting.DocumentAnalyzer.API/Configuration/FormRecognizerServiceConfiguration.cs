using Microsoft.Extensions.Options;

namespace SmartAccounting.DocumentAnalyzer.API.Configuration
{
    internal interface IFormRecognizerServiceConfiguration
    {
        string EndpointUrl { get; set; }
        string Key { get; set; }
    }

    internal class FormRecognizerServiceConfiguration : IFormRecognizerServiceConfiguration
    {
        public string EndpointUrl { get; set; }
        public string Key { get; set; }
    }

    internal class FormRecognizerServiceConfigurationValidation : IValidateOptions<FormRecognizerServiceConfiguration>
    {
        public ValidateOptionsResult Validate(string name, FormRecognizerServiceConfiguration options)
        {
            if (string.IsNullOrEmpty(options.EndpointUrl))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.EndpointUrl)} configuration parameter for the Form Recognizer is required");
            }

            if (string.IsNullOrEmpty(options.Key))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Key)} configuration parameter for the Form Recognizer is required");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
