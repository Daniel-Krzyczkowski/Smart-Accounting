using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using SmartAccounting.WebPortal.Application.Model;
using System;
using System.Threading.Tasks;

namespace SmartAccounting.WebPortal.Application.Infrastructure
{
    internal class SignalRCommunicationService
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IConfiguration _config;
        private HubConnection _hubConnection;

        public event Action<DocumentProcessedNotification> OnMessageReceived;

        public SignalRCommunicationService(ITokenAcquisition tokenAcquisition,
                                           IConfiguration config)
        {
            _tokenAcquisition = tokenAcquisition ?? throw new ArgumentNullException(nameof(tokenAcquisition));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task InitializeAsync()
        {
            try
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
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("IDW10502",
                                       StringComparison.InvariantCultureIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine("SignalR connection cannot be estabilished before user is authenticated.");
                }

                else
                {
                    System.Diagnostics.Debug.WriteLine("SignalR connection cannot be estabilished - unauthorized access.");
                }
            }
        }

        public void SubscribeHubMethod()
        {
            _hubConnection.On<DocumentProcessedNotification>("direct-notification", (documentProcessedNotification) =>
            {
                OnMessageReceived?.Invoke(documentProcessedNotification);
            });
        }

        public async Task CloseConnectionAsync()
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
