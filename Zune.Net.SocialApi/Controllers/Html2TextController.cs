using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Zune.Net.SocialApi.Controllers
{
    [Route("/html2text.ashx")]
    [ApiController]
    public class Html2TextController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public Html2TextController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [Route("touFragments/LiveTOU_{locale}.htm")]
        public ActionResult<string> LiveTOU(string locale)
        {
            string path = Path.Combine(_env.ContentRootPath, "Resources", "LiveTOU.html");
            string html = System.IO.File.ReadAllText(path);

            return Content(new HtmlToText().Convert(html), "text/plain");
        }
    }
}
