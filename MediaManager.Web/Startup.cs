using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MediaManager.Web.Data;
using MediaManager.Web.Data.Entities;
using MediaManager.Web.Models;
using MediaManager.Web.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MediaManager.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite("Filename=C:/Users/Raphael/Documents/Programming/C#/Console/MediaManager/MediaManager.Web/MediaManager.Web.db"));

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole<long>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.LoginPath = "/auth/unauthorized";
                options.LogoutPath = "/auth/logout";
            });

            services.AddSingleton<RolesConfiguration>();
            services.AddSingleton<TwitterAppConfiguration>();
            services.AddSingleton<TwitterService>();
                
            services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddTwitter(
                    options =>
                    {
                        options.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
                        options.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
                
                        options.SaveTokens = true;
                    });

            AddMediaManagerComponents(services);
        }

        private void AddMediaManagerComponents(IServiceCollection services)
        {
            IPostOperationValidator validator = new PostOperationValidator();
            services.AddSingleton(validator);

            IProvidersOperator @operator = new ProvidersOperator();
            services.AddSingleton(@operator);
            
            IPostsChecker postsChecker = new PostsChecker(validator, @operator);
            services.AddSingleton(postsChecker);

            var intervalSeconds = Convert.ToDouble(
                Configuration["MediaManager:WatchIntervalSeconds"]);
            
            IPostsWatcher postsWatcher = new PostsWatcher(
                TimeSpan.FromSeconds(intervalSeconds),
                postsChecker);
            services.AddSingleton(postsWatcher);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

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
