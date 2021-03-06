﻿using System.Threading.Tasks;
using Microsoft.Owin;

namespace Foundation.Api.Middlewares
{
    public class ReadAuthTokenFromCookieMiddleware : OwinMiddleware
    {
        public ReadAuthTokenFromCookieMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Headers != null && !context.Request.Headers.ContainsKey("Authorization"))
            {
                if (context.Request.Cookies?["access_token"] != null)
                    context.Request.Headers.Add("Authorization", new[]
                    {
                        $"{context.Request.Cookies["token_type"]} {context.Request.Cookies["access_token"]}"
                    });
            }

            await Next.Invoke(context);
        }
    }
}