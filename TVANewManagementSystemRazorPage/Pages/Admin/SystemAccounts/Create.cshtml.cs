using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Interfaces;

namespace TVANewManagementSystemRazorPage.Pages.Admin.SystemAccounts
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ISystemAccountService _systemAccountService;
        private readonly IConfiguration _configuration;

        public CreateModel(ISystemAccountService systemAccountService, IConfiguration configuration)
        {
            _systemAccountService = systemAccountService;
            _configuration = configuration;
        }

        [BindProperty]
        public SystemAccount SystemAccount { get; set; } = new SystemAccount();

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public IEnumerable<SelectListItem> RoleList { get; set; } = default!;

        public IActionResult OnGet()
        {
            // Populate role dropdown
            var roleMapping = _configuration.GetSection("AccountRole")
                .Get<Dictionary<string, int>>()
                .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            RoleList = roleMapping.Select(r => new SelectListItem
            {
                Value = r.Key.ToString(),
                Text = r.Value
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (SystemAccount.AccountPassword != ConfirmPassword)
            {
                ModelState.AddModelError("SystemAccount.AccountPassword", "Passwords do not match.");
            }

            if (!ModelState.IsValid)
            {
                // Repopulate RoleList for the view in case of validation errors
                var roleMapping = _configuration.GetSection("AccountRole")
                    .Get<Dictionary<string, int>>()
                    .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
                RoleList = roleMapping.Select(r => new SelectListItem
                {
                    Value = r.Key.ToString(),
                    Text = r.Value
                }).ToList();

                return Page();
            }
            Random random = new Random();
            SystemAccount.AccountId = (short)random.Next(100, 1001);
            await _systemAccountService.CreateSystemAccount(SystemAccount);
            return RedirectToPage("Index");
        }
    }
}
