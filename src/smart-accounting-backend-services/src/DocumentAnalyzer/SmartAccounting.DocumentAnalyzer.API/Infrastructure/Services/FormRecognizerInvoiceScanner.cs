using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;
using Microsoft.Extensions.Logging;
using SmartAccounting.DocumentAnalyzer.API.Application.Model;
using System;
using System.Threading.Tasks;

namespace SmartAccounting.DocumentAnalyzer.API.Infrastructure.Services
{
    internal interface IFormRecognizerInvoiceScanner
    {
        Task<UserInvoice> ScanDocumentAndGetContentAsync(string documentUrl);
    }

    internal class FormRecognizerInvoiceScanner : IFormRecognizerInvoiceScanner
    {
        private readonly ILogger<FormRecognizerInvoiceScanner> _logger;
        private readonly FormRecognizerClient _formRecognizerClient;

        public FormRecognizerInvoiceScanner(ILogger<FormRecognizerInvoiceScanner> logger,
                                          FormRecognizerClient formRecognizerClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _formRecognizerClient = formRecognizerClient ?? throw new ArgumentNullException(nameof(formRecognizerClient));
        }

        public async Task<UserInvoice> ScanDocumentAndGetContentAsync(string documentUrl)
        {
            var userInvoice = new UserInvoice();
            var scanResponse = await _formRecognizerClient.StartRecognizeInvoicesFromUri(new Uri(documentUrl))
                                                  .WaitForCompletionAsync();

            foreach (var page in scanResponse.Value)
            {

                FormField invoiceIdField;
                if (page.Fields.TryGetValue("InvoiceId", out invoiceIdField))
                {
                    if (invoiceIdField.Value.ValueType == FieldValueType.String)
                    {
                        string invoiceId = invoiceIdField.Value.AsString();
                        userInvoice.InvoiceId = invoiceId;
                    }
                }

                FormField invoiceDateField;
                if (page.Fields.TryGetValue("InvoiceDate", out invoiceDateField))
                {
                    if (invoiceDateField.Value.ValueType == FieldValueType.Date)
                    {
                        DateTime invoiceDate = invoiceDateField.Value.AsDate();
                        userInvoice.InvoiceDate = invoiceDate;
                    }
                }

                FormField dueDateField;
                if (page.Fields.TryGetValue("DueDate", out dueDateField))
                {
                    if (dueDateField.Value.ValueType == FieldValueType.Date)
                    {
                        DateTime dueDate = dueDateField.Value.AsDate();
                        userInvoice.DueDate = dueDate;
                    }
                }

                FormField vendorNameField;
                if (page.Fields.TryGetValue("VendorName", out vendorNameField))
                {
                    if (vendorNameField.Value.ValueType == FieldValueType.String)
                    {
                        string vendorName = vendorNameField.Value.AsString();
                        userInvoice.VendorName = vendorName;
                    }
                }

                FormField vendorAddressField;
                if (page.Fields.TryGetValue("VendorAddress", out vendorAddressField))
                {
                    if (vendorAddressField.Value.ValueType == FieldValueType.String)
                    {
                        string vendorAddress = vendorAddressField.Value.AsString();
                        userInvoice.VendorAddress = vendorAddress;
                    }
                }

                FormField customerNameField;
                if (page.Fields.TryGetValue("CustomerName", out customerNameField))
                {
                    if (customerNameField.Value.ValueType == FieldValueType.String)
                    {
                        string customerName = customerNameField.Value.AsString();
                        userInvoice.CustomerName = customerName;
                    }
                }

                FormField customerAddressField;
                if (page.Fields.TryGetValue("CustomerAddress", out customerAddressField))
                {
                    if (customerAddressField.Value.ValueType == FieldValueType.String)
                    {
                        string customerAddress = customerAddressField.Value.AsString();
                        userInvoice.CustomerAddress = customerAddress;
                    }
                }

                FormField customerAddressRecipientField;
                if (page.Fields.TryGetValue("CustomerAddressRecipient", out customerAddressRecipientField))
                {
                    if (customerAddressRecipientField.Value.ValueType == FieldValueType.String)
                    {
                        string customerAddressRecipient = customerAddressRecipientField.Value.AsString();
                        userInvoice.CustomerAddressRecipient = customerAddressRecipient;
                    }
                }

                FormField invoiceTotalField;
                if (page.Fields.TryGetValue("InvoiceTotal", out invoiceTotalField))
                {
                    if (invoiceTotalField.Value.ValueType == FieldValueType.Float)
                    {
                        float invoiceTotal = invoiceTotalField.Value.AsFloat();
                        userInvoice.InvoiceTotal = invoiceTotal;
                    }
                }
            }

            return userInvoice;
        }
    }
}
