using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Interfaces;

namespace TVANewManagementSystemRazorPage.Pages.Lecture
{
    public class IndexModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;

        public IndexModel(INewsArticleService newsArticleService, ICategoryService categoryService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
        }

        public IList<NewsArticle> NewsArticles { get; set; } = default!;
        public SelectList CategorySelectList { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync()
        {
            var categories = await _categoryService.GetAllCategories();
            CategorySelectList = new SelectList(categories, "CategoryId", "CategoryName", CategoryId);

            // Filter news articles
            var newsArticles = await _newsArticleService.NewsArticlesFilter(SearchString, CategoryId ?? 0);

            // Filter by NewsStatus = true and sort by CreatedDate descending
            var filteredAndSortedArticles = newsArticles
                .Where(na => na.NewsStatus == true) // Only articles with NewsStatus = true
                .OrderByDescending(na => na.CreatedDate) // Newest first
                .ToList();

            // Pagination
            TotalItems = filteredAndSortedArticles.Count();
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);
            NewsArticles = filteredAndSortedArticles
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
