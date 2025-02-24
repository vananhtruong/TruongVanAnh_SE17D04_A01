using BusinessObjects.Models;

namespace Services.Interfaces
{
    public interface INewsArticleService
    {
        Task<List<NewsArticle>> GetAllActiveNewsArticles();
        Task<NewsArticle?> GetNewsArticleById(string newsArticleId);
        Task<NewsArticle> CreateNewsArticle(NewsArticle newsArticle, short createdById);
        Task<NewsArticle?> UpdateNewsArticle(NewsArticle newsArticle);
        Task<bool> DeleteNewsArticle(string newsArticleId);
        Task AddTagToNewsArticle(string newsArticleId, int tagId);
    }
}