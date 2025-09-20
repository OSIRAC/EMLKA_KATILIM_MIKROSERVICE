
using Repositories.Contracts;
using Repositories.EfCore;
using Services.Contracts;
using Services;
using AuthMicroService.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Polly;
using Microsoft.Extensions.Caching.Distributed;

namespace AuthMicroService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Services.AddControllers()
                .AddApplicationPart(typeof(AuthController).Assembly);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddRepositoryService(builder.Configuration);

           
            var app = builder.Build();

            var policy = Policy
                    .Handle<SqlException>()
                    .WaitAndRetry(10, attempt => TimeSpan.FromSeconds(2));

            policy.Execute(() =>
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
                if (!db.Database.CanConnect())
                {
                    db.Database.Migrate();
                }
            });
            DbSeeder.Seed(app.Services);

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<JwtMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
           
            app.MapControllers();

            app.Run();
        }
    }
}