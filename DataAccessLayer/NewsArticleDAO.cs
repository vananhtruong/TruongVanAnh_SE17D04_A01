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
                .Include(c => c.Category)
                .Include(a => a.CreatedBy)
                .Include(t => t.Tags)
                .ToListAsync();
        }

        // Lấy bài viết theo ID
        public async Task<NewsArticle?> GetNewsArticleById(string newsArticleId)
        {
            return await _context.NewsArticles
                .Include(c => c.Category).Include(a => a.CreatedBy).Include(t => t.Tags)
                .FirstOrDefaultAsync(na => na.NewsArticleId == newsArticleId);
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
            var existingArticle = await _context.NewsArticles
                .Include(na => na.Tags) // Nạp danh sách Tags hiện tại
                .FirstOrDefaultAsync(na => na.NewsArticleId == newsArticle.NewsArticleId);

            if (existingArticle != null)
            {
                // Cập nhật các thuộc tính cơ bản
                _context.Entry(existingArticle).CurrentValues.SetValues(newsArticle);

                // Xử lý Tags
                if (newsArticle.Tags != null)
                {
                    // Xóa các tag cũ trong bảng trung gian
                    existingArticle.Tags.Clear();

                    // Thêm các tag mới từ danh sách Tags của newsArticle
                    foreach (var tag in newsArticle.Tags)
                    {
                        var tagToAdd = await _context.Tags.FindAsync(tag.TagId);
                        if (tagToAdd != null)
                        {
                            existingArticle.Tags.Add(tagToAdd);
                        }
                    }
                }

                // Cập nhật ModifiedDate
                existingArticle.ModifiedDate = DateTime.Now;

                // Lưu thay đổi
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
                // Perform a soft delete by setting NewsStatus to false
                newsArticle.NewsStatus = false;
                // Optionally update ModifiedDate to reflect the change
                newsArticle.ModifiedDate = DateTime.Today;

                // Save the changes
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
        public async Task<bool> NewsArticleExists(string id)
        {
            return await _context.NewsArticles.AnyAsync(e => e.NewsArticleId == id);
        }
        public async Task<List<NewsArticle>> NewsArticlesFilter(string searchString, int cateogryId)
        {
            // Lấy tất cả bài viết nếu không có điều kiện nào
            var query = _context.NewsArticles.Include(t => t.Tags).AsQueryable();

            // Xử lý searchString
            if (!string.IsNullOrEmpty(searchString))
            {
                string searchLower = searchString.ToLower();
                query = query.Where(na => na.NewsTitle.ToLower().Contains(searchLower) ||
                                        na.Headline.ToLower().Contains(searchLower) ||
                                        na.NewsContent.ToLower().Contains(searchLower) ||
                                        na.Category.CategoryName.ToLower().Contains(searchLower) ||
                                        na.CreatedBy.AccountName.ToLower().Contains(searchLower));
            }

            // Xử lý cateogryId
            if (cateogryId > 0) // Giả sử cateogryId > 0 là giá trị hợp lệ
            {
                query = query.Where(na => na.CategoryId == cateogryId);
            }

            return await query.ToListAsync();
        }
        public async Task<List<NewsArticle>> NewsArticlesStaff(string searchString, int cateogryId, int id)
        {
            // Lấy tất cả bài viết nếu không có điều kiện nào
            var query = _context.NewsArticles.Include(t => t.Tags).AsQueryable();

            // Xử lý searchString
            if (!string.IsNullOrEmpty(searchString))
            {
                string searchLower = searchString.ToLower();
                query = query.Where(na => na.NewsTitle.ToLower().Contains(searchLower) ||
                                        na.Headline.ToLower().Contains(searchLower) ||
                                        na.NewsContent.ToLower().Contains(searchLower) ||
                                        na.Category.CategoryName.ToLower().Contains(searchLower) ||
                                        na.CreatedBy.AccountName.ToLower().Contains(searchLower));
            }

            // Xử lý cateogryId
            if (cateogryId > 0) // Giả sử cateogryId > 0 là giá trị hợp lệ
            {
                query = query.Where(na => na.CategoryId == cateogryId);
            }
            if(id > 0)
            {
                query = query.Where(na => na.CreatedBy.AccountId == id);
            }

            return await query.ToListAsync();
        }
    }
} 