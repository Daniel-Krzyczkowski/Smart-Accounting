using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using SmartAccounting.Logging.Configuration;
using System.Linq;

namespace SmartAccounting.Logging
{
    public static class LoggingServicesInitializer
    {
        public static IServiceCollection AddLoggingServices(this IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                var serviceProvider = loggingBuilder.Services.BuildServiceProvider();
                var azureApplicationInsightsConfiguration = serviceProvider.GetRequiredService<IApplicationInsightsServiceConfiguration>();

                string instrumentationKey = azureApplicationInsightsConfiguration.InstrumentationKey;

                loggingBuilder.AddApplicationInsights(instrumentationKey);
                loggingBuilder.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Warning);
            });

            return services;
        }
    }


    #region AdditionalConfig

    public class CustomTelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            ISupportProperties propTelematry = (ISupportProperties)telemetry;

            var removeProps = new[] { "RequestId" };
            removeProps = removeProps.Where(prop => propTelematry.Properties.ContainsKey(prop)).ToArray();

            foreach (var prop in removeProps)
            {
                propTelematry.Properties.Remove(prop);
            }
        }
    }

    public class SuccessfulDependencyFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        public SuccessfulDependencyFilter(ITelemetryProcessor next)
        {
            Next = next;
        }

        public void Process(ITelemetry item)
        {
            if (!OKtoSend(item)) { return; }

            Next.Process(item);
        }

        private bool OKtoSend(ITelemetry item)
        {
            var dependency = item as DependencyTelemetry;
            if (dependency == null) return true;

            return dependency.Success != true;
        }
    }
    #endregion AdditionalConfig
}
