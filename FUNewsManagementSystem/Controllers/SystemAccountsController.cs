using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using Services.Interfaces;
using FUNewsManagementSystem.Filters;

namespace FUNewsManagementSystem.Controllers
{
    [AuthorizeRole("Admin")]
    public class SystemAccountsController : Controller
    {
        private readonly ISystemAccountService _systemAccountService;
        private readonly IConfiguration _configuration;

        public SystemAccountsController(ISystemAccountService systemAccountService, IConfiguration configuration)
        {
            _systemAccountService = systemAccountService;
            _configuration = configuration;
        }

        // GET: SystemAccounts
        public async Task<IActionResult> Index(string searchString)
        {
            var systemAccounts = await _systemAccountService.SystemAccounts();

            if (!string.IsNullOrEmpty(searchString))
            {
                systemAccounts = systemAccounts
                    .Where(a => a.AccountName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                               a.AccountEmail.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var roleMapping = _configuration.GetSection("AccountRole")
                .Get<Dictionary<string, int>>()
                .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            ViewBag.RoleMapping = roleMapping;
            ViewBag.CurrentFilter = searchString;

            return View(systemAccounts);
        }

        // GET: SystemAccounts/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemAccount = await _systemAccountService.GetSystemAccountById(id.Value);
            if (systemAccount == null)
            {
                return NotFound();
            }

            var roleMapping = _configuration.GetSection("AccountRole")
                .Get<Dictionary<string, int>>()
                .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            ViewBag.RoleMapping = roleMapping;

            return View(systemAccount);
        }

        // GET: SystemAccounts/Create
        public IActionResult Create()
        {
            var roleMapping = _configuration.GetSection("AccountRole")
                .Get<Dictionary<string, int>>()
                .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            ViewBag.RoleList = roleMapping.Select(r => new SelectListItem
            {
                Value = r.Key.ToString(),
                Text = r.Value
            }).ToList();

            return View();
        }

        // POST: SystemAccounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,AccountName,AccountEmail,AccountRole,AccountPassword")] SystemAccount systemAccount, string confirmPassword)
        {
            if (systemAccount.AccountPassword != confirmPassword)
            {
                ModelState.AddModelError("AccountPassword", "Passwords do not match.");
            }

            if (ModelState.IsValid)
            {
                await _systemAccountService.CreateSystemAccount(systemAccount);
                return RedirectToAction(nameof(Index));
            }

            var roleMapping = _configuration.GetSection("AccountRole")
                .Get<Dictionary<string, int>>()
                .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            ViewBag.RoleList = roleMapping.Select(r => new SelectListItem
            {
                Value = r.Key.ToString(),
                Text = r.Value
            }).ToList();

            return View(systemAccount);
        }
        // GET: SystemAccounts/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemAccount = await _systemAccountService.GetSystemAccountById(id.Value);
            if (systemAccount == null)
            {
                return NotFound();
            }

            var roleMapping = _configuration.GetSection("AccountRole")
                .Get<Dictionary<string, int>>()
                .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            ViewBag.RoleList = roleMapping.Select(r => new SelectListItem
            {
                Value = r.Key.ToString(),
                Text = r.Value,
                Selected = r.Key == systemAccount.AccountRole
            }).ToList();

            return View(systemAccount);
        }

        // POST: SystemAccounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("AccountId,AccountName,AccountEmail,AccountRole,AccountPassword")] SystemAccount systemAccount)
        {
            if (id != systemAccount.AccountId)
            {
                return NotFound();
            }

            var existingAccount = await _systemAccountService.GetSystemAccountById(id);
            if (existingAccount == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingAccount.AccountName = systemAccount.AccountName;
                    existingAccount.AccountEmail = systemAccount.AccountEmail;
                    existingAccount.AccountRole = systemAccount.AccountRole;

                    if (!string.IsNullOrEmpty(systemAccount.AccountPassword))
                    {
                        existingAccount.AccountPassword = systemAccount.AccountPassword;
                    }

                    await _systemAccountService.UpdateSystemAccount(existingAccount);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await SystemAccountExists(systemAccount.AccountId);
                    if (!exists)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var roleMapping = _configuration.GetSection("AccountRole")
                .Get<Dictionary<string, int>>()
                .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            ViewBag.RoleList = roleMapping.Select(r => new SelectListItem
            {
                Value = r.Key.ToString(),
                Text = r.Value,
                Selected = r.Key == systemAccount.AccountRole
            }).ToList();

            return View(systemAccount);
        }

        // GET: SystemAccounts/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemAccount = await _systemAccountService.GetSystemAccountById(id.Value);
            if (systemAccount == null)
            {
                return NotFound();
            }

            return View(systemAccount);
        }

        // POST: SystemAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var systemAccount = await _systemAccountService.GetSystemAccountById(id);
            if (systemAccount != null)
            {
                await _systemAccountService.DeleteSystemAccount(id);
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> SystemAccountExists(short id)
        {
            return await _systemAccountService.SystemAccountExists(id);
        }
    }
}
