
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zune.DB;
using Zune.Net.Middleware;
using Zune.Xml.Commerce;

namespace CommerceZuneNet.Controllers
{
    [Route("/{version}/{language}/tuner")]
    [Route("/{language}/tuner")]
    public class TunerController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ZuneNetContext _database;
        public TunerController(ILogger<TunerController> logger, ZuneNetContext database)
        {
            _logger = logger;
            _database = database;
        }

        [HttpPost("GetRegistrationInfo")]
        // [Produces("application/xml")]
        [Consumes("application/x-www-form-urlencoded","application/xml")]
        [Produces("application/xml")]
        public ActionResult GetRegistrationInfo()
        {
            if (this.TryGetAuthedMember(out var member))
            {
                _logger.LogInformation($"Session has been associated with: {member.UserName}");
                //var unused = new GetTunerRegistrationInfoResponse();
                return Ok(@"
                            <GetTunerRegistrationInfoResponse>
                                <RegisteredTuners>
                                    <MediaTypeTunerPair>
                                    </MediaTypeTunerPair>
                                </RegisteredTuners>
                            </GetTunerRegistrationInfoResponse>
                            ");
            }

            _logger.LogError("Failed to get a user for this session.");
            return Unauthorized();
        }

    }
}