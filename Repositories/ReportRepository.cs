using BusinessObjects.Models;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ReportRepository: IReportRepository
    {
        private readonly FunewsManagementContext _context;
        public ReportRepository(FunewsManagementContext context)
        {
            _context = context;
        }
        public List<NewsArticle> GenerateReport(DateTime startDate, DateTime endDate)
        {
            return _context.NewsArticles
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .Include(n => n.CreatedBy)
                .Include(n => n.Category)
                .OrderByDescending(n => n.CreatedDate)
                .ToList();
        }
    }
}
