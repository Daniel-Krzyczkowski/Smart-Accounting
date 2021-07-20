using System;
using System.Text.Json.Serialization;

namespace SmartAccounting.DocumentAnalyzer.API.Application.Model
{
    internal class UserInvoice
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("invoiceDate")]
        public DateTime InvoiceDate { get; set; }

        [JsonPropertyName("customerAddress")]
        public string CustomerAddress { get; set; }

        [JsonPropertyName("customerAddressRecipient")]
        public string CustomerAddressRecipient { get; set; }

        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; }

        [JsonPropertyName("dueDate")]
        public DateTime DueDate { get; set; }

        [JsonPropertyName("invoiceId")]
        public string InvoiceId { get; set; }

        [JsonPropertyName("vendorAddress")]
        public string VendorAddress { get; set; }

        [JsonPropertyName("vendorName")]
        public string VendorName { get; set; }

        [JsonPropertyName("invoiceTotal")]
        public float InvoiceTotal { get; set; }

        [JsonPropertyName("fileUrl")]
        public string FileUrl { get; set; }
    }
}
