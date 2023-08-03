using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;

namespace GroceryList
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // IConfiguration'� al�n
            var configuration = builder.Configuration;

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Session yap�land�rmas�n� ekleyin
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum s�resini ihtiyac�n�za g�re ayarlayabilirsiniz.
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Yap�land�rmalar� yap�n
            var webHostEnvironment = app.Services.GetRequiredService<IWebHostEnvironment>();

            var webRootPath = webHostEnvironment.WebRootPath;

            // Configure the HTTP request pipeline.
            if (!webHostEnvironment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Session kullan�m�n� etkinle�tirin
            app.UseSession();

            app.UseAuthentication();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=it}/{action=login}/{id?}");

            app.Run();
        }
    }
}
