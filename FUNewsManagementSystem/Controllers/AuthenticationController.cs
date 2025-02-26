using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Services.Interfaces;

namespace FUNewsManagementSystem.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly ISystemAccountService _systemAccountService;
        public AuthenticationController(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(SystemAccount accountLogin)
        {
            var (role, message) = await _systemAccountService.Login(accountLogin, HttpContext);

            if (!string.IsNullOrEmpty(message))
            {
                ModelState.AddModelError(string.Empty, message);
                return View(accountLogin);
            }
            return role switch
            {
                1 => RedirectToAction("Index", "NewsArticles"), // Staff
                2 => RedirectToAction("Index", "LectureNewsArticles", new { area = "Lecturer" }), // Lecture
                3 => RedirectToAction("Index", "SystemAccounts"), // Admin
                _ => NoContent() // Trường hợp không hợp lệ
            };
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "GuestNewsArticles");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
