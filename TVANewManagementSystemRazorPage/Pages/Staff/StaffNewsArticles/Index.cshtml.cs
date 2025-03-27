using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace TVANewManagementSystemRazorPage.Pages.Staff.StaffNewsArticles
{
    [Authorize(Roles = "Staff")]
    public class IndexModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ISystemAccountService _systemAccountService;
        public IndexModel(
            INewsArticleService newsArticleService,
            ICategoryService categoryService,
            ISystemAccountService systemAccountService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _systemAccountService = systemAccountService;
        }
        public IList<NewsArticle> NewsArticles { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool CreatedByMe { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Thêm các thuộc tính thay cho ViewBag
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            SystemAccount systemAccount = null;
            if (CreatedByMe)
            {
                string userEmail = HttpContext.Session.GetString("UserEmail");
                systemAccount = await _systemAccountService.GetSystemAccountByEmail(userEmail);
            }

            var categories = await _categoryService.GetAllCategories();
            ViewData["CurrentFilter"] = SearchString;
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName", CategoryId);
            ViewData["CreatedByMe"] = CreatedByMe;

            var newsArticles = await _newsArticleService.NewsArticlesStaff(
                SearchString,
                CategoryId ?? 0,
                systemAccount?.AccountId ?? 0);

            TotalItems = newsArticles.Count();
            NewsArticles = newsArticles
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            // Không cần ViewBag nữa, dữ liệu đã nằm trong thuộc tính
            return Page();
        }
    }
}
