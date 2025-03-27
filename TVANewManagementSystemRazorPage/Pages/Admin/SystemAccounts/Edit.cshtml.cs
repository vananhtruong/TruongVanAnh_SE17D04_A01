using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;

namespace TVANewManagementSystemRazorPage.Pages.Admin.SystemAccounts
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly ISystemAccountService _systemAccountService;
        private readonly IConfiguration _configuration;

        public EditModel(ISystemAccountService systemAccountService, IConfiguration configuration)
        {
            _systemAccountService = systemAccountService;
            _configuration = configuration;
        }

        [BindProperty]
        public SystemAccount SystemAccount { get; set; } = default!;
        public IEnumerable<SelectListItem> RoleList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SystemAccount = await _systemAccountService.GetSystemAccountById(id.Value);

            if (SystemAccount == null)
            {
                return NotFound();
            }

            // Populate role dropdown
            var roleMapping = _configuration.GetSection("AccountRole")
                .Get<Dictionary<string, int>>()
                .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            RoleList = roleMapping.Select(r => new SelectListItem
            {
                Value = r.Key.ToString(),
                Text = r.Value,
                Selected = r.Key == SystemAccount.AccountRole
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Repopulate RoleList for the view in case of validation errors
                var roleMapping = _configuration.GetSection("AccountRole")
                    .Get<Dictionary<string, int>>()
                    .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
                RoleList = roleMapping.Select(r => new SelectListItem
                {
                    Value = r.Key.ToString(),
                    Text = r.Value,
                    Selected = r.Key == SystemAccount.AccountRole
                }).ToList();

                return Page();
            }

            var existingAccount = await _systemAccountService.GetSystemAccountById(SystemAccount.AccountId);
            if (existingAccount == null)
            {
                return NotFound();
            }

            try
            {
                existingAccount.AccountName = SystemAccount.AccountName;
                existingAccount.AccountEmail = SystemAccount.AccountEmail;
                existingAccount.AccountRole = SystemAccount.AccountRole;

                if (!string.IsNullOrEmpty(SystemAccount.AccountPassword))
                {
                    existingAccount.AccountPassword = SystemAccount.AccountPassword;
                }

                await _systemAccountService.UpdateSystemAccount(existingAccount);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SystemAccountExists(SystemAccount.AccountId))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToPage("Index");
        }

        private async Task<bool> SystemAccountExists(short id)
        {
            return await _systemAccountService.SystemAccountExists(id);
        }
    }
}
