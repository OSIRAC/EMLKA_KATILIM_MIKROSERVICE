using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Redis
{
    public class RedisSession
    {
        private readonly IDistributedCache _distributedCache;
        public RedisSession(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public bool IsTokenValid(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var userId = jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var jti = jwt.Claims.First(c => c.Type == "jti").Value;

            var storedJti = _distributedCache.GetString($"session:{userId}");
            if (storedJti == null)
                return false;
            if (storedJti != jti)
                return false;
            return true;
        }
    }
}
