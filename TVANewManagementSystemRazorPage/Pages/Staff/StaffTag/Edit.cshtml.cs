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

namespace TVANewManagementSystemRazorPage.Pages.Staff.StaffTag
{
    public class EditModel : PageModel
    {
        private readonly ITagService _tagService;

        public EditModel(ITagService tagService)
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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _tagService.UpdateTag(Tag);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TagExists(Tag.TagId))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToPage("Index");
        }

        private async Task<bool> TagExists(int id)
        {
            var tag = await _tagService.GetTagById(id);
            return tag != null;
        }
    }
}
