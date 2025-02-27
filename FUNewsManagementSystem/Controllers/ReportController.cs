using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace FUNewsManagementSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // Hiển thị form ban đầu
        public IActionResult Report()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GenerateReport(DateTime startDate, DateTime endDate)
        {
            // Kiểm tra ngày hợp lệ
            if (startDate > endDate)
            {
                TempData["Error"] = "Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.";
                return RedirectToAction("Report");
            }

            var reportData = _reportService.GenerateReport(startDate, endDate);
            return View("Report", reportData);
        }
    }
}