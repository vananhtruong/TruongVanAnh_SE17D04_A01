using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using Services.Interfaces;
using Services;
using FUNewsManagementSystem.Filters;

namespace FUNewsManagementSystem.Controllers
{
    [AuthorizeRole("Lecture")]
    public class LectureNewsArticlesController : Controller
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;

        public LectureNewsArticlesController(INewsArticleService newsArticleService, ICategoryService categoryService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
        }

        // GET: NewsArticles
        public async Task<IActionResult> Index(string searchString, int? categoryId, int pageNumber = 1, int pageSize = 10)
        {
            var categories = await _categoryService.GetAllCategories();
            ViewData["CurrentFilter"] = searchString;
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName", categoryId);

            // Lấy danh sách bài viết từ service
            var newsArticles = await _newsArticleService.NewsArticlesFilter(searchString, categoryId ?? 0);

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
    }
}