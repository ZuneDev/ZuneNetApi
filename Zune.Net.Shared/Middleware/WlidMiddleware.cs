using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Zune.DB;
using Zune.DB.Models;

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
                string token = authHeader[0];
                int idxToken = token.IndexOf(' ');
                if (idxToken >= 0)
                    token = token[(idxToken + 1)..];

                if (!string.IsNullOrWhiteSpace(token) && database != null)
                {
                    authedMember = await database.GetMemberByToken(token);
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
