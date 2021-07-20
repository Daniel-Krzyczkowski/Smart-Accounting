using System;
using System.Text.Json.Serialization;

namespace SmartAccounting.FileProcessor.API.Application.Model
{
    public class UserInvoice
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public string FileUrl { get; set; }
    }
}
