using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TVANewManagementSystemRazorPage.Pages.Staff.Categories
{
    [Authorize(Roles = "Staff")]
    public class EditModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public EditModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [BindProperty]
        public Category Category { get; set; }

        [BindProperty]
        public bool IsActiveCheckbox { get; set; } // Thuộc tính trung gian cho checkbox

        public List<Category> Categories { get; set; }

        // GET: Hiển thị form chỉnh sửa
        public async Task<IActionResult> OnGetAsync(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category = await _categoryService.GetCategoryById(id.Value);
            if (Category == null)
            {
                return NotFound();
            }

            Categories = await _categoryService.GetAllCategories();
            IsActiveCheckbox = Category.IsActive ?? false; // Ánh xạ từ bool? sang bool

            return Page();
        }

        // POST: Xử lý khi submit form
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Categories = await _categoryService.GetAllCategories(); // Tải lại danh sách nếu validation thất bại
                return Page();
            }

            Category.IsActive = IsActiveCheckbox; // Ánh xạ từ bool sang bool?

            try
            {
                await _categoryService.UpdateCategory(Category);
            }
            catch (Exception)
            {
                
                throw;
            }

            return RedirectToPage("./Index");
        }
    }
}
