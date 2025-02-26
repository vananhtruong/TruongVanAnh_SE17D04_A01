using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Interfaces
{
    public interface ISystemAccountRepository
    {
        Task<SystemAccount> GetSystemAccount(string email, string password);
        bool GetAdminSystemAccount(string? accountEmail, string? password);
        Task<IEnumerable<SystemAccount>> SystemAccounts();
        Task<SystemAccount> GetSystemAccountById(short Id);
        Task<SystemAccount> CreateSystemAccount(SystemAccount systemAccount);
        Task<SystemAccount> UpdateSystemAccount(SystemAccount systemAccount);
        Task DeleteSystemAccount(short Id);
        Task<bool> SystemAccountExists(short id);
    }
}