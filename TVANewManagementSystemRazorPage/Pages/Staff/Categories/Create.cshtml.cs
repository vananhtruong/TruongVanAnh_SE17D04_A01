using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObjects.Models;
using Services.Interfaces;

namespace TVANewManagementSystemRazorPage.Pages.Staff.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public CreateModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [BindProperty]
        public Category Category { get; set; }

        [BindProperty]
        public bool IsActiveCheckbox { get; set; } // Thuộc tính trung gian kiểu bool

        public List<Category> Categories { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Categories = await _categoryService.GetAllCategories();
            Category = new Category();
            IsActiveCheckbox = Category.IsActive ?? false; // Ánh xạ từ bool? sang bool
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Categories = await _categoryService.GetAllCategories();
                return Page();
            }

            Category.IsActive = IsActiveCheckbox; // Ánh xạ từ bool sang bool?
            await _categoryService.CreateCategory(Category);
            return RedirectToPage("./Index");
        }
    }
}
