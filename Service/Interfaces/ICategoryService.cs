using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategories();
        Task<Category?> GetCategoryById(short categoryId);
        Task<Category> CreateCategory(Category category);
        Task<Category?> UpdateCategory(Category category);
        Task<bool> DeleteCategory(short categoryId);
        bool CategoryExists(short id);
    }
}