using SmartAccounting.EventBus;
using System.Text.Json.Serialization;

namespace SmartAccounting.DocumentAnalyzer.API.Application.IntegrationEvents
{
    internal record FileSuccessfullyUploadedIntegrationEvent : IntegrationEvent
    {
        [JsonPropertyName("userId")]
        public string UserId { get; init; }
        [JsonPropertyName("fileUrl")]
        public string FileUrl { get; init; }

    }
}
