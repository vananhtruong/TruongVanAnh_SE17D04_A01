using BusinessObjects.Models;

namespace Services.Interfaces
{
    public interface ISystemAccountService
    {
        Task<SystemAccount> AuthenticateUser(string email, string password);
        bool IsAdmin(string? accountEmail, string? password);
    }
}