using SmartAccounting.DocumentAnalyzer.API.Application.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartAccounting.DocumentAnalyzer.API.Application.Repositories
{
    internal interface IUserInvoiceRepository
    {
        Task<UserInvoice> AddAsync(UserInvoice userInvoice);
        Task DeleteAsync(UserInvoice userInvoice);
        Task<UserInvoice> GetAsync(UserInvoice userInvoice);
        Task<UserInvoice> UpdateAsync(UserInvoice userInvoice);
        Task<IReadOnlyList<UserInvoice>> GetAllAsync();
    }
}
