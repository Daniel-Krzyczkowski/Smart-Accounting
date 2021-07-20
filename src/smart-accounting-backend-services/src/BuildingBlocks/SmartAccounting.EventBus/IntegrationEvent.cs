using System;
using System.Text.Json.Serialization;

namespace SmartAccounting.EventBus
{
    public abstract record IntegrationEvent
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; init; }
    }
}
