using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class SystemAccountService : ISystemAccountService
    {
        private readonly ISystemAccountRepository _systemAccountRepository;

        public SystemAccountService(ISystemAccountRepository systemAccountRepository)
        {
            _systemAccountRepository = systemAccountRepository;
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

        public bool IsAdmin(string? accountEmail, string? password)
        {
            return _systemAccountRepository.GetAdminSystemAccount(accountEmail, password);
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
    }
}