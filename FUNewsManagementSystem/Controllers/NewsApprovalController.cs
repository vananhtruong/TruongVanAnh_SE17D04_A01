using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using FUNewsManagementSystem.Filters;

namespace YourProjectName.Controllers
{
    [AuthorizeRole("Admin")]
    public class NewsApprovalController : Controller
    {
        private readonly INewsArticleService _newsArticleService;

        public NewsApprovalController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        // GET: List all pending news articles (NewsStatus = false)
        public async Task<IActionResult> Index(string searchString = "", int categoryId = 0)
        {
            try
            {
                // Get all articles (including inactive ones for admin approval)
                var articles = await _newsArticleService.NewsArticlesFilter(searchString, categoryId);

                // Filter to show only pending articles (NewsStatus = false)
                var pendingArticles = articles.Where(a => a.NewsStatus == false).ToList();

                ViewData["SearchString"] = searchString;
                ViewData["CategoryId"] = categoryId;
                return View(pendingArticles);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading articles: {ex.Message}";
                return View(new List<NewsArticle>());
            }
        }

        // GET: View details of a specific article for approval
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var article = await _newsArticleService.GetNewsArticleById(id);
                if (article == null)
                {
                    return NotFound();
                }
                return View(article);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading article: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Approve a news article
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var article = await _newsArticleService.GetNewsArticleById(id);
                if (article == null)
                {
                    return NotFound();
                }

                // Set status to active/approved
                article.NewsStatus = true;
                article.ModifiedDate = DateTime.Now;
                // You might want to add UpdatedById here with current admin's ID

                await _newsArticleService.UpdateNewsArticle(article);

                TempData["Success"] = "Article approved successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error approving article: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: Reject a news article (soft delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var success = await _newsArticleService.DeleteNewsArticle(id);
                if (!success)
                {
                    return NotFound();
                }

                TempData["Success"] = "Article rejected successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error rejecting article: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}