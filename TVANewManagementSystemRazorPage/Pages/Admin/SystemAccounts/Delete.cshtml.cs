using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace TVANewManagementSystemRazorPage.Pages.Admin.SystemAccounts
{
    public class DeleteModel : PageModel
    {
        private readonly ISystemAccountService _systemAccountService;

        public DeleteModel(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }

        [BindProperty]
        public SystemAccount SystemAccount { get; set; } = default!;

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

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (SystemAccount == null || SystemAccount.AccountId == 0)
            {
                return NotFound();
            }

            var account = await _systemAccountService.GetSystemAccountById(SystemAccount.AccountId);
            if (account != null)
            {
                await _systemAccountService.DeleteSystemAccount(SystemAccount.AccountId);
            }

            return RedirectToPage("Index");
        }
    }
}
