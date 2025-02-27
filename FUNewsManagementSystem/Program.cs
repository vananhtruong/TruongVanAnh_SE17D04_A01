using BusinessObjects.Models;
using Services;
using Services.Interfaces;
using Repositories;
using Repositories.Interfaces;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;
using Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Cấu hình DbContext với lifetime Scoped
builder.Services.AddDbContext<FunewsManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

// Đăng ký DAO với lifetime Scoped
builder.Services.AddScoped<SystemAccountDAO>();
builder.Services.AddScoped<CategoryDAO>();
builder.Services.AddScoped<NewsArticleDAO>();
builder.Services.AddScoped<TagDAO>();

// Đăng ký Repositories với lifetime Scoped
builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

// Đăng ký Services với lifetime Scoped
builder.Services.AddScoped<ISystemAccountService, SystemAccountService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<INewsArticleService, NewsArticleService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IReportService, ReportService>();
//sesssion
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Set session timeout
    options.Cookie.HttpOnly = true; // For security
    options.Cookie.IsEssential = true; // Ensure session cookie is always created
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=GuestNewsArticles}/{action=Index}/{id?}");

app.Run();