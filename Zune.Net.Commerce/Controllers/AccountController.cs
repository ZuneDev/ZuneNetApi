using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CommerceZuneNet.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zune.DB;
using Zune.Net.Middleware;
using Zune.Xml.Commerce;

namespace CommerceZuneNet.Controllers
{
    [Route("/{version}/{language}/account/{action=SignIn}")]
    [Route("/{language}/account/{action=SignIn}")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ZuneNetContext _database;
        private readonly ILogger _logger;
        public AccountController(ZuneNetContext database, ILogger<AccountController> logger)
        {
            _logger = logger;
            _database = database;
        }

        [HttpPost]
        [Produces("application/xml")]
        [Consumes("application/x-www-form-urlencoded","application/xml")]
        public async Task<ActionResult<SignInResponse>> SignIn()
        {
            if (this.TryGetAuthedMember(out var member))
            {
                _logger.LogInformation($"Session has been associated with: {member.UserName}");
                return member.GetSignInResponse();
            }
            else
            {
                // Get the user by SID and tie the session to that user:
                try
                {
                    if(this.TryGetSessionId(out var sessionID))
                    {
                        _logger.LogInformation("We have a session ID, attempting to tie to a SID/User");

                        // parse the SID out of the body the hard way, since the asp.net core xml deserializer chokes on TunerInfo.
                        var body = await Request.GetRawBodyAsync();
                        _logger.LogDebug($"Got a body of: {body.ToString()}");
                        using var reader = new StringReader(body);
                        var serializer = new XmlSerializer(typeof(SignInRequest));
                        SignInRequest requestBody = null;
                        try
                        {
                            requestBody = (SignInRequest)serializer.Deserialize(reader);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e,"Failed to deserialize request");
                            return Reject();
                        }

                        if(requestBody != null)
                        {
                            // we have a sid, probably
                            var user_sid = requestBody.TunerInfo.ID;
                            if(string.IsNullOrEmpty(user_sid))
                            {
                                _logger.LogDebug("failed to get a SID from the request body");
                                return Reject();
                            }

                            _logger.LogDebug($"User SID: {user_sid}");

                            member = await _database.GetMemberBySid(user_sid);
                            if(member == null)
                            {
                                _logger.LogError($"Failed to find a member with SID: {user_sid}");
                                return Reject();
                            }
                            _logger.LogInformation("We got a user by SID");
                            await _database.AddToken(sessionID, member.UserName);
                            _logger.LogInformation("Session is associated with SID");
                            await _database.UpdateAsync(member);
                            _logger.LogInformation("Updating the database!");
                            member = await _database.GetMemberBySid(user_sid);

                            // TODO: We need to be adding the TunerInfo as a tuner to the db when we see a NEW one.

                            return member.GetSignInResponse();
                        } else 
                        {
                            _logger.LogInformation("No sid was recovered, rejecting the request");
                            return Reject();
                        }
                    }                    
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to get our user");
                }
            }

            return Reject();
        }

        private ActionResult<SignInResponse> Reject()
        {
            return Unauthorized(new SignInResponse
                {
                    AccountState = new AccountState
                    {
                        SignInErrorCode = 0x80070057,
                    }
                });
        }

        [HttpPost("User")]
        [Produces("application/xml")]
        [Consumes("application/x-www-form-urlencoded")]
        public ActionResult<GetUserResponse> ZuneUser()
        {
            return new(new GetUserResponse());
        }

        [HttpPost]
        public ActionResult<BalancesResponse> Balances()
        {
            return new BalancesResponse
            {
                Balances = new()
                {
                    PointsBalance = 8000,
                    SongCreditBalance = 0,
                    SongCreditRenewalDate = null
                }
            };
        }
    }
}
