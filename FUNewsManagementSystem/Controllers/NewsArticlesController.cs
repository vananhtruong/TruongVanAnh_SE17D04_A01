using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using Services.Interfaces;
using FUNewsManagementSystem.Filters;

namespace FUNewsManagementSystem.Controllers
{
    [AuthorizeRole("Staff")]
    public class NewsArticlesController : Controller
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ISystemAccountService _systemAccountService;
        private readonly ITagService _tagService;
        public NewsArticlesController(INewsArticleService newsArticleService, ICategoryService categoryService, ISystemAccountService systemAccountService, ITagService tagService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _systemAccountService = systemAccountService;
            _tagService = tagService;
        }

        // GET: NewsArticles
        // GET: NewsArticles
        public async Task<IActionResult> Index(string searchString, int? categoryId, bool createdByMe, int pageNumber = 1, int pageSize = 10)
        {
            SystemAccount systemAccount = null;
            if (createdByMe)
            {
                string userEmail = HttpContext.Session.GetString("UserEmail");
                systemAccount = await _systemAccountService.GetSystemAccountByEmail(userEmail);
            }

            var categories = await _categoryService.GetAllCategories();
            ViewData["CurrentFilter"] = searchString;
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName", categoryId);
            ViewData["CreatedByMe"] = createdByMe;

            // Lấy danh sách bài viết
            var newsArticles = await _newsArticleService.NewsArticlesStaff(
                searchString,
                categoryId ?? 0,
                systemAccount?.AccountId ?? 0);

            // Tính toán phân trang
            int totalItems = newsArticles.Count();
            var pagedArticles = newsArticles
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Truyền thông tin phân trang vào ViewBag
            ViewBag.TotalItems = totalItems;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(pagedArticles);
        }

        // GET: NewsArticles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsArticle = await _newsArticleService.GetNewsArticleById(id);
            if (newsArticle == null)
            {
                return NotFound();
            }

            return View(newsArticle);
        }

        // GET: NewsArticles/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategories();
            var systemAccounts = await _systemAccountService.SystemAccounts();
            var tags = await _tagService.GetAllTags();

            // Lấy email từ session và tìm account
            string userEmail = HttpContext.Session.GetString("UserEmail");
            var message = "";
            if (string.IsNullOrEmpty(userEmail))
            {
                message = "User email not found in session.";
            }
            var account = await _systemAccountService.GetSystemAccountByEmail(userEmail);

            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName");
            ViewData["CreatedById"] = new SelectList(systemAccounts, "AccountId", "AccountId");
            ViewData["Tags"] = new MultiSelectList(tags, "TagId", "TagName");

            string newId;
            Random rand = new Random();
            do
            {
                newId = rand.Next(-32768, 32768).ToString();
            } while (await NewsArticleExists(newId)); 

            return View(new NewsArticle
            {
                NewsArticleId = newId,
                NewsStatus = true,
                CreatedDate = DateTime.Today,
                ModifiedDate = DateTime.Today,
                CreatedById = account?.AccountId ?? 0,
                UpdatedById = account?.AccountId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    [Bind("NewsArticleId,NewsTitle,Headline,NewsContent,NewsSource,CategoryId")]
    NewsArticle newsArticle,
    int[] selectedTags)
        {
            // Lấy email từ session và tìm account
            string userEmail = HttpContext.Session.GetString("UserEmail");
            var message = "";
            if (string.IsNullOrEmpty(userEmail))
            {
                message = "User email not found in session.";
            }
            var account = await _systemAccountService.GetSystemAccountByEmail(userEmail);

            // Gán các giá trị mặc định (chỉ nếu cần thiết, không ghi đè NewsArticleId từ form)
            newsArticle.NewsStatus = true;
            newsArticle.CreatedDate = DateTime.Today;
            newsArticle.ModifiedDate = DateTime.Today;
            newsArticle.CreatedById = account?.AccountId ?? 0;
            newsArticle.UpdatedById = account?.AccountId;

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        ErrorMessage = x.Value.Errors.FirstOrDefault()?.ErrorMessage
                    })
                    .ToList();
                ViewData["ValidationErrors"] = errors;
            }

            if (ModelState.IsValid)
            {
                if (selectedTags != null && selectedTags.Any())
                {
                    newsArticle.Tags = await _tagService.GetTagsByIdsAsync(selectedTags);
                }
                await _newsArticleService.CreateNewsArticle(newsArticle);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryService.GetAllCategories();
            var systemAccounts = await _systemAccountService.SystemAccounts();
            var tags = await _tagService.GetAllTags();

            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName", newsArticle.CategoryId);
            ViewData["CreatedById"] = new SelectList(systemAccounts, "AccountId", "AccountId", newsArticle.CreatedById);
            ViewData["Tags"] = new MultiSelectList(tags, "TagId", "TagName", selectedTags);
            return View(newsArticle);
        }
        // GET: NewsArticles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsArticle = await _newsArticleService.GetNewsArticleById(id);
            if (newsArticle == null)
            {
                return NotFound();
            }
            var categories = await _categoryService.GetAllCategories();
            var systemAccounts = await _systemAccountService.SystemAccounts();
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName", newsArticle.CategoryId);
            ViewData["CreatedById"] = new SelectList(systemAccounts, "AccountId", "AccountId", newsArticle.CreatedById);
            return View(newsArticle);
        }

        // POST: NewsArticles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NewsArticleId,NewsTitle,Headline,CreatedDate,NewsContent,NewsSource,CategoryId,NewsStatus,CreatedById,UpdatedById,ModifiedDate")] NewsArticle newsArticle)
        {
            if (id != newsArticle.NewsArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _newsArticleService.UpdateNewsArticle(newsArticle);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await NewsArticleExists(newsArticle.NewsArticleId);
                    if (!exists)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var categories = await _categoryService.GetAllCategories();
            var systemAccounts = await _systemAccountService.SystemAccounts();
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName", newsArticle.CategoryId);
            ViewData["CreatedById"] = new SelectList(systemAccounts, "AccountId", "AccountId", newsArticle.CreatedById);
            return View(newsArticle);
        }

        // GET: NewsArticles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsArticle = await _newsArticleService.GetNewsArticleById(id);
            if (newsArticle == null)
            {
                return NotFound();
            }

            return View(newsArticle);
        }

        // POST: NewsArticles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _newsArticleService.DeleteNewsArticle(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> NewsArticleExists(string id)
        {
            return await _newsArticleService.NewsArticleExists(id);
        }
    }
}
