using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SmartAccounting.WebPortal.Application.Infrastructure
{
    internal abstract class ApiService
    {
        protected readonly HttpClient _httpClient;
        protected readonly ITokenAcquisition _tokenAcquisition;
        protected readonly IConfiguration _configuration;

        protected abstract string ApiScope { get; }

        public ApiService(HttpClient httpClient,
                                   ITokenAcquisition tokenAcquisition,
                                   IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _tokenAcquisition = tokenAcquisition ?? throw new ArgumentNullException(nameof(tokenAcquisition));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(tokenAcquisition));
        }

        protected async Task GetAndAddApiAccessTokenToAuthorizationHeaderAsync()
        {
            string[] scopes = new[] { _configuration[ApiScope] };
            string accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        protected void GetAndAddApiSubscriptionKeyHeaderAsync()
        {
            var subscriptionKey = _configuration["SmartAccountingApis:SubscriptionKey"];
            _httpClient.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Key");
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
        }
    }
}
