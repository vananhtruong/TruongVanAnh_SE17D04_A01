using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using Services.Interfaces;

namespace FUNewsManagementSystem.Controllers
{
    public class NewsArticlesController : Controller
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ISystemAccountService _systemAccountService;

        public NewsArticlesController(INewsArticleService newsArticleService, ICategoryService categoryService, ISystemAccountService systemAccountService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _systemAccountService = systemAccountService;
        }

        // GET: NewsArticles
        public async Task<IActionResult> Index(string? searchString)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                var newsArticlesSearch = await _newsArticleService.SearchNewsArticles(searchString);
                ViewData["CurrentFilter"] = searchString;
                return View(newsArticlesSearch);
            }
            var newsArticles = await _newsArticleService.GetAllActiveNewsArticles();
            return View(newsArticles);
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
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryDesciption");
            ViewData["CreatedById"] = new SelectList(systemAccounts, "AccountId", "AccountId");
            return View();
        }

        // POST: NewsArticles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NewsArticleId,NewsTitle,Headline,CreatedDate,NewsContent,NewsSource,CategoryId,NewsStatus,CreatedById,UpdatedById,ModifiedDate")] NewsArticle newsArticle)
        {
            if (ModelState.IsValid)
            {
                await _newsArticleService.CreateNewsArticle(newsArticle);
                return RedirectToAction(nameof(Index));
            }
            var categories = await _categoryService.GetAllCategories();
            var systemAccounts = await _systemAccountService.SystemAccounts();
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryDesciption", newsArticle.CategoryId);
            ViewData["CreatedById"] = new SelectList(systemAccounts, "AccountId", "AccountId", newsArticle.CreatedById);
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
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryDesciption", newsArticle.CategoryId);
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
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryDesciption", newsArticle.CategoryId);
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
