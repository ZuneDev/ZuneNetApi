using Microsoft.AspNetCore.Mvc;
using Zune.Net.MetaServices.DomainModels.Endpoints;

namespace Zune.Net.MetaServices.Controllers
{
    [ApiController]
    [Route("/redir/")]
    
    public class Redir : ControllerBase{
        
        [HttpGet("ZuneFAI/")]
        [Produces("application/xml")]
        public IActionResult ZuneFai()
        {
            return Ok(new Metadata());
        }

        //eg: mdrcdposturlbackgroundzune
        [HttpGet("get{mdrcd}")]
        [Produces("text/plain")]
        public string MDRCDdir(string mdrcd)
        {
            // ZuneNativeLib does this request. It takes this string and seemingly blindly appends &requestId.
            // the rest of the string is pretty useless but i don't know enough about it yet.
            return $"http://metaservices.zune.net/MDRCD/{mdrcd}{HttpContext.Request.QueryString.ToUriComponent()}";
        }
    }
}