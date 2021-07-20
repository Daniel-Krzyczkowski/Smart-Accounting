using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using SmartAccounting.WebPortal.Application.Model;
using System.Threading.Tasks;

namespace SmartAccounting.WebPortal.Application.Infrastructure
{
    internal class SignalRCommunicationService
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IConfiguration _config;
        private HubConnection _hubConnection;

        public async Task InitializeSignalRConnectionAsync()
        {
            var hubUrl = _config["SmartAccountingApis:NotificationHubUrl"];

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        string[] scopes = new[] { _config["SmartAccountingApis:NotificationApiScope"] };
                        string accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                        return accessToken;
                    };
                })
                .Build();
            await _hubConnection.StartAsync();

            _hubConnection.On<DocumentProcessedNotification>("direct-notification", (documentProcessedNotification) =>
            {
                // Handle message...
            });
        }
    }
}
