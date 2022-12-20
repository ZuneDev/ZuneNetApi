using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Zune.Net.Login.Controllers
{
    [Route($"/{PPCRLCONFIG_NAME}")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        internal const string PPCRLCONFIG_NAME = "ppcrlconfig.bin";

        private readonly IWebHostEnvironment _env;

        public ConfigController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public ActionResult Get()
        {
            string path = Path.Combine(_env.ContentRootPath, "Resources", PPCRLCONFIG_NAME);
            return PhysicalFile(path, "application/octet-stream", PPCRLCONFIG_NAME, true);
        }
    }
}
