using SmartAccounting.ProcessedDocument.API.Application.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartAccounting.ProcessedDocument.API.Application.Repositories
{
    internal interface IUserInvoiceRepository
    {
        Task DeleteAsync(UserInvoice userInvoice);
        Task<UserInvoice> GetAsync(UserInvoice userInvoice);
        Task<IReadOnlyList<UserInvoice>> GetAllAsync(string userId);
    }
}
