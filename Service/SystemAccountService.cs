﻿using BusinessObjects.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Services.Interfaces;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Data;

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
            // Logic xác thực người dùng (ví dụ: kiểm tra email và password)
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
                    return (account.AccountRole ?? 0, "");
                }
            }
        }
        //private async Task SignInUser(HttpContext httpContext, List<Claim> claims)
        //{
        //    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //    var authProperties = new AuthenticationProperties { IsPersistent = true }; // Cookie sẽ tồn tại sau khi đóng trình duyệt

        //    await httpContext.SignInAsync(
        //        CookieAuthenticationDefaults.AuthenticationScheme,
        //        new ClaimsPrincipal(claimsIdentity),
        //        authProperties
        //    );
        //}
        private string GetRole(int role)
        {
            var roleSection = _configuration.GetSection("AccountRole");

            // Lấy tất cả các cặp key-value trong section "AccountRole"
            foreach (var child in roleSection.GetChildren())
            {
                // Lấy giá trị (value) dưới dạng int từ config
                if (int.TryParse(child.Value, out int roleValue) && roleValue == role)
                {
                    return child.Key; // Trả về tên vai trò (key)
                }
            }

            return "Unknown"; // Trả về "Unknown" nếu không tìm thấy
        }
    }
}