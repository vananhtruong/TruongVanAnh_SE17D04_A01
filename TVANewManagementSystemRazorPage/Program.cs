using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories;
using Services.Interfaces;
using Services;
using Service.Interfaces;
using Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using DataAccessLayer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using TVANewManagementSystemRazorPage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<FunewsManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

builder.Services.AddScoped<SystemAccountDAO>();
builder.Services.AddScoped<CategoryDAO>();
builder.Services.AddScoped<NewsArticleDAO>();
builder.Services.AddScoped<TagDAO>();

builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

builder.Services.AddScoped<ISystemAccountService, SystemAccountService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<INewsArticleService, NewsArticleService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ITagService, TagService>();

builder.Services.AddSignalR();

// Configure authentication with Cookies and Google
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Set Google as the default challenge
})
.AddCookie(options =>
 {
     options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
     options.LoginPath = "/Login";  // Login page
     options.LogoutPath = "/Logout"; // Logout page
     options.AccessDeniedPath = "/AccessDenied"; // Access denied page
 })
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Use cookies to store the session
});

builder.Services.AddAuthorization();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthorization();
app.MapGet("/", (HttpContext http) =>
{
    http.Response.Redirect("/Login");
    return Task.CompletedTask;
});
app.MapHub<SignalrServer>("/signalrServer");
app.MapRazorPages();

app.Run();
