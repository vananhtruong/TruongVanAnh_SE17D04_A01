using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObjects.Models;
using Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TVANewManagementSystemRazorPage.Pages.Staff.StaffNewsArticles
{
    [Authorize(Roles = "Staff")]
    public class CreateModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ISystemAccountService _systemAccountService;
        private readonly ITagService _tagService;

        public CreateModel(
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

        public SystemAccount SystemAccount { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Category> Categories { get; set; }

        // GET: Hiển thị form tạo mới
        public async Task<IActionResult> OnGetAsync()
        {
            int userId = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
            SystemAccount = await _systemAccountService.GetSystemAccountById((short)userId);
            Tags = await _tagService.GetAllTags();
            Categories = await _categoryService.GetAllCategories();

            // Tạo ID mới
            string newId;
            Random rand = new Random();
            do
            {
                newId = rand.Next(-32768, 32768).ToString();
            } while (await NewsArticleExists(newId));

            // Khởi tạo NewsArticle
            NewsArticle = new NewsArticle
            {
                NewsArticleId = newId,
                CreatedDate = DateTime.Today,
                ModifiedDate = DateTime.Today,
                CreatedById = SystemAccount?.AccountId ?? 0,
                UpdatedById = SystemAccount?.AccountId,
                NewsStatus = false
            };

            return Page();
        }

        // POST: Xử lý khi submit form
        public async Task<IActionResult> OnPostAsync()
        {
            int userId = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
            SystemAccount = await _systemAccountService.GetSystemAccountById((short)userId);
            Tags = await _tagService.GetAllTags();
            Categories = await _categoryService.GetAllCategories();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Cập nhật các giá trị từ SystemAccount
            NewsArticle.CreatedById = SystemAccount?.AccountId ?? 0;
            NewsArticle.UpdatedById = SystemAccount?.AccountId;
            NewsArticle.CreatedDate = DateTime.Today;
            NewsArticle.ModifiedDate = DateTime.Today;
            NewsArticle.NewsStatus = false; // Giá trị mặc định

            // Xử lý tags nếu có
            if (SelectedTags != null && SelectedTags.Any())
            {
                NewsArticle.Tags = await _tagService.GetTagsByIdsAsync(SelectedTags);
            }

            // Lưu bài viết
            await _newsArticleService.CreateNewsArticle(NewsArticle);
            return RedirectToPage("./Index");
        }

        private async Task<bool> NewsArticleExists(string id)
        {
            return await _newsArticleService.NewsArticleExists(id);
        }
    }
}