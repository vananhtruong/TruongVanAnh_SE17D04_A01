using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TVANewManagementSystemRazorPage.Pages.Staff.StaffNewsArticles
{
    [Authorize(Roles = "Staff")]
    public class EditModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ISystemAccountService _systemAccountService;
        private readonly ITagService _tagService;

        public EditModel(
            INewsArticleService newsArticleService,
            ICategoryService categoryService,
            ISystemAccountService systemAccountService,
            ITagService tagService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _systemAccountService = systemAccountService;
            _tagService = tagService;
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; }

        [BindProperty]
        public int[] SelectedTags { get; set; }

        public List<Category> Categories { get; set; }
        public List<Tag> Tags { get; set; }

        // GET: Hiển thị form chỉnh sửa
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

            // Lấy danh sách categories và tags
            Categories = await _categoryService.GetAllCategories();
            Tags = await _tagService.GetAllTags();

            // Lấy danh sách tag đã chọn
            SelectedTags = NewsArticle.Tags?.Select(t => t.TagId).ToArray() ?? new int[0];

            // Cập nhật thông tin người chỉnh sửa
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
            {
                var account = await _systemAccountService.GetSystemAccountById((short)userId);
                if (account != null)
                {
                    NewsArticle.UpdatedById = account.AccountId;
                    NewsArticle.ModifiedDate = DateTime.Today;
                }
            }

            return Page();
        }

        // POST: Xử lý khi submit form
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Tải lại danh sách khi validation thất bại
                Categories = await _categoryService.GetAllCategories();
                Tags = await _tagService.GetAllTags();
                return Page();
            }

            // Cập nhật thông tin người chỉnh sửa
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
            {
                var account = await _systemAccountService.GetSystemAccountById((short)userId);
                if (account != null)
                {
                    NewsArticle.UpdatedById = account.AccountId;
                    NewsArticle.ModifiedDate = DateTime.Today;
                }
            }

            // Xử lý tags
            if (SelectedTags != null && SelectedTags.Any())
            {
                NewsArticle.Tags = await _tagService.GetTagsByIdsAsync(SelectedTags);
            }
            else
            {
                NewsArticle.Tags = new List<Tag>(); // Xóa hết tags nếu không chọn
            }

            try
            {
                await _newsArticleService.UpdateNewsArticle(NewsArticle);
            }
            catch (Exception)
            {
                if (!await NewsArticleExists(NewsArticle.NewsArticleId))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> NewsArticleExists(string id)
        {
            return await _newsArticleService.NewsArticleExists(id);
        }
    }
}
