using Microsoft.AspNetCore.Mvc;

namespace Zune.Net.MetaServices.Controllers
{
    [Route("/Cover/")]
    public class Cover : Controller
    {
        [Route("{coverId}")]
        public async Task<ActionResult> GetCover(Guid coverId)
        {
            return NotFound();
        }
    }
}