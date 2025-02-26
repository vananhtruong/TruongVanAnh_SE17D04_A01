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

        public bool CheckAdminSystemAccount(string? accountEmail, string? password)
        {
            return _systemAccountDAO.CheckAdminSystemAccount(accountEmail, password);
        }
        public async Task<IEnumerable<SystemAccount>> SystemAccounts()
        {
            return await _systemAccountDAO.SystemAccounts();
        }
        public async Task<SystemAccount> GetSystemAccountById(short Id)
        {
            return await _systemAccountDAO.GetSystemAccountById(Id);
        }
        public async Task<SystemAccount> CreateSystemAccount(SystemAccount systemAccount)
        {
            return await _systemAccountDAO.CreateSystemAccount(systemAccount);
        }
        public async Task<SystemAccount> UpdateSystemAccount(SystemAccount systemAccount)
        {
            return await _systemAccountDAO.UpdateSystemAccount(systemAccount);
        }
        public async Task DeleteSystemAccount(short Id)
        {
            await _systemAccountDAO.DeleteSystemAccount(Id);
        }
        public async Task<bool> SystemAccountExists(short id)
        {
            return await _systemAccountDAO.SystemAccountExists(id);
        }
        public async Task<bool> IsEmailExit(string email)
        {
            return await _systemAccountDAO.IsEmailExit(email);
        }
    }
}