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
            return await _context.Categories.Include(c => c.ParentCategory).ToListAsync();
        }

        // Lấy danh mục theo ID
        public async Task<Category?> GetCategoryById(short categoryId)
        {
            return await _context.Categories
                                 .Include(c => c.ParentCategory)
                                 .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
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

        public async Task<bool> DeleteCategory(short categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return false;
            }

            bool hasNewsArticles = _context.NewsArticles.Any(na => na.CategoryId == categoryId);

            // Kiểm tra xem category có phải là parent category của category nào khác không
            bool isParentCategory = _context.Categories.Any(c => c.ParentCategoryId == categoryId);

            if (hasNewsArticles || isParentCategory)
            {
                category.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return true;
            }
        }
        public bool CategoryExists(short id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}