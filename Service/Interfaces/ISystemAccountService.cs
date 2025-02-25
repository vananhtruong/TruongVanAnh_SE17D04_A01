using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Services.Interfaces
{
    public interface ISystemAccountService
    {
        Task<SystemAccount> AuthenticateUser(string email, string password);
        bool IsAdmin(string? accountEmail, string? password);
        Task<IEnumerable<SystemAccount>> SystemAccounts();
    }
}