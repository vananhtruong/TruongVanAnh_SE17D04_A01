using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class CategoryDAO
    {
        private readonly FunewsManagementContext _context;

        public CategoryDAO(FunewsManagementContext context)
        {
            _context = context;
        }

        // Lấy tất cả danh mục
        public async Task<List<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        // Lấy danh mục theo ID
        public async Task<Category?> GetCategoryById(short categoryId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }

        // Tạo mới danh mục
        public async Task<Category> CreateCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        // Cập nhật danh mục
        public async Task<Category?> UpdateCategory(Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(category.CategoryId);
            if (existingCategory != null)
            {
                _context.Entry(existingCategory).CurrentValues.SetValues(category);
                await _context.SaveChangesAsync();
                return existingCategory;
            }
            return null;
        }

        // Xóa danh mục (kiểm tra không liên quan đến bài viết tin tức)
        public async Task<bool> DeleteCategory(short categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null && !_context.NewsArticles.Any(na => na.CategoryId == categoryId))
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}