using CS3750_PlanetExpressLMS.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CS3750_PlanetExpressLMS
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
            services.AddRazorPages();
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            services.AddMemoryCache();
            //services.AddMvc();
            services.AddScoped<IUserRepository, SQLUserRepository>();
            services.AddScoped<ICourseRepository, SQLCourseRepository>();
            services.AddScoped<IEnrollmentRepository, SQLEnrollmentRepository>();
            services.AddScoped<IInvoiceRepository, SQLInvoiceRepository>();
            services.AddScoped<IPaymentRepository, SQLPaymentRepository>();
            services.AddScoped<IAssignmentRepository, SQLAssignmentRepository>();
            services.AddScoped<ISubmissionRepository, SQLSubmissionRepository>();

            services.AddScoped<INotificationRepository, SQLNotificationRepository>();

            services.AddDbContext<CS3750_PlanetExpressLMSContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("CS3750_PlanetExpressLMSContext")));
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

            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller}/{action=Index}/{id?}");
            //});
        }
    }
}
