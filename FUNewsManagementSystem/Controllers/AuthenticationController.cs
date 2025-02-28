using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(SystemAccount accountRegister)
        {
            if (!ModelState.IsValid)
            {
                return View(accountRegister);
            }

            try
            {
                // Kiểm tra email đã tồn tại chưa
                if (string.IsNullOrWhiteSpace(accountRegister.AccountEmail))
                {
                    ModelState.AddModelError("AccountEmail", "Email không được để trống!");
                    return View(accountRegister);
                }

                var existingAccount = await _systemAccountService.GetSystemAccountByEmail(accountRegister.AccountEmail);
                if (existingAccount != null)
                {
                    ModelState.AddModelError("AccountEmail", "Email này đã được sử dụng!");
                    return View(accountRegister);
                }

                // Đảm bảo các trường bắt buộc khác không null
                if (string.IsNullOrWhiteSpace(accountRegister.AccountName))
                {
                    ModelState.AddModelError("AccountName", "Tên tài khoản không được để trống!");
                    return View(accountRegister);
                }
                if (string.IsNullOrWhiteSpace(accountRegister.AccountPassword))
                {
                    ModelState.AddModelError("AccountPassword", "Mật khẩu không được để trống!");
                    return View(accountRegister);
                }

                // Sinh AccountId ngẫu nhiên trong khoảng 1000-1999
                Random random = new Random();
                short newAccountId;
                int maxAttempts = 10; // Giới hạn số lần thử để tránh vòng lặp vô hạn
                int attempts = 0;

                do
                {
                    if (attempts >= maxAttempts)
                    {
                        throw new Exception("Không thể tạo AccountId duy nhất sau nhiều lần thử. Vui lòng thử lại sau.");
                    }
                    newAccountId = (short)random.Next(1000, 2000); // Sinh số từ 1000 đến 1999
                    attempts++;
                } while (await _systemAccountService.SystemAccountExists(newAccountId)); // Kiểm tra trùng lặp

                accountRegister.AccountId = newAccountId;
                accountRegister.AccountRole = 2; // Lecturer

                // Tạo tài khoản mới
                var newAccount = await _systemAccountService.CreateSystemAccount(accountRegister);

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (DbUpdateException dbEx)
            {
                var innerException = dbEx.InnerException?.Message ?? dbEx.Message;
                ModelState.AddModelError(string.Empty, $"Lỗi khi lưu dữ liệu: {innerException}");
                return View(accountRegister);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi khi đăng ký: {ex.Message}");
                return View(accountRegister);
            }
        }
    }
}
