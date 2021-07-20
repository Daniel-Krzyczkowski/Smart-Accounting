using System;

namespace SmartAccounting.ProcessedDocument.API.Application.DTO
{
    internal class UserInvoiceDto
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string CustomerAddress { get; set; }

        public string CustomerAddressRecipient { get; set; }

        public string CustomerName { get; set; }

        public DateTime DueDate { get; set; }

        public string InvoiceId { get; set; }

        public string VendorAddress { get; set; }

        public string VendorName { get; set; }

        public float InvoiceTotal { get; set; }

        public string InvoiceFileUrl { get; set; }
    }
}
