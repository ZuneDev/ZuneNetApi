using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Zune.Net;
using Zune.Net.Middleware;

namespace CommerceZuneNet
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
            services.AddControllersWithViews(o => o.UseZestFormatters());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("CommerceZuneNet", new OpenApiInfo { Title = "CommerceZuneNet", Version = "v2" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Config cfg)
        {
            LoggerFactory.Create(loggerFactory =>
            {
                loggerFactory.AddConsole();
                loggerFactory.AddDebug();
            });

            app.UseStatusCodePages();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Commerce.Zune.Net v2");
                    c.RoutePrefix = string.Empty;
                    //c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                });
            }

            app.UseRequestBuffering();

            // app.UseHttpsRedirection();

            app.UseRouting();
            app.UseWlidAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHomeRoute();
            });
        }
    }
}
