using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface ISystemAccountRepository
    {
        Task<SystemAccount> GetSystemAccount(string email, string password);
        bool GetAdminSystemAccount(string? accountEmail, string? password);
    }
}