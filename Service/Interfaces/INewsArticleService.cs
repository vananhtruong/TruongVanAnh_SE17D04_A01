using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Services.Interfaces
{
    public interface INewsArticleService
    {
        Task<List<NewsArticle>> GetAllActiveNewsArticles();
        Task<NewsArticle?> GetNewsArticleById(string newsArticleId);
        Task<NewsArticle> CreateNewsArticle(NewsArticle newsArticle);
        Task<NewsArticle?> UpdateNewsArticle(NewsArticle newsArticle);
        Task<bool> DeleteNewsArticle(string newsArticleId);
        Task AddTagToNewsArticle(string newsArticleId, int tagId);
        Task<bool> NewsArticleExists(string id);
    }
}