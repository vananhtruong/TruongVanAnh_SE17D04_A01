using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace TVANewManagementSystemRazorPage.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;

        public IndexModel(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        public IList<NewsArticle> PendingArticles { get; set; } = new List<NewsArticle>();

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public int CategoryId { get; set; } = 0;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get all articles with the filter
                var articles = await _newsArticleService.NewsArticlesFilter(SearchString, CategoryId);

                // Filter to show only pending articles (NewsStatus = false)
                PendingArticles = articles.Where(a => a.NewsStatus == false).ToList();

                return Page();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading articles: {ex.Message}";
                PendingArticles = new List<NewsArticle>();
                return Page();
            }
        }
    }
}
