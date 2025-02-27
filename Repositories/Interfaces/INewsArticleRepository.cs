using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Interfaces
{
    public interface INewsArticleRepository
    {
        Task<List<NewsArticle>> GetAllActiveNewsArticles();
        Task<NewsArticle?> GetNewsArticleById(string newsArticleId);
        Task<NewsArticle> CreateNewsArticle(NewsArticle newsArticle);
        Task<NewsArticle?> UpdateNewsArticle(NewsArticle newsArticle);
        Task<bool> DeleteNewsArticle(string newsArticleId);
        Task AddTagToNewsArticle(string newsArticleId, int tagId);
        Task<bool> NewsArticleExists(string id);
        Task<List<NewsArticle>> SearchNewsArticles(string searchString);
    }
}