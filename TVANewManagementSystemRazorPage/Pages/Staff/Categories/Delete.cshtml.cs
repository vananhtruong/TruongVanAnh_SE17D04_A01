using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using Services.Interfaces;

namespace TVANewManagementSystemRazorPage.Pages.Staff.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public DeleteModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [BindProperty]
        public Category Category { get; set; }

        // GET: Hiển thị trang xác nhận xóa
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

            return Page();
        }

        // POST: Xử lý xóa danh mục
        public async Task<IActionResult> OnPostAsync()
        {
            if (Category == null || Category.CategoryId == 0)
            {
                return NotFound();
            }

            await _categoryService.DeleteCategory(Category.CategoryId);
            return RedirectToPage("./Index");
        }
    }
}
