using BusinessObjects.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;
using Repositories.Interfaces;
using Services.Interfaces;
using System.Security.Claims;

namespace TVANewManagementSystemRazorPage.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ISystemAccountService _systemAccountService;
        private readonly IConfiguration _configuration;

        [BindProperty] public SystemAccount systemAccount { get; set; }

        public LoginModel(ISystemAccountService systemAccountService, IConfiguration configuration)
        {
            _systemAccountService = systemAccountService;
            _configuration = configuration;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _systemAccountService.GetSystemAccount(systemAccount.AccountEmail, systemAccount.AccountPassword);

            var adminEmail = _configuration["AccountAdmin:Email"];
            var adminPassword = _configuration["AccountAdmin:Password"];

            // Kiểm tra xem có phải tài khoản admin từ config không
            bool isAdminFromConfig = systemAccount.AccountEmail == adminEmail &&
                                   systemAccount.AccountPassword == adminPassword;

            if (user != null || isAdminFromConfig)
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, systemAccount.AccountEmail),
                // Nếu là admin từ config, gán role Admin, nếu không thì lấy role từ database
                new Claim(ClaimTypes.Role, isAdminFromConfig ? "Admin" : GetRole(user?.AccountRole.Value ?? 0)),
            };

                // Nếu user không null thì thêm UserId
                if (user != null)
                {
                    claims.Add(new Claim("UserId", user.AccountId.ToString()));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                // Điều hướng dựa trên role
                if (isAdminFromConfig || (user?.AccountRole == 3))
                {
                    return RedirectToPage("/Admin/Index"); // Có thể tạo page riêng cho admin
                }
                else if (user?.AccountRole == 1)
                {
                    return RedirectToPage("/Index");
                }
                else if (user?.AccountRole == 2)
                {
                    return RedirectToPage("/Privacy");
                }
            }

            return RedirectToPage("/Index");
        }

        private string GetRole(int role)
        {
            var roleSection = _configuration.GetSection("AccountRole");
            foreach (var child in roleSection.GetChildren())
            {
                if (int.TryParse(child.Value, out int roleValue) && roleValue == role)
                {
                    return child.Key;
                }
            }
            return "Unknown";
        }
    }
}
