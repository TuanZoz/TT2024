using Microsoft.EntityFrameworkCore;
using AutoMapper;
using WebBanHang.Models;
using WebBanHang;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<QuanLyBanHangOnline4Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Hshop"));
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Th?i gian h?t h?n
    options.Cookie.HttpOnly = true;                // Ch? cho phép truy c?p qua HTTP
    options.Cookie.IsEssential = true;             // B?t bu?c cho ho?t ??ng c?a ?ng d?ng
});
builder.Services.AddScoped<SessionActionFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession(); // Kích ho?t Session Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
