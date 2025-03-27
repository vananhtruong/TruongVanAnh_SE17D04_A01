using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TVANewManagementSystemRazorPage.Pages.Staff.StaffTag
{
    [Authorize(Roles = "Staff")]
    public class IndexModel : PageModel
    {
        private readonly ITagService _tagService;

        public IndexModel(ITagService tagService)
        {
            _tagService = tagService;
        }

        public IList<Tag> Tags { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Tags = await _tagService.GetAllTags();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var result = new { success = false, message = "Error deleting tag." };

            try
            {
                await _tagService.DeleteTag(id);
                result = new { success = true, message = "Tag deleted successfully." };
            }
            catch (Exception ex)
            {
                result = new { success = false, message = "this tag cannot delete because has news using this" };
            }

            return new JsonResult(result);
        }
    }
}
