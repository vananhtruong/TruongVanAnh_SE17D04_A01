using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
namespace Services.Interfaces
{
    public interface ISystemAccountService
    {
        Task<SystemAccount> AuthenticateUser(string email, string password);
        Task<IEnumerable<SystemAccount>> SystemAccounts();
        Task<SystemAccount> GetSystemAccountById(short Id);
        Task<SystemAccount> CreateSystemAccount(SystemAccount systemAccount);
        Task<SystemAccount> UpdateSystemAccount(SystemAccount systemAccount);
        Task DeleteSystemAccount(short Id);
        Task<bool> SystemAccountExists(short id);
        Task<(int role, string message)> Login(SystemAccount accountLogin, HttpContext httpContext);
        Task<SystemAccount> GetSystemAccountByEmail(string email);
    }
}