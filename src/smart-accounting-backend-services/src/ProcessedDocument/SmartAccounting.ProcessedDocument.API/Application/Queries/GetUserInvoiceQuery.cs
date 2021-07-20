using MediatR;
using SmartAccounting.Common.CommonResponse;
using SmartAccounting.ProcessedDocument.API.Application.DTO;
using SmartAccounting.ProcessedDocument.API.Application.Model;
using SmartAccounting.ProcessedDocument.API.Application.Repositories;
using SmartAccounting.ProcessedDocument.API.Infrastructure.Identity;
using SmartAccounting.ProcessedDocument.API.Infrastructure.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAccounting.ProcessedDocument.API.Application.Queries
{
    internal class GetUserInvoiceQuery : IRequest<OperationResponse<UserInvoiceDto>>
    {
        public GetUserInvoiceQuery(string invoiceId)
        {
            InvoiceId = invoiceId;
        }
        public string InvoiceId { get; }
    }

    internal class GetUserInvoiceQueryHandler : IRequestHandler<GetUserInvoiceQuery, OperationResponse<UserInvoiceDto>>
    {
        private readonly IUserInvoiceRepository _userInvoiceRepository;
        private readonly IIdentityService _identityService;
        private readonly IStorageService _storageService;

        public GetUserInvoiceQueryHandler(IUserInvoiceRepository userInvoiceRepository,
                                           IIdentityService identityService,
                                           IStorageService storageService)
        {
            _userInvoiceRepository = userInvoiceRepository ?? throw new ArgumentNullException(nameof(userInvoiceRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task<OperationResponse<UserInvoiceDto>> Handle(GetUserInvoiceQuery request, CancellationToken cancellationToken)
        {
            var userId = _identityService.GetUserIdentity()
                                         .ToString();
            var userInvoice = new UserInvoice
            {
                Id = request.InvoiceId,
                UserId = userId
            };

            userInvoice = await _userInvoiceRepository.GetAsync(userInvoice);

            var userInvoiceDto = new UserInvoiceDto
            {
                Id = userInvoice.Id,
                CustomerAddress = userInvoice.CustomerAddress,
                CustomerAddressRecipient = userInvoice.CustomerAddressRecipient,
                CustomerName = userInvoice.CustomerName,
                DueDate = userInvoice.DueDate,
                InvoiceDate = userInvoice.InvoiceDate,
                InvoiceTotal = userInvoice.InvoiceTotal,
                UserId = userId,
                VendorAddress = userInvoice.VendorAddress,
                VendorName = userInvoice.VendorName,
                InvoiceId = userInvoice.InvoiceId,
                InvoiceFileUrl = userInvoice.FileUrl
            };

            if (!string.IsNullOrEmpty(userInvoice.FileUrl))
            {
                string filename = Path.GetFileName(new Uri(userInvoice.FileUrl).AbsolutePath);
                var sasToken = _storageService.GenerateSasTokenForBlob(userId, filename);
                var fileUrlWithSas = $"{userInvoice.FileUrl}?{sasToken}";
                userInvoiceDto.InvoiceFileUrl = fileUrlWithSas;
            }

            return new OperationResponse<UserInvoiceDto>
            {
                Result = userInvoiceDto
            };
        }
    }
}
