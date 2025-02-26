using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer
{
    public class SystemAccountDAO
    {
        private readonly FunewsManagementContext _context;
        private readonly IConfiguration _configuration;

        public SystemAccountDAO(FunewsManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<SystemAccount> GetSystemAccount(string email, string password)
        {
            return await _context.SystemAccounts
                .Where(x => x.AccountEmail == email && x.AccountPassword == password)
                .FirstOrDefaultAsync();
        }

        public bool CheckAdminSystemAccount(string? accountEmail, string? password)
        {
            if (string.IsNullOrEmpty(accountEmail) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            string adminEmail = _configuration["AccountAdmin:Email"] ?? string.Empty;
            string adminPassword = _configuration["AccountAdmin:Password"] ?? string.Empty;

            return adminEmail.Equals(accountEmail) && adminPassword.Equals(password);
        }
        public async Task<IEnumerable<SystemAccount>> SystemAccounts()
        {
            return await _context.SystemAccounts.ToListAsync();
        }
        public async Task<SystemAccount> GetSystemAccountById(short Id)
        {
            return await _context.SystemAccounts
                .Where(x => x.AccountId == Id)
                .FirstOrDefaultAsync();
        }
        public async Task<SystemAccount> CreateSystemAccount(SystemAccount systemAccount)
        {
            _context.SystemAccounts.Add(systemAccount);
            await _context.SaveChangesAsync();
            return systemAccount;
        }
        public async Task<SystemAccount> UpdateSystemAccount(SystemAccount systemAccount)
        {
            _context.SystemAccounts.Update(systemAccount);
            await _context.SaveChangesAsync();
            return systemAccount;
        }
        public async Task DeleteSystemAccount(short Id)
        {
            var systemAccount = await _context.SystemAccounts
                .Where(x => x.AccountId == Id)
                .FirstOrDefaultAsync();
            if (systemAccount != null)
            {
                _context.SystemAccounts.Remove(systemAccount);
                await _context.SaveChangesAsync();
            }
            ;
        }
        public async Task<bool> SystemAccountExists(short id)
        {
            return await _context.SystemAccounts.AnyAsync(e => e.AccountId == id);
        }
        public async Task<bool> IsEmailExit(string email)
        {
            return await _context.SystemAccounts.AnyAsync(e => e.AccountEmail == email);
        }
    }
}