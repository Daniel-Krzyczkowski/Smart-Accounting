using System.IO;

namespace SmartAccounting.WebPortal.Application.Model
{
    public class InvoiceUpload
    {
        public Stream Attachment { get; set; }
        public string AttachmentFileName { get; set; }
    }
}
