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
        [BindProperty] public SystemAccount systemAccount { get; set; }
        [BindProperty] public NewsArticle NewsArticle { get; set; }

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

            var newsArticles = await _newsArticleService.NewsArticlesStaff(
                searchString,
                categoryId ?? 0,
                systemAccount?.AccountId ?? 0);

            int totalItems = newsArticles.Count();
            var pagedArticles = newsArticles
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

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

            string userEmail = HttpContext.Session.GetString("UserEmail");
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
                NewsStatus = false, // Default to false instead of true
                CreatedDate = DateTime.Today,
                ModifiedDate = DateTime.Today,
                CreatedById = account?.AccountId ?? 0,
                UpdatedById = account?.AccountId
            });
        }

        // POST: NewsArticles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("NewsArticleId,NewsTitle,Headline,NewsContent,NewsSource,CategoryId")]
    NewsArticle newsArticle,
            int[] selectedTags)
        {
            string userEmail = HttpContext.Session.GetString("UserEmail");
            var account = await _systemAccountService.GetSystemAccountByEmail(userEmail);

            newsArticle.NewsStatus = false; // Default to false instead of true
            newsArticle.CreatedDate = DateTime.Today;
            newsArticle.ModifiedDate = DateTime.Today;
            newsArticle.CreatedById = account?.AccountId ?? 0;
            newsArticle.UpdatedById = account?.AccountId;

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
            var tags = await _tagService.GetAllTags();
            var selectedTagIds = newsArticle.Tags?.Select(t => t.TagId).ToArray() ?? new int[0];

            string userEmail = HttpContext.Session.GetString("UserEmail");
            var account = await _systemAccountService.GetSystemAccountByEmail(userEmail);

            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName", newsArticle.CategoryId);
            ViewData["CreatedById"] = new SelectList(systemAccounts, "AccountId", "AccountId", newsArticle.CreatedById);
            ViewData["Tags"] = new MultiSelectList(tags, "TagId", "TagName", selectedTagIds);

            newsArticle.UpdatedById = account?.AccountId; // Cập nhật UpdatedById
            newsArticle.ModifiedDate = DateTime.Today;    // Cập nhật ModifiedDate

            return View(newsArticle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,
    [Bind("NewsArticleId,NewsTitle,Headline,CreatedDate,NewsContent,NewsSource,CategoryId,NewsStatus,CreatedById,UpdatedById,ModifiedDate")]
    NewsArticle newsArticle,
    int[] selectedTags)
        {
            if (id != newsArticle.NewsArticleId)
            {
                return NotFound();
            }

            string userEmail = HttpContext.Session.GetString("UserEmail");
            var account = await _systemAccountService.GetSystemAccountByEmail(userEmail);

            newsArticle.UpdatedById = account?.AccountId; // Cập nhật UpdatedById
            newsArticle.ModifiedDate = DateTime.Today;    // Cập nhật ModifiedDate

            if (ModelState.IsValid)
            {
                try
                {
                    // Cập nhật tags
                    if (selectedTags != null && selectedTags.Any())
                    {
                        newsArticle.Tags = await _tagService.GetTagsByIdsAsync(selectedTags);
                    }
                    else
                    {
                        newsArticle.Tags = new List<Tag>(); // Nếu không chọn tag nào thì xóa hết tags
                    }

                    await _newsArticleService.UpdateNewsArticle(newsArticle);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await NewsArticleExists(newsArticle.NewsArticleId))
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
            var tags = await _tagService.GetAllTags();
            var currentTagIds = selectedTags.Any() ? selectedTags : newsArticle.Tags?.Select(t => t.TagId).ToArray() ?? new int[0];

            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName", newsArticle.CategoryId);
            ViewData["CreatedById"] = new SelectList(systemAccounts, "AccountId", "AccountId", newsArticle.CreatedById);
            ViewData["Tags"] = new MultiSelectList(tags, "TagId", "TagName", currentTagIds);

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