using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CS3750_PlanetExpressLMS.Data;

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
/*            services.AddDbContext<CS3750_PlanetExpressLMSContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("CS3750_PlanetExpressLMSContext")));*/
            services.AddDbContext<CS3750_PlanetExpressLMSContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));
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
