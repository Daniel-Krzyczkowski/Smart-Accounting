using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using SmartAccounting.WebPortal.Application.Model;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartAccounting.WebPortal.Application.Infrastructure
{
    internal interface IProcessedDocumentApiService
    {
        Task<IReadOnlyCollection<Invoice>> GetAllInvoicesAsync();
    }

    internal class ProcessedDocumentApiService : ApiService, IProcessedDocumentApiService
    {
        public ProcessedDocumentApiService(HttpClient httpClient,
                                           ITokenAcquisition tokenAcquisition,
                                           IConfiguration configuration) : base(httpClient, tokenAcquisition, configuration) { }

        protected override string ApiScope => "SmartAccountingApis:ProcessedDocumentApiScope";

        public async Task<IReadOnlyCollection<Invoice>> GetAllInvoicesAsync()
        {
            await GetAndAddApiAccessTokenToAuthorizationHeaderAsync();
            GetAndAddApiSubscriptionKeyHeaderAsync();
            var allInvoices = new List<Invoice>();

            var requestURI = _configuration["SmartAccountingApis:ProcessedDocumentApiUrl"];
            var response = await _httpClient.GetAsync($"{requestURI}/document");
            if (response.IsSuccessStatusCode)
            {
                var dataAsString = await response.Content.ReadAsStringAsync();
                allInvoices = JsonSerializer.Deserialize<List<Invoice>>(dataAsString);
            }

            return allInvoices.AsReadOnly();
        }
    }
}
