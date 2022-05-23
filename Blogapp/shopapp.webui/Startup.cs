using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using shopapp.business.Abstract;
using shopapp.business.Concrete;
using shopapp.data.Abstract;
using shopapp.data.Concrete.EfCore;
using shopapp.webui.EmailServices;
using shopapp.webui.Identity;
using shopapp.webui.Services;

namespace shopapp.webui
{
    public class Startup
    {
        private IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
       
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<RedisService>();
            services.AddMemoryCache();
             
            services.AddDbContext<ApplicationContext>(options=> options.UseSqlite("Data Source=shopDb"));
            services.AddIdentity<User,IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options=>
            {
                // şifre
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase= true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength=7;
                options.Password.RequireNonAlphanumeric= false;

                //Hesap kitleme
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.AllowedForNewUsers = true;

                // Kullanıcı Bilgileri Onay
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail= true;
                options.SignIn.RequireConfirmedPhoneNumber = false;

            });

            services.ConfigureApplicationCookie(options=>
            {
                options.LoginPath ="/account/login";
                options.LogoutPath ="/account/logout";
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true,
                    Name = ".BlogApp.Security.Cookie",
                    SameSite = SameSiteMode.Strict
                };
            });

            services.AddScoped<IProductRepository,EfCoreProductRepository>();
            services.AddScoped<ICategoryRepository,EfCoreCategoryRepository>();
            services.AddScoped<ICartRepository,EfCoreCartRepository>();
            
            services.AddScoped<IProductService,ProductManager>();
            services.AddScoped<ICategoryService,CategoryManager>();
            services.AddScoped<ICartService,CartManager>();

            services.AddScoped<IEmailSender,SmtpEmailSender>(i=>
             new SmtpEmailSender(
                 _configuration["EmailSender:Host"],
                 _configuration.GetValue<int>("EmailSender:Port"),
                 _configuration.GetValue<bool>("EmailSender:EnableSSl"),
                 _configuration["EmailSender:UserName"],
                 _configuration["EmailSender:Password"])
                 );
            services.AddControllersWithViews();
        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration, UserManager<User> userManager, RoleManager<IdentityRole> roleManager,RedisService redisService)
        {
            
            app.UseStaticFiles(); 

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(),"node_modules")),
                    RequestPath="/modules"                
            });

            if (env.IsDevelopment())
            {
                SeedDatabase.Seed();
                app.UseDeveloperExceptionPage();
            }
            
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            redisService.Connect();

            

            app.UseEndpoints(endpoints =>
            {
                 endpoints.MapControllerRoute(
                    name:"cart",
                    pattern: "ReadLater",
                    defaults: new {Controller="Cart",action="Index"}
                );
                endpoints.MapControllerRoute(
                    name:"adminuseredit",
                    pattern: "admin/user/{id?}",
                    defaults: new {Controller="Admin",action="UserEdit"}
                );
                endpoints.MapControllerRoute(
                    name:"adminusers",
                    pattern: "admin/user/list",
                    defaults: new {Controller="Admin",action="UserList"}
                );
                endpoints.MapControllerRoute(
                    name:"adminroles",
                    pattern: "admin/role/list",
                    defaults: new {Controller="Admin",action="RoleList"}
                );
                endpoints.MapControllerRoute(
                    name:"adminrolecreate",
                    pattern: "admin/role/create",
                    defaults: new {Controller="Admin",action="RoleCreate"}
                );
                endpoints.MapControllerRoute(
                    name:"adminroleedit",
                    pattern: "admin/role/{id?}",
                    defaults: new {Controller="Admin",action="RoleEdit"}
                );
                
                endpoints.MapControllerRoute(
                    name:"adminblogs",
                    pattern: "admin/blogs",
                    defaults: new {Controller="Admin",action="ProductList"}
                );

                endpoints.MapControllerRoute(
                    name:"adminblogcreate",
                    pattern: "admin/blogs/create",
                    defaults: new {Controller="Admin",action="ProductCreate"}
                );

                endpoints.MapControllerRoute(
                    name:"adminblogedit",
                    pattern: "admin/blogs/{id}",
                    defaults: new {Controller="Admin",action="ProductEdit"}
                );

                endpoints.MapControllerRoute(
                    name:"admincategories",
                    pattern: "admin/categories",
                    defaults: new {Controller="Admin",action="CategoryList"}
                );
                
                endpoints.MapControllerRoute(
                    name:"admincategorycreate",
                    pattern: "admin/categories/create",
                    defaults: new {Controller="Admin",action="CategoryCreate"}
                );
                endpoints.MapControllerRoute(
                    name:"admincategoryedit",
                    pattern: "admin/categories/{id}",
                    defaults: new {Controller="Admin",action="CategoryEdit"}
                );
                
                endpoints.MapControllerRoute(
                    name:"search",
                    pattern: "search",
                    defaults: new {Controller="Read",action="Search"}
                );

                endpoints.MapControllerRoute(
                    name:"blogdetails",
                    pattern: "{Url}",
                    defaults: new {Controller="Read",action="Details"}
                );


                endpoints.MapControllerRoute(
                    name:"blogs",
                    pattern: "blogs/{category?}",
                    defaults: new {Controller="Read",action="list"}
                );


                endpoints.MapControllerRoute(
                    name: "default",
                    pattern:"{controller=Home}/{action=Index}/{id?}"
                );
            });
        
            SeedIdentity.Seed(userManager,roleManager,configuration).Wait();
        
        
        }

    }
}
