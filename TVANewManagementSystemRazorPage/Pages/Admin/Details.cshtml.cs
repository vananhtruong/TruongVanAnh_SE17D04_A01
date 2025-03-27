using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace TVANewManagementSystemRazorPage.Pages.Admin
{
    public class DetailsModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;

        public DetailsModel(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        public NewsArticle Article { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                Article = await _newsArticleService.GetNewsArticleById(id);
                if (Article == null)
                {
                    return NotFound();
                }
                return Page();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading article: {ex.Message}";
                return RedirectToPage("Index");
            }
        }

        public async Task<IActionResult> OnPostApproveAsync(string id)
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
                // Optionally add UpdatedById here with current admin's ID if available

                await _newsArticleService.UpdateNewsArticle(article);

                TempData["Success"] = "Article approved successfully";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error approving article: {ex.Message}";
                return RedirectToPage("Details", new { id });
            }
        }
    }
}
