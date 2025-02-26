using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repositories.Interfaces;
using Services.Interfaces;

namespace FUNewsManagementSystem.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ISystemAccountService _systemAccountService;

        public ProfileController(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }
        public async Task<IActionResult> Details()
        {
            string userEmail = HttpContext.Session.GetString("UserEmail");
            var message = "";
            if (string.IsNullOrEmpty(userEmail))
            {
                message = "User email not found in session.";
            }

            var account = await _systemAccountService.GetSystemAccountByEmail(userEmail);
            if (!string.IsNullOrEmpty(message))
            {
                ModelState.AddModelError(string.Empty, message);
                return View(account);
            }
            return View(account);
        }
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
                return RedirectToAction(nameof(Details));
            }
            return View(systemAccount);
        }
        private async Task<bool> SystemAccountExists(short id)
        {
            return await _systemAccountService.SystemAccountExists(id);
        }
    }
}
