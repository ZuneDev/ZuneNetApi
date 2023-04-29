using Microsoft.AspNetCore.Mvc;
using Zune.Net.MetaServices.DomainModels.Endpoints;

namespace Zune.Net.MetaServices.Controllers
{
    [ApiController]
    [Route("/redir/")]
    [Produces("application/xml")]
    public class Redir : ControllerBase{
        [HttpGet("ZuneFAI/")]
        public IActionResult ZuneFai()
        {
            return Ok(new Metadata());
        }
    }
}