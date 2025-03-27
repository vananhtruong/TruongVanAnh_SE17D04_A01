using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace TVANewManagementSystemRazorPage.Pages.Admin.SystemAccounts
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly ISystemAccountService _systemAccountService;
        private readonly IConfiguration _configuration;

        public DetailsModel(ISystemAccountService systemAccountService, IConfiguration configuration)
        {
            _systemAccountService = systemAccountService;
            _configuration = configuration;
        }

        public SystemAccount SystemAccount { get; set; } = default!;
        public Dictionary<int, string> RoleMapping { get; set; } = default!;

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

            // Get role mapping from configuration
            RoleMapping = _configuration.GetSection("AccountRole")
                .Get<Dictionary<string, int>>()
                .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            return Page();
        }
    }
}
