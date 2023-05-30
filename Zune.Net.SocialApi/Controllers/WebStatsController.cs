using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Zune.Net.SocialApi.Controllers
{
    [ApiController]
    public class WebStatsController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public WebStatsController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // sometimes things crash when m.webtrends.net doesnt return a valid gif, for example m.webtrends.net/something/something.gif&abunch=of&args=that...

        [Route("/{gibberish}/{file}.gif")]
        public ActionResult<string> LiveTOU()
        {
            string path = Path.Combine(_env.ContentRootPath, "Resources", "1x1px.gif");
            var image = System.IO.File.ReadAllBytes(path);

            return File(image, "image/gif");
        }
    }
}
