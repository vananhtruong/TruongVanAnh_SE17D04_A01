using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using Services.Interfaces;

namespace FUNewsManagementSystem.Controllers
{
    public class SystemAccountsController : Controller
    {
        private readonly ISystemAccountService _systemAccountService;

        public SystemAccountsController(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        } 

        // GET: SystemAccounts
        public async Task<IActionResult> Index()
        {
            var systemAccounts = await _systemAccountService.SystemAccounts();
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

            return View(systemAccount);
        }

        // GET: SystemAccounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SystemAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,AccountName,AccountEmail,AccountRole,AccountPassword")] SystemAccount systemAccount)
        {
            if (ModelState.IsValid)
            {
                await _systemAccountService.CreateSystemAccount(systemAccount);
                return RedirectToAction(nameof(Index));
            }
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
            return View(systemAccount);
        }

        // POST: SystemAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("AccountId,AccountName,AccountEmail,AccountRole,AccountPassword")] SystemAccount systemAccount)
        {
            if (id != systemAccount.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _systemAccountService.UpdateSystemAccount(systemAccount);
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
