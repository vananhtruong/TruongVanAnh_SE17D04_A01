using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class NewsArticleDAO
    {
        private readonly FunewsManagementContext _context;

        public NewsArticleDAO(FunewsManagementContext context)
        {
            _context = context;
        }

        // Lấy tất cả bài viết tin tức (chỉ lấy bài viết hoạt động)
        public async Task<List<NewsArticle>> GetAllActiveNewsArticles()
        {
            return await _context.NewsArticles
                .Where(na => na.NewsStatus == true)
                .ToListAsync();
        }

        // Lấy bài viết theo ID
        public async Task<NewsArticle?> GetNewsArticleById(string newsArticleId)
        {
            return await _context.NewsArticles.FindAsync(newsArticleId);
        }

        // Tạo mới bài viết tin tức
        public async Task<NewsArticle> CreateNewsArticle(NewsArticle newsArticle)
        {
            _context.NewsArticles.Add(newsArticle);
            await _context.SaveChangesAsync();
            return newsArticle;
        }

        // Cập nhật bài viết tin tức
        public async Task<NewsArticle?> UpdateNewsArticle(NewsArticle newsArticle)
        {
            var existingArticle = await _context.NewsArticles.FindAsync(newsArticle.NewsArticleId);
            if (existingArticle != null)
            {
                _context.Entry(existingArticle).CurrentValues.SetValues(newsArticle);
                await _context.SaveChangesAsync();
                return existingArticle;
            }
            return null;
        }

        // Xóa bài viết tin tức
        public async Task<bool> DeleteNewsArticle(string newsArticleId)
        {
            var newsArticle = await _context.NewsArticles.FindAsync(newsArticleId);
            if (newsArticle != null)
            {
                _context.NewsArticles.Remove(newsArticle);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Thêm thẻ vào bài viết tin tức
        public async Task AddTagToNewsArticle(string newsArticleId, int tagId)
        {
            var newsArticle = await _context.NewsArticles.FindAsync(newsArticleId);
            var tag = await _context.Tags.FindAsync(tagId);
            if (newsArticle != null && tag != null)
            {
                newsArticle.Tags.Add(tag);
                await _context.SaveChangesAsync();
            }
        }
    }
} 