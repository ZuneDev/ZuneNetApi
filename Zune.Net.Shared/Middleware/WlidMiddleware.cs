using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Zune.DB;
using Zune.DB.Models;
using Zune.Net.LiveAuth;

namespace Zune.Net.Middleware
{
    public class WlidMiddleware
    {
        internal const string AUTHED_MEMBER_KEY = "Member";
        private readonly RequestDelegate _next;

        public WlidMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ZuneNetContext database)
        {
            Member authedMember = null;
            var authHeader = context.Request.Headers.Authorization;

            if (authHeader.Count > 0)
            {
                string authHeaderValue = authHeader[0]["WLID1.0 t=".Length..];
                if (!string.IsNullOrWhiteSpace(authHeaderValue) && database != null
                    && TokenStore.Current.TryGetCidForToken(authHeaderValue, out var cid))
                {
                    authedMember = await database.GetSingleAsync(m => m.Cid == cid);
                }
            }

            if (authedMember != null)
                context.Items.Add(AUTHED_MEMBER_KEY, authedMember);

            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }

    public static class WlidMiddlewareExtensions
    {
        public static IApplicationBuilder UseWlidAuthorization(this IApplicationBuilder app)
        {
            return app.UseMiddleware<WlidMiddleware>();
        }

        public static bool TryGetAuthedMember(this ControllerBase controller, out Member member)
        {
            bool isSuccess = controller.HttpContext.Items.TryGetValue(WlidMiddleware.AUTHED_MEMBER_KEY, out var obj);
            member = obj as Member;
            return isSuccess;
        }
    }
}
