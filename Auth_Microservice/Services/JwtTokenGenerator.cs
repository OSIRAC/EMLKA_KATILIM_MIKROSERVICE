using Entities.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _config;
        private readonly IDistributedCache _distributedCache;

        public JwtTokenGenerator(IConfiguration config,IDistributedCache distributedCache)
        {
            _config = config;
            _distributedCache = distributedCache;
        }
        public string GenerateToken(string UserName, List<string> roles, int Id)
        {
            var jti = Guid.NewGuid().ToString();

            var claims_list = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, UserName),
                new Claim(ClaimTypes.NameIdentifier,Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            foreach (var role in roles)
            {
                claims_list.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims_list,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds);

            _distributedCache.SetString($"session:{Id}", jti, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            });

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
