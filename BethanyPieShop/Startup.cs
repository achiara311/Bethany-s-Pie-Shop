using BethanyPieShop.Models;
using BethanysPieShop.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanyPieShop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //add this connection string in here AFTER creating AppDbContext and installing EF
            services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<AppDbContext>();
            //AddEntityFrameworkStores<AppDbContext>() indicates that Identity needs to use Entity
            //Framework to store its data and it's going to use AppDbContext, which inherits
            //from IdentityDbContext to do so

           
            services.AddScoped<IPieRepository, PieRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            //services.AddTransient -- going to give back a new instance every time you ask for one
            //services.AddSingleton() -- going to create a single instance for the entire application
            //and reuse that single instance
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddScoped<ShoppingCart>(serviceProvider => ShoppingCart.GetCart(serviceProvider));
            //GetCart() method is going to be invoked when the user sends a request
            //that gives me the ability to check if the cart ID is already in the session,
            //if not, I pass the cart ID into this session and I return the ShoppingCart itself in the GetCart() method
            //at the very bottom of it.
            //This way, I'm sure that when a user comes to the site, a shopping cart will
            //be associated with that request. 
            //And since it's scoped, it means it all interacts with that same shopping cart,
            //within that SAME request, we'll use that SAME ShoppingCart

            services.AddHttpContextAccessor(); //add in a services.AddHttpContextAccessor to have it work
            //with GetCart() in ShoppingCart.cs

            //afterwards..
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            //add below so that it will bring in correct middleware
            app.UseSession(); //THIS NEEDS TO GO BEFORE APP.USEROUTING()

            app.UseRouting();
            //add this in below after putting IdentityDbContext in AppDbXontext file
            app.UseAuthentication();

            app.UseAuthorization();
     


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapRazorPages();
            });
        }
    }
}
