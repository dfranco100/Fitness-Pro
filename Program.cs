using FitnessPro.Data;
using FitnessPro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FitnessPro.Services;
using FitnessPro.Services.Interfaces;
using FitnessPro.ViewModel;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;


namespace FitnessPro
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetSection("pgSettings")["pgConnection"];
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();



            builder.Services.AddIdentity<FitnessUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                  .AddDefaultTokenProviders()

                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();
            

            //custom services
            builder.Services.AddRazorPages();

            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<IGymManagmentService, GymManagmentService>();
            builder.Services.AddScoped<DataService>();

            builder.Services.AddTransient<IFitnessEmailSender, EmailService>();
            //Register a preconfigured instance of the MailSetting
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

            var app = builder.Build();
            //Register our custom DataService class          
            //Pull out my registered DataService
            using (var scope = app.Services.CreateScope())
            {
                var dataService = scope.ServiceProvider
                                      .GetRequiredService<DataService>();


                await dataService.ManageDataAsync();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. 
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Map Razor Pages
                endpoints.MapRazorPages();

                // Map default controller route
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            //app.MapControllerRoute(
            //name: "default",
            // pattern: "{controller=Home}/{action=Index}/{id?}");






            app.Run();
        }
    }
}