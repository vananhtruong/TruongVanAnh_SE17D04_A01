using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TVANewManagementSystemRazorPage.Pages.Staff.StaffNewsArticles
{
    [Authorize(Roles = "Staff")]
    public class DeleteModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;

        public DeleteModel(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; }

        // GET: Hiển thị trang xác nhận xóa
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            NewsArticle = await _newsArticleService.GetNewsArticleById(id);
            if (NewsArticle == null)
            {
                return NotFound();
            }

            return Page();
        }

        // POST: Xử lý xóa bài viết
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(NewsArticle.NewsArticleId))
            {
                return NotFound();
            }

            await _newsArticleService.DeleteNewsArticle(NewsArticle.NewsArticleId);
            return RedirectToPage("./Index");
        }
    }
}
