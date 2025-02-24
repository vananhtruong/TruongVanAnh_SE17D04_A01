using BusinessObjects.Models;
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
    }
}