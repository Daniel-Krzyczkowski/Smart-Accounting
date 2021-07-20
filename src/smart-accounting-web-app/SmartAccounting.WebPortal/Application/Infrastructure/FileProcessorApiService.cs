using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using SmartAccounting.WebPortal.Application.Model;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartAccounting.WebPortal.Application.Infrastructure
{
    internal interface IFileProcessorApiService
    {
        Task<HttpResponseMessage> UploadInvoiceAsync(string attachmentFileName, InvoiceUpload invoiceUpload);
    }

    internal class FileProcessorApiService : ApiService, IFileProcessorApiService
    {
        public FileProcessorApiService(HttpClient httpClient,
                                           ITokenAcquisition tokenAcquisition,
                                           IConfiguration configuration) : base(httpClient, tokenAcquisition, configuration) { }

        protected override string ApiScope => "SmartAccountingApis:FileProcessorApiScope";

        public async Task<HttpResponseMessage> UploadInvoiceAsync(string attachmentFileName, InvoiceUpload invoiceUpload)
        {
            await GetAndAddApiAccessTokenToAuthorizationHeaderAsync();
            GetAndAddApiSubscriptionKeyHeaderAsync();

            var multipartContent = new MultipartFormDataContent();
            if (invoiceUpload.Attachment != null)
            {
                multipartContent.Add(new StreamContent(invoiceUpload.Attachment), "Files", attachmentFileName);
            }
            var requestURI = _configuration["SmartAccountingApis:FileProcessorApiUrl"];
            var response = await _httpClient.PostAsync($"{requestURI}/file/upload", multipartContent);
            return response;
        }
    }
}
