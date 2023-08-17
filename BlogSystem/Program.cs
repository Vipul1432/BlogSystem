using BlogSystem.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BlogSystem.Data;
using BlogSystem.Data.Repository;
using BlogSystem.Domain.Interfaces;
using BlogSystem.Domain.Models;
using BlogSystem.Data.UnitOfWork;

namespace BlogSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                // enable lazy loading
                options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("blogDbEntities"));
            });

            // Dependency Injection
            builder.Services.AddScoped<IRepository<Blog>, Repository<Blog>>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

           

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}