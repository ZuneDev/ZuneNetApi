using Microsoft.AspNetCore.Mvc;

namespace Zune.Net.MetaServices.Controllers
{
    [ApiController]
    [Route("/ZuneAPI/")]
    public class ZuneApi : ControllerBase
    {
        private readonly ILogger<ZuneApi> _logger;

        public ZuneApi(ILogger<ZuneApi> logger)
        {
            _logger = logger;
        }

        [HttpGet, Route("EndPoints.aspx"), Route("EndPoints")]
        public IActionResult EndPoints()
        {
            return Content(@"<METADATA>
    <ENDPOINTS>
        <ENDPOINT/>
    </ENDPOINTS>
</METADATA>", Atom.Constants.XML_MIMETYPE);
        }
    }
}