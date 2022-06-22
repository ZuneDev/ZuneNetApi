using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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
        public AccountController(ZuneNetContext database)
        {
            _database = database;
        }

        [HttpPost]
        public async Task<ActionResult<SignInResponse>> SignIn(SignInRequest request)
        {
            // TODO: Authentication is a mess. SignInRequest only provides a (device?)
            // ID that is only seen in this endpoint. After that, all subsequent
            // requests use a Zune tag or GUID to specify a member, and a WLID
            // token for authorization. Most Commerce endpoints just send the WLID,
            // which doesn't directly identify which user's data is being requested.

            SignInResponse response;
            if (this.TryGetAuthedMember(out var member))
            {
                response = member.GetSignInResponse();
            }
            else
            {
                // TODO: Work out the error response format
                response = new SignInResponse
                {
                    AccountState = new AccountState
                    {
                        SignInErrorCode = 0x80070057,
                    }
                };
                return Unauthorized(response);
            }

            return response;
        }

        [HttpPost]
        public ActionResult<GetUserResponse> User(GetUserRequest request)
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
