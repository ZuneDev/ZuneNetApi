using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Zune.Net.Tiles.Controllers
{
    [Route("/tiles/{typeStr}/{zuneTag}")]
    [ApiController]
    public class TilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        enum TileType : byte
        {
            Background,
            Avatar
        }

        public TilesController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet, HttpPost]
        public ActionResult GetTile(string typeStr, string zuneTag)
        {
            if (!Enum.TryParse(typeStr, true, out TileType type))
                return NotFound();

            // Perform some validation to prevent attacks
            if (zuneTag.Contains(Path.DirectorySeparatorChar))
                return BadRequest();
            if (Path.GetExtension(zuneTag) != ".jpg")
                zuneTag += ".jpg";

            string path = Path.Combine(_env.ContentRootPath, "Assets", type.ToString(), zuneTag);
            if (Request.Method == "GET")
            {
                if (System.IO.File.Exists(path))
                    return PhysicalFile(path, "image/jpeg");
                else
                    return NotFound();
            }
            else if (Request.Method == "POST")
            {
                throw new NotImplementedException();
            }

            return BadRequest();
        }
    }
}
