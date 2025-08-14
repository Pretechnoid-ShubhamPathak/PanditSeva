using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PanditSeva.Identity.IdentityServices;
using System.Net;

namespace PanditSeva.Core.Controllers
{
    public class EncryptedJwtMiddleware 
    {
        private readonly RequestDelegate _next;

        public EncryptedJwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, EncryptedJwtService jwtService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                var principal = jwtService.DecryptToken(token);
                if (principal != null)
                {
                    context.User = principal;
                }
            }else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.Headers.Append("Content-Type", "application/json");
                await context.Response.WriteAsJsonAsync("{\n\"message\" : \"Authorization has been denied for the request.\"\n}");
            }

            await _next(context);
        }
    }

}
