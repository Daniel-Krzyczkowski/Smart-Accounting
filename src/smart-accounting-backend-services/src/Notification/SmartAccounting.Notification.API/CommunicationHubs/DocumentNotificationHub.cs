using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SmartAccounting.Notification.API.Application.Model;
using System.Threading.Tasks;

namespace SmartAccounting.Notification.API.CommunicationHubs
{
    [Authorize(Policy = "AccessAsUser")]
    internal class DocumentNotificationHub : Hub
    {
        [HubMethodName(HubMethodName)]
        public async Task SendDirectMessageToUserAsync(DocumentProcessedNotification documentProcessedNotification)
        {
            await Clients.User(documentProcessedNotification.UserId).SendAsync(HubMethodName, documentProcessedNotification);
        }


        public const string HubMethodName = "direct-notification";
        public const string HubName = "DocumentNotificationHub";
    }
}
