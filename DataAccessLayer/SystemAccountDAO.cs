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

        public bool GetAdminSystemAccount(string? accountEmail, string? password)
        {
            string adminEmail = _configuration["AccountAdmin:Email"];
            string adminPassword = _configuration["AccountAdmin:Password"];
            return adminEmail.Equals(accountEmail) && adminPassword.Equals(password);
        }
    }
}