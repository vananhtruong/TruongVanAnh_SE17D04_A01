using BusinessObjects.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CategoryDAO _categoryDAO;

        public CategoryRepository(CategoryDAO categoryDAO)
        {
            _categoryDAO = categoryDAO;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _categoryDAO.GetAllCategories();
        }

        public async Task<Category?> GetCategoryById(short categoryId)
        {
            return await _categoryDAO.GetCategoryById(categoryId);
        }

        public async Task<Category> CreateCategory(Category category)
        {
            return await _categoryDAO.CreateCategory(category);
        }

        public async Task<Category?> UpdateCategory(Category category)
        {
            return await _categoryDAO.UpdateCategory(category);
        }

        public async Task<bool> DeleteCategory(short categoryId)
        {
            return await _categoryDAO.DeleteCategory(categoryId);
        }
        public bool CategoryExists(short id)
        {
            return _categoryDAO.CategoryExists(id);    
        }
    }
}