using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using Services.Interfaces;

namespace TVANewManagementSystemRazorPage.Pages.Staff.StaffTag
{
    public class DeleteModel : PageModel
    {
        private readonly ITagService _tagService;

        public DeleteModel(ITagService tagService)
        {
            _tagService = tagService;
        }

        [BindProperty]
        public Tag Tag { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Tag = await _tagService.GetTagById(id);

            if (Tag == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Tag == null || Tag.TagId == 0)
            {
                return NotFound();
            }

            try
            {
                await _tagService.DeleteTag(Tag.TagId);
                return RedirectToPage("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "This tag cannot be deleted because it is used by news articles.");
                return Page();
            }
        }
    }
}
