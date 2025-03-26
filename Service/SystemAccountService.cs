using BusinessObjects.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Services.Interfaces;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class SystemAccountService : ISystemAccountService
    {
        private readonly ISystemAccountRepository _systemAccountRepository;
        private readonly IConfiguration _configuration;

        public SystemAccountService(ISystemAccountRepository systemAccountRepository, IConfiguration configuration)
        {
            _systemAccountRepository = systemAccountRepository;
            _configuration = configuration;
        }

        public async Task<SystemAccount> AuthenticateUser(string email, string password)
        {
            var account = await _systemAccountRepository.GetSystemAccount(email, password);
            if (account == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            return account;
        }
        public async Task<IEnumerable<SystemAccount>> SystemAccounts()
        {
            return await _systemAccountRepository.SystemAccounts();
        }
        public async Task<SystemAccount> GetSystemAccountById(short Id)
        {
            return await _systemAccountRepository.GetSystemAccountById(Id);
        }
        public async Task<SystemAccount> CreateSystemAccount(SystemAccount systemAccount)
        {
            return await _systemAccountRepository.CreateSystemAccount(systemAccount);
        }
        public async Task<SystemAccount> UpdateSystemAccount(SystemAccount systemAccount)
        {
            return await _systemAccountRepository.UpdateSystemAccount(systemAccount);
        }
        public async Task DeleteSystemAccount(short Id)
        {
            await _systemAccountRepository.DeleteSystemAccount(Id);
        }
        public async Task<bool> SystemAccountExists(short id)
        {
            return await _systemAccountRepository.SystemAccountExists(id);
        }
        public async Task<SystemAccount> GetSystemAccountByEmail(string email)
        {
            return await _systemAccountRepository.GetSystemAccountByEmail(email);
        }
        public async Task<(int role, string message)> Login(SystemAccount accountLogin, HttpContext httpContext)
        {
            string message = "";
            if (accountLogin == null)
            {
                message = "Tài khoản không đúng!";
                return (0, message);
            }

            bool isAdmin = _systemAccountRepository.CheckAdminSystemAccount(accountLogin.AccountEmail, accountLogin.AccountPassword);
            bool checkEmail = await _systemAccountRepository.IsEmailExit(accountLogin.AccountEmail);
            if (!checkEmail && !isAdmin)
            {   
                message = "Email không tồn tại!";
                return (0, message);
            }
            if (isAdmin)
            {
                var adminEmail = _configuration["AccountAdmin:Email"];

                httpContext.Session.SetString("UserEmail", adminEmail); // Dùng email từ config
                httpContext.Session.SetString("UserRole", "Admin");
                return (3, message);
            }
            else
            {
                var account = await _systemAccountRepository.GetSystemAccount(accountLogin.AccountEmail, accountLogin.AccountPassword);
                if (account == null)
                {
                    message = "Mật khẩu không đúng!";
                    return (0, message);
                }
                else
                {
                    string role = GetRole(account.AccountRole ?? 0);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, account.AccountEmail),
                        new Claim(ClaimTypes.Role, role),
                        new Claim("AccountID", account.AccountId.ToString())
                    };

                    httpContext.Session.SetString("UserEmail", account.AccountEmail);
                    httpContext.Session.SetString("UserRole", role);
                    httpContext.Session.SetString("UserName", account.AccountName);
                    return (account.AccountRole ?? 0, "");
                }
            }
        }
        private string GetRole(int role)
        {
            var roleSection = _configuration.GetSection("AccountRole");

            foreach (var child in roleSection.GetChildren())
            {
                if (int.TryParse(child.Value, out int roleValue) && roleValue == role)
                {
                    return child.Key;
                }
            }

            return "Unknown"; 
        }
        public async Task<(SystemAccount account, string role)> HandleGoogleLogin(string email, string name, HttpContext httpContext)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email không hợp lệ.");
            }

            var account = await _systemAccountRepository.GetSystemAccountByEmail(email);
            if (account == null)
            {
                Random random = new Random();
                short newAccountId;
                int maxAttempts = 10;
                int attempts = 0;

                do
                {
                    if (attempts >= maxAttempts)
                    {
                        throw new Exception("Không thể tạo AccountId duy nhất sau nhiều lần thử.");
                    }
                    newAccountId = (short)random.Next(1000, 2000);
                    attempts++;
                } while (await _systemAccountRepository.SystemAccountExists(newAccountId));

                account = new SystemAccount
                {
                    AccountId = newAccountId,
                    AccountEmail = email,
                    AccountName = name ?? email.Split('@')[0], // Sử dụng tên từ Google hoặc phần đầu của email
                    AccountRole = 2, // Role mặc định là Lecturer
                    AccountPassword = null // Không cần mật khẩu cho đăng nhập bằng Google
                };

                await _systemAccountRepository.CreateSystemAccount(account);
            }

            string role = GetRole1(account.AccountRole ?? 0);

            httpContext.Session.SetString("UserEmail", account.AccountEmail);
            httpContext.Session.SetString("UserRole", role);
            httpContext.Session.SetString("UserName", account.AccountName);

            return (account, role);
        }

        private string GetRole1(int role)
        {
            var roleSection = _configuration.GetSection("AccountRole");
            foreach (var child in roleSection.GetChildren())
            {
                if (int.TryParse(child.Value, out int roleValue) && roleValue == role)
                {
                    return child.Key;
                }
            }
            return "Unknown";
        }
        public async Task<SystemAccount> GetSystemAccount(string email, string pass)
        {
            return await _systemAccountRepository.GetSystemAccount(email, pass);
        }

    }
}