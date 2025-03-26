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
    public class GuestNewsArticlesController : Controller
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;

        public GuestNewsArticlesController(INewsArticleService newsArticleService, ICategoryService categoryService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
        }

        // GET: NewsArticles
        public async Task<IActionResult> Index(string searchString, int? categoryId)
        {
            var categories = await _categoryService.GetAllCategories();
            ViewData["CurrentFilter"] = searchString;
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName", categoryId);
            var newsArticles = await _newsArticleService.NewsArticlesFilter(searchString, categoryId ?? 0);
            newsArticles = newsArticles.Where(n => n.NewsStatus == true).ToList();
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
    }
}
