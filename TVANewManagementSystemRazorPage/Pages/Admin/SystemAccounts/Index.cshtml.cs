using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace TVANewManagementSystemRazorPage.Pages.Admin.SystemAccounts
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ISystemAccountService _systemAccountService;
        private readonly IConfiguration _configuration;

        public IndexModel(ISystemAccountService systemAccountService, IConfiguration configuration)
        {
            _systemAccountService = systemAccountService;
            _configuration = configuration;
        }

        public IList<SystemAccount> SystemAccounts { get; set; } = default!;
        public Dictionary<int, string> RoleMapping { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public async Task OnGetAsync()
        {
            // Get role mapping from configuration
            RoleMapping = _configuration.GetSection("AccountRole")
                .Get<Dictionary<string, int>>()
                .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            // Get system accounts
            var systemAccounts = await _systemAccountService.SystemAccounts();

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(SearchString))
            {
                systemAccounts = systemAccounts
                    .Where(a => a.AccountName.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ||
                               a.AccountEmail.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            SystemAccounts = (IList<SystemAccount>)systemAccounts;
        }
    }
}
