using BusinessObjects.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;

namespace TVANewManagementSystemRazorPage.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ISystemAccountService _systemAccountService;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public SystemAccount systemAccount { get; set; }

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

            // Check if it's the admin account from config
            bool isAdminFromConfig = systemAccount.AccountEmail == adminEmail &&
                                    systemAccount.AccountPassword == adminPassword;

            if (user != null || isAdminFromConfig)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, systemAccount.AccountEmail),
                    new Claim(ClaimTypes.Role, isAdminFromConfig ? "Admin" : GetRole(user?.AccountRole.Value ?? 0)),
                };

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

                // Redirect based on role
                if (isAdminFromConfig || (user?.AccountRole == 3))
                {
                    return RedirectToPage("/Admin/SystemAccounts/Index"); // Admin
                }
                else if (user?.AccountRole == 1)
                {
                    return RedirectToPage("/Staff/StaffNewsArticles/Index"); // Staff
                }
                else if (user?.AccountRole == 2)
                {
                    return RedirectToPage("/Lecture/Index"); // Lecture
                }
            }
            else
            {
                ModelState.AddModelError("LoginError", "Email hoặc mật khẩu không đúng.");
                return Page();
            }

            return RedirectToPage("/Index");
        }

        // Initiate Google login
        public IActionResult OnPostGoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Page("/Login", "GoogleResponse") // Redirect to GoogleResponse handler
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Handle Google login response
        public async Task<IActionResult> OnGetGoogleResponseAsync()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Principal == null)
            {
                return RedirectToPage("/Login");
            }

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            try
            {
                var (account, role) = await _systemAccountService.HandleGoogleLogin(email, name, HttpContext);

                // Sign in the user with cookie authentication
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Role, role)
                };

                if (account != null)
                {
                    claims.Add(new Claim("UserId", account.AccountId.ToString()));
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

                // Redirect based on role
                return role switch
                {
                    "Staff" => RedirectToPage("/Staff/StaffNewsArticles/Index"),
                    "Lecture" => RedirectToPage("/Lecture/Index"),
                    "Admin" => RedirectToPage("/Admin/SystemAccounts/Index"),
                    _ => RedirectToPage("/Index") // Default case
                };
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi khi đăng nhập bằng Google: {ex.Message}");
                return RedirectToPage("/Login");
            }
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