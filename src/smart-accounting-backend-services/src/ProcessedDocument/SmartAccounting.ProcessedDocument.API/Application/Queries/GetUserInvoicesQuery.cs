using MediatR;
using SmartAccounting.Common.CommonResponse;
using SmartAccounting.ProcessedDocument.API.Application.DTO;
using SmartAccounting.ProcessedDocument.API.Application.Repositories;
using SmartAccounting.ProcessedDocument.API.Infrastructure.Identity;
using SmartAccounting.ProcessedDocument.API.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAccounting.ProcessedDocument.API.Application.Queries
{
    internal class GetUserInvoicesQuery : IRequest<OperationResponse<IEnumerable<UserInvoiceDto>>>
    {
    }

    internal class GetUserInvoicesQueryHandler : IRequestHandler<GetUserInvoicesQuery, OperationResponse<IEnumerable<UserInvoiceDto>>>
    {
        private readonly IUserInvoiceRepository _userInvoiceRepository;
        private readonly IIdentityService _identityService;
        private readonly IStorageService _storageService;

        public GetUserInvoicesQueryHandler(IUserInvoiceRepository userInvoiceRepository,
                                           IIdentityService identityService,
                                           IStorageService storageService)
        {
            _userInvoiceRepository = userInvoiceRepository ?? throw new ArgumentNullException(nameof(userInvoiceRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task<OperationResponse<IEnumerable<UserInvoiceDto>>> Handle(GetUserInvoicesQuery request, CancellationToken cancellationToken)
        {
            var userId = _identityService.GetUserIdentity()
                                         .ToString();
            var allUserInvoices = await _userInvoiceRepository.GetAllAsync(userId);
            var allUserInvoicesAsDTOs = allUserInvoices.Select(userInvoice =>
            {
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

                return userInvoiceDto;
            });

            return new OperationResponse<IEnumerable<UserInvoiceDto>>
            {
                Result = allUserInvoicesAsDTOs
            };
        }
    }

}
