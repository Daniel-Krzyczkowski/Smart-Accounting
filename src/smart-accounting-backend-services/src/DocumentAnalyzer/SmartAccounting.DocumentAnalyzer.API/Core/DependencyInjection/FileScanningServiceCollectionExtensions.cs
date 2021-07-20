using Azure;
using Azure.AI.FormRecognizer;
using Microsoft.Extensions.DependencyInjection;
using SmartAccounting.DocumentAnalyzer.API.Configuration;
using SmartAccounting.DocumentAnalyzer.API.Infrastructure.Services;
using System;

namespace SmartAccounting.DocumentAnalyzer.API.Core.DependencyInjection
{
    internal static class FileScanningServiceCollectionExtensions
    {
        public static IServiceCollection AddFileContentScanningServices(this IServiceCollection services)
        {
            services.AddSingleton(implementationFactory =>
            {
                var formRecognizerServiceConfiguration = implementationFactory.GetRequiredService<IFormRecognizerServiceConfiguration>();
                var formRecognizerClient = new FormRecognizerClient(new Uri(formRecognizerServiceConfiguration.EndpointUrl),
                                                                    new AzureKeyCredential(formRecognizerServiceConfiguration.Key));
                return formRecognizerClient;
            });

            services.AddTransient<IFormRecognizerInvoiceScanner, FormRecognizerInvoiceScanner>();
            services.AddTransient<IFileContentAnalyzerService, FileContentAnalyzerService>();

            return services;
        }
    }
}
