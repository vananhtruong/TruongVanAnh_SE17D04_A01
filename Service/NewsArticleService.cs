using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository _newsArticleRepository;

        public NewsArticleService(INewsArticleRepository newsArticleRepository)
        {
            _newsArticleRepository = newsArticleRepository;
        }

        public async Task<List<NewsArticle>> GetAllActiveNewsArticles()
        {
            return await _newsArticleRepository.GetAllActiveNewsArticles();
        }

        public async Task<NewsArticle?> GetNewsArticleById(string newsArticleId)
        {
            var article = await _newsArticleRepository.GetNewsArticleById(newsArticleId);
            if (article == null || article.NewsStatus == false)
            {
                return null;
            }
            return article;
        }

        public async Task<NewsArticle> CreateNewsArticle(NewsArticle newsArticle)
        {
            // Kiểm tra logic kinh doanh (ví dụ: Headline và NewsTitle không được rỗng)
            if (string.IsNullOrEmpty(newsArticle.Headline) || string.IsNullOrEmpty(newsArticle.NewsTitle))
            {
                throw new ArgumentException("Headline and NewsTitle are required.");
            }
            newsArticle.NewsStatus = true; // Mặc định bài viết mới là hoạt động
            newsArticle.CreatedDate = DateTime.Now;
            newsArticle.ModifiedDate = DateTime.Now;
            return await _newsArticleRepository.CreateNewsArticle(newsArticle);
        }

        public async Task<NewsArticle?> UpdateNewsArticle(NewsArticle newsArticle)
        {
            var existingArticle = await _newsArticleRepository.GetNewsArticleById(newsArticle.NewsArticleId);
            if (existingArticle == null || existingArticle.NewsStatus == false)
            {
                return null;
            }
            existingArticle.ModifiedDate = DateTime.Now;
            return await _newsArticleRepository.UpdateNewsArticle(newsArticle);
        }

        public async Task<bool> DeleteNewsArticle(string newsArticleId)
        {
            var article = await _newsArticleRepository.GetNewsArticleById(newsArticleId);
            if (article == null || article.NewsStatus == false)
            {
                return false;
            }
            return await _newsArticleRepository.DeleteNewsArticle(newsArticleId);
        }

        public async Task AddTagToNewsArticle(string newsArticleId, int tagId)
        {
            var article = await _newsArticleRepository.GetNewsArticleById(newsArticleId);
            if (article == null || article.NewsStatus == false)
            {
                throw new InvalidOperationException("News article not found or inactive.");
            }
            await _newsArticleRepository.AddTagToNewsArticle(newsArticleId, tagId);
        }
        public async Task<bool> NewsArticleExists(string id)
        {
            return await _newsArticleRepository.NewsArticleExists(id);
        }
        public async Task<List<NewsArticle>> NewsArticlesFilter(string searchString, int cateogryId)
        {
            return await _newsArticleRepository.NewsArticlesFilter(searchString, cateogryId);
        }
        public async Task<List<NewsArticle>> NewsArticlesStaff(string searchString, int cateogryId, int id)
        {
            return await _newsArticleRepository.NewsArticlesStaff(searchString, cateogryId, id);
        }
    }
}