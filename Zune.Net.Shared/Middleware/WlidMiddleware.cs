using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Zune.DB;
using Zune.DB.Models;

namespace Zune.Net.Middleware
{
    public class WlidMiddleware
    {
        internal const string AUTHED_MEMBER_KEY = "Member";
        internal const string WLID_SESSION_ID = "WLID-SESSIONID";
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
                var token = authHeader[0];

                // This is how the Escargot project grabs the session, presumably it's reliable enough?
                // WLID1.0 t=[0-20] - session ID
                if(!string.IsNullOrWhiteSpace(token) && token.StartsWith("WLID1.0 t=") && database != null)
                {
                    var idxToken = token.IndexOf(' ');
                    if (idxToken >= 0)
                    {
                        var start = idxToken + 3;
                        var stop = idxToken + 23;
                        token = token[start..stop];
                        context.Items.Add(WLID_SESSION_ID, token);
                    }
                
                    authedMember = await database.GetMemberByToken(token);
                }
            } 
        
            if (authedMember != null)
            {
                context.Items.Add(AUTHED_MEMBER_KEY, authedMember);
                // holy cow I wish I had an ILogger here.
                Console.WriteLine($"Authed Member: {authedMember.ZuneTag}");
            }

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

        public static bool TryGetSessionId(this ControllerBase controller, out string wlid_session_id)
        {
            bool isSuccess = controller.HttpContext.Items.TryGetValue(WlidMiddleware.WLID_SESSION_ID, out var obj);
            wlid_session_id = obj as string;
            return isSuccess;
        }
    }
}
