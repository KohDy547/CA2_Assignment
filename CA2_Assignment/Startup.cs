using CA2_Assignment.Configurations;
using CA2_Assignment.Repositories.CscRepositories;
using CA2_Assignment.Services;
using CA2_Ultima.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe;

namespace CA2_Ultima
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region ConfigureServices - Configure DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection")));
            #endregion
            #region ConfigureServices - Configure Identity Server
            services.AddDefaultIdentity<IdentityUser>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            });
            #endregion
            #region ConfigureServices - Set Compatibility Version & Security
            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Csc");
                    options.Conventions.AllowAnonymousToPage("/Csc/Index");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            #endregion
            #region ConfigureServices - Configure Cookie Policies
            services.AddSession();
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.IsEssential = true;
            });
            #endregion
            #region ConfigureServices - Require HTTPS
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });
            #endregion

            #region ConfigureServices - Configure Settings
            services.Configure<App_SharedSettings>(Configuration.GetSection("App_SharedSettings"));

            services.Configure<Csc_AwsS3Settings>(Configuration.GetSection("Csc_AwsS3Settings"));
            services.Configure<Csc_StripeSettings>(Configuration.GetSection("Csc_StripeSettings"));
            services.Configure<Csc_GoogleAuthSettings>(Configuration.GetSection("Csc_GoogleAuthSettings"));
            #endregion
            #region ConfigureServices - Inject Dependencies
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IAwsService, AwsService>();
            services.AddSingleton<IEmailSender, EmailService>();

            services.AddScoped<ITalentsRepository, TalentsRepository>();
            #endregion
            #region ConfigureServices - Configure Google Authentication
            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration.GetSection("CSC_GoogleAuthSettings")["ClientId"];
                googleOptions.ClientSecret = Configuration.GetSection("CSC_GoogleAuthSettings")["ClientSecret"];
            });
            #endregion
            #region ConfigureServices - RequireHTTPS
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });
            #endregion 

        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            #region Configure - Configure Stripe Settings
            StripeConfiguration.SetApiKey(Configuration.GetSection("CSC_StripeSettings")["SecretKey"]);
            #endregion
            #region Configure - Define Settings
            app.UseHttpsRedirection();

            app.UseCookiePolicy();
            app.UseSession();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc();
            #endregion
        }
    }
}
