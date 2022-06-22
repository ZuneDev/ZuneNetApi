using Microsoft.AspNetCore.Builder;

namespace Zune.Net.Middleware
{
    public class LocalizationPipeline
    {
        public void Configure(IApplicationBuilder app, RequestLocalizationOptions options)
        {
            app.UseRequestLocalization(options);
        }
    }

}
