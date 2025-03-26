using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllCategories();
            return categories; 
        }

        public async Task<Category?> GetCategoryById(short categoryId)
        {
            var category = await _categoryRepository.GetCategoryById(categoryId);
            if (category == null || category.IsActive == false)
            {
                return null;
            }
            return category;
        }

        public async Task<Category> CreateCategory(Category category)
        {
            // Kiểm tra logic kinh doanh (ví dụ: CategoryName không được trùng lặp)
            var categories = await _categoryRepository.GetAllCategories();
            if (categories.Any(c => c.CategoryName == category.CategoryName))
            {
                throw new ArgumentException("Category name already exists.");
            }
            category.IsActive = true; // Mặc định danh mục mới là hoạt động
            return await _categoryRepository.CreateCategory(category);
        }

        public async Task<Category?> UpdateCategory(Category category)
        {
            var existingCategory = await _categoryRepository.GetCategoryById(category.CategoryId);
            if (existingCategory == null || existingCategory.IsActive == false)
            {
                return null;
            }
            return await _categoryRepository.UpdateCategory(category);
        }

        public async Task<bool> DeleteCategory(short categoryId)
        {
            return await _categoryRepository.DeleteCategory(categoryId);
        }
        public bool CategoryExists(short id)
        {
            return _categoryRepository.CategoryExists(id);
        }
    }
}