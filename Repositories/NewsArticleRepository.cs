using BusinessObjects.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        private readonly NewsArticleDAO _newsArticleDAO;

        public NewsArticleRepository(NewsArticleDAO newsArticleDAO)
        {
            _newsArticleDAO = newsArticleDAO;
        }

        public async Task<List<NewsArticle>> GetAllActiveNewsArticles()
        {
            return await _newsArticleDAO.GetAllActiveNewsArticles();
        }

        public async Task<NewsArticle?> GetNewsArticleById(string newsArticleId)
        {
            return await _newsArticleDAO.GetNewsArticleById(newsArticleId);
        }

        public async Task<NewsArticle> CreateNewsArticle(NewsArticle newsArticle)
        {
            return await _newsArticleDAO.CreateNewsArticle(newsArticle);
        }

        public async Task<NewsArticle?> UpdateNewsArticle(NewsArticle newsArticle)
        {
            return await _newsArticleDAO.UpdateNewsArticle(newsArticle);
        }

        public async Task<bool> DeleteNewsArticle(string newsArticleId)
        {
            return await _newsArticleDAO.DeleteNewsArticle(newsArticleId);
        }

        public async Task AddTagToNewsArticle(string newsArticleId, int tagId)
        {
            await _newsArticleDAO.AddTagToNewsArticle(newsArticleId, tagId);
        }
        public async Task<bool> NewsArticleExists(string id)
        {
            return await _newsArticleDAO.NewsArticleExists(id);
        }
    }
}