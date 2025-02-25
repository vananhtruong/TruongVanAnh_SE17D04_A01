using BusinessObjects.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class SystemAccountRepository : ISystemAccountRepository
    {
        private readonly SystemAccountDAO _systemAccountDAO;

        public SystemAccountRepository(SystemAccountDAO systemAccountDAO)
        {
            _systemAccountDAO = systemAccountDAO;
        }

        public async Task<SystemAccount> GetSystemAccount(string email, string password)
        {
            return await _systemAccountDAO.GetSystemAccount(email, password);
        }

        public bool GetAdminSystemAccount(string? accountEmail, string? password)
        {
            return _systemAccountDAO.GetAdminSystemAccount(accountEmail, password);
        }
        public async Task<IEnumerable<SystemAccount>> SystemAccounts()
        {
            return await _systemAccountDAO.SystemAccounts();
        }
    }
}