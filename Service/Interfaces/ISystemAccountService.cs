using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Services.Interfaces
{
    public interface ISystemAccountService
    {
        Task<SystemAccount> AuthenticateUser(string email, string password);
        bool IsAdmin(string? accountEmail, string? password);
        Task<IEnumerable<SystemAccount>> SystemAccounts();
        Task<SystemAccount> GetSystemAccountById(short Id);
        Task<SystemAccount> CreateSystemAccount(SystemAccount systemAccount);
        Task<SystemAccount> UpdateSystemAccount(SystemAccount systemAccount);
        Task DeleteSystemAccount(short Id);
        Task<bool> SystemAccountExists(short id);
    }
}