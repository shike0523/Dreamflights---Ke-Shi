using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DreamFlights.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DreamFlights.Interface;
using DreamFlights.Dependencies;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using DreamFlights.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using DreamFlights.Data.Repositories;

namespace DreamFlights
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private const string enUSCulture = "en-AU";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            //.Net Core 2.1要加这一句才可以用role
            //services.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>()   
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddIdentity<IdentityUser, IdentityRole>()
            .AddDefaultUI()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            });

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });

            //设定身份验证自动跳转到某一个登陆页面("ctx.Response.Redirect("/Identity/Account/Login");")-redirect access denied login based on the URL on ASP.NET Core 2 Identity
            services.ConfigureApplicationCookie(options => {

                options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        
                        {
                            ctx.Response.Redirect("/Identity/Account/Login");
                        }

                        return Task.CompletedTask;
                    }
                };

            });
            //设定登陆用户的登陆时间长度
            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromSeconds(25);
            });

            services.AddDbContext<FlightContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FlightConnection")));

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddSessionStateTempDataProvider();   //引入TempData
            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN"); //为Jquery的ajax的post方法而设

            //Session;设定Session的时间寿命
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(45);
                options.Cookie.HttpOnly = true;
            });

            //services.AddMemoryCache();
            services.AddSession();
            //services.AddMvc();

            services.AddScoped<IScheduleSearching, ScheduleSearching>();
            services.AddScoped<IFlight_ScheduleRepository, Flight_ScheduleRepository>();

            //多语言
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();   // 注：往项目中加入多语言/文化的功能
            // 注：定义页面上的语言选择框的选项,以及往项目中加入多语言/文化的功能
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                            new CultureInfo(enUSCulture),
                        	//new CultureInfo("en-AU"), // 注：定义页面上的语言选择框的选项
                            //new CultureInfo("fr"),
                            new CultureInfo("en-US"),
                            new CultureInfo("zh")
                };
                options.DefaultRequestCulture = new RequestCulture(culture: enUSCulture, uiCulture: enUSCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddHostedService<TimedHostedService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, FlightContext context, IServiceProvider services, UserManager<IdentityUser> userManager)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //加了多语言之后,session就得在多语言之前声明了,不然session调用时会报错
            app.UseSession();

            //多语言
            var supportedCultures = new[]
                    {
                        new CultureInfo(enUSCulture),
                        new CultureInfo("en-US"),
                        new CultureInfo("en-GB"),
                        new CultureInfo("en"),
                        new CultureInfo("es-ES"),
                        new CultureInfo("es-MX"),
                        new CultureInfo("es"),
                        new CultureInfo("fr-FR"),
                        new CultureInfo("fr"),   // 注：该CutureInfol对象与Resources文件夹相对应,没有它会导致多语种功能失效,从而无法选择多语言
                        new CultureInfo("zh-Hant"),
                        new CultureInfo("zh-Hans"),
                        new CultureInfo("zh"),
                    };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(enUSCulture),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });
            //以下app.XXXXX的顺序不能乱,不然会导致以上部分功能无效(引入的service太多所导致)
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();

            app.UseHttpsRedirection();
            //app.UseStaticFiles();
            app.UseCookiePolicy();            
            //app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            DbInitializer.Initialize(context);

            var context2 = services.GetRequiredService<ApplicationDbContext>();
            DbInitializer.InitializeRoles(context2);
            CreateUserRoles(services, userManager).Wait();
        }

        private async Task CreateUserRoles(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var _userManager = userManager;

            IdentityResult roleResult, roleResult2, roleResult3;
            //Adding Admin Role 
            var roleCheck = await RoleManager.RoleExistsAsync("SuperAdmin");
            if (!roleCheck)
            {
                //create the roles and seed them to the database 
                roleResult = await RoleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                roleResult2 = await RoleManager.CreateAsync(new IdentityRole("RouteManager"));
                roleResult3 = await RoleManager.CreateAsync(new IdentityRole("ScheduleManager"));
            }

            if (!_userManager.Users.Any())
            {
                var approver = new IdentityUser { UserName = "SuperAdmin@gmail.com", Email = "SuperAdmin@gmail.com" };
                await _userManager.CreateAsync(approver, "Sk123!");
                var editor = new IdentityUser { UserName = "RouteManager@gmail.com", Email = "RouteManager@gmail.com" };
                await _userManager.CreateAsync(editor, "Sk123!");
                var editor2 = new IdentityUser { UserName = "RouteManager2@gmail.com", Email = "RouteManager2@gmail.com" };
                await _userManager.CreateAsync(editor2, "Sk123!");
                var dataMaintenaner = new IdentityUser { UserName = "ScheduleManager@gmail.com", Email = "ScheduleManager@gmail.com" };
                await _userManager.CreateAsync(dataMaintenaner, "Sk123!");
                var dataMaintenaner2 = new IdentityUser { UserName = "ScheduleManager2@gmail.com", Email = "ScheduleManager2@gmail.com" };
                await _userManager.CreateAsync(dataMaintenaner2, "Sk123!");
            }

            //Assign Admin role to the main User here we have given our newly registered  
            //login id for Admin management 
            IdentityUser user = await UserManager.FindByEmailAsync("SuperAdmin@gmail.com");
            IdentityUser user2 = await UserManager.FindByEmailAsync("RouteManager@gmail.com");
            IdentityUser user3 = await UserManager.FindByEmailAsync("RouteManager2@gmail.com");
            IdentityUser user4 = await UserManager.FindByEmailAsync("ScheduleManager@gmail.com");
            IdentityUser user5 = await UserManager.FindByEmailAsync("ScheduleManager2@gmail.com");

            var User = new IdentityUser();
            if (user != null)
            {
                await UserManager.AddToRoleAsync(user, "SuperAdmin");
            }
            if (user2 != null)
            {
                await UserManager.AddToRoleAsync(user2, "RouteManager");
            }
            if (user3 != null)
            {
                await UserManager.AddToRoleAsync(user3, "RouteManager");
            }
            if (user4 != null)
            {
                await UserManager.AddToRoleAsync(user4, "ScheduleManager");
            }
            if (user5 != null)
            {
                await UserManager.AddToRoleAsync(user5, "ScheduleManager");
            }
        }
    }
}
