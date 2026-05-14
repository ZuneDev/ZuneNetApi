using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Zune.Net.Helpers;

namespace Zune.Net.Catalog.Image
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
            
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", Extensions.PermissiveCorsPolicy);
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            MusicBrainz.Initialize(env);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Append("Expires", "Sun, 19 Apr 2071 10:00:00 GMT");
                context.Response.Headers.Append("Keep-Alive", "timeout=150000, max=10");
                await next.Invoke();
            });

            app.UseCors("AllowAll");

            app.UseRequestBuffering();

            app.UseCommonRouting();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHomeRoute();
            });
        }
    }
}
