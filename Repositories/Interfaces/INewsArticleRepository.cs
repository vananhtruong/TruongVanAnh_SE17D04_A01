using BusinessObjects.Models;

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
    }
}