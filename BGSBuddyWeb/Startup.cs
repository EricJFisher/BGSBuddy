using Interfaces.Repositories;
using Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BGSBuddyWeb
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
            services.AddScoped<IEliteBgsRepository, EliteBgsRepository>();
            services.AddScoped<IAssetsService>(s => new AssetsService((IEliteBgsRepository)s.GetService(typeof(IEliteBgsRepository))));
            services.AddScoped<IFactionsService>(s => new FactionsService((IEliteBgsRepository)s.GetService(typeof(IEliteBgsRepository))));
            services.AddScoped<ISolarSystemsService>(s => new SolarSystemsService((IEliteBgsRepository)s.GetService(typeof(IEliteBgsRepository))));
            services.AddScoped<ITickService>(s => new TickService((IEliteBgsRepository)s.GetService(typeof(IEliteBgsRepository))));
            services.AddScoped<ISituationReportsService>(s =>
                new SituationReportsService(
                    (IAssetsService)s.GetService(typeof(IAssetsService)),
                    (IFactionsService)s.GetService(typeof(IFactionsService)),
                    (ISolarSystemsService)s.GetService(typeof(ISolarSystemsService)),
                    (ITickService)s.GetService(typeof(ITickService))));
            services.AddControllersWithViews();
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
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Expansion}/{id?}");
            });
        }
    }
}
