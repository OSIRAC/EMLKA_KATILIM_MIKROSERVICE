using Microsoft.AspNetCore.Http;
using Services.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class JwtMiddleWare
    {
        private readonly RequestDelegate _next;
        public JwtMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, RedisSession session)
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
