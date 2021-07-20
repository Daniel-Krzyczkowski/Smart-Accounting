namespace SmartAccounting.Notification.API.Application.Model
{
    internal class DocumentProcessedNotification
    {
        public string UserId { get; set; }
        public string InvoiceId { get; set; }
    }
}
