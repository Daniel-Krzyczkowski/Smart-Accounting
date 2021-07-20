using Microsoft.Extensions.Options;
using SmartAccounting.EventBus.Configuration;

namespace SmartAccounting.Notification.API.Configuration
{
    internal class EventBusConfiguration : IEventBusConfiguration
    {
        public string TopicName { get; set; }
        public string Subscription { get; set; }
        public string ListenAndSendConnectionString { get; set; }
    }

    internal class EventBusConfigurationValidation : IValidateOptions<EventBusConfiguration>
    {
        public ValidateOptionsResult Validate(string name, EventBusConfiguration options)
        {
            if (string.IsNullOrEmpty(options.ListenAndSendConnectionString))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.ListenAndSendConnectionString)} configuration parameter for the Azure Service Bus is required");
            }

            if (string.IsNullOrEmpty(options.Subscription))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.Subscription)} configuration parameter for the Azure Service Bus is required");
            }

            if (string.IsNullOrEmpty(options.TopicName))
            {
                return ValidateOptionsResult.Fail($"{nameof(options.TopicName)} configuration parameter for the Azure Service Bus is required");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
