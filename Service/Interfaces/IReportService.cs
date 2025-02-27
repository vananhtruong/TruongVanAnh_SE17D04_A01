using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IReportService
    {
        List<NewsArticle> GenerateReport(DateTime startDate, DateTime endDate);
    }
}
