using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategories();
        Task<Category?> GetCategoryById(short categoryId);
        Task<Category> CreateCategory(Category category);
        Task<Category?> UpdateCategory(Category category);
        Task<bool> DeleteCategory(short categoryId);
        bool CategoryExists(short id);
    }
}