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
    public class LectureNewsArticlesController : Controller
    {
        private readonly INewsArticleService _newsArticleService;

        public LectureNewsArticlesController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        // GET: NewsArticles
        public async Task<IActionResult> Index()
        {
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
    }
}
