using Microsoft.AspNetCore.Http;
using Services.Contracts;
using Services.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context, Redis_Session session)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                if (!session.IsTokenValid(token))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Tekrar Login Olmalısın");
                    return;
                }
            }
            await _next(context);
        }
    }
}
