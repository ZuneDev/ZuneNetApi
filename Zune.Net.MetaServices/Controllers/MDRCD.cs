using Microsoft.AspNetCore.Mvc;
using Zune.Net.Helpers;
using Zune.Net.MetaServices.DomainModels.MdqCd;

namespace Zune.Net.MetaServices.Controllers
{
    [Route("MDRCD")]
    public class MDRCD : Controller
    {
        
        private readonly WMIS _wmis;
        private readonly ILogger _logger;
        public MDRCD(WMIS wmis, ILogger<FAI> logger)
        {
            _wmis = wmis;
            _logger = logger;
        }

        [HttpPost("mdrcdposturlbackgroundzune")]
        [Produces("application/xml")]
        public async Task<IActionResult> MdrCdBackground(Guid requestID, [FromBody]MdqRequestMetadata request)
        {
            if(request.MdqCd.Tracks.Count != 1)
            {
                return BadRequest($"Got {request.MdqCd.Tracks.Count} tracks, expected 1");
            }
            return Ok(await _wmis.GetMdrCdRequestFromTrackIdAsync(request.MdqCd.Tracks[0].trackRequestId, request.MdqCd.Tracks[0].TrackDurationMs, requestID));
        }

        [HttpGet("{type}")]
        [HttpPost("{type}")]
        public IActionResult Handle(string type)
        {
            return Ok(type);
        }

    }
}