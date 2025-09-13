using Microsoft.Extensions.DependencyInjection;
using Repositories.Contracts;
using Repositories.EfCore;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Entities.Mapping;
using AutoMapper;
using Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace Services
{
    public static class ServiceRegistration
    {
        public static void AddRepositoryService(this IServiceCollection Services, IConfiguration configuration)
        {
            Services.AddDbContext<RepositoryContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            Services.AddScoped<IAccountService, AccountService>();
            Services.AddHttpClient();
            Services.AddScoped<JwtTokenGenerator>();
            Services.AddAutoMapper(typeof(MapProfile).Assembly);
            Services.AddHostedService<AccountConsumerService>();
            Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
            });
        }
    }
}
