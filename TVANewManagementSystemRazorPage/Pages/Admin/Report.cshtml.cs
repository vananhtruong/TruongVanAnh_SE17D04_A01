using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interfaces;

namespace TVANewManagementSystemRazorPage.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ReportModel : PageModel
    {
        private readonly IReportService _reportService;

        public ReportModel(IReportService reportService)
        {
            _reportService = reportService;
        }

        public IEnumerable<NewsArticle> NewsArticles { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public IActionResult OnGet()
        {
            // Display the initial form without data
            return Page();
        }

        public IActionResult OnPost(DateTime startDate, DateTime endDate)
        {
            // Validate dates
            if (startDate > endDate)
            {
                TempData["Error"] = "Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.";
                return RedirectToPage("Report");
            }

            // Generate report data
            NewsArticles = _reportService.GenerateReport(startDate, endDate);
            StartDate = startDate;
            EndDate = endDate;

            return Page();
        }
    }
}
