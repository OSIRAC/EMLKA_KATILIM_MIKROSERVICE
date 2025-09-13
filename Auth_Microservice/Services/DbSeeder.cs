using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class DbSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var _manager = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            if (!_manager.User.Select().Any(u => u.Username == "manager"))
            {
                var manager = new User
                {
                    Username = "manager",
                    Active = true
                };
                manager.HashPassword = _passwordHasher.HashPassword(manager, "123456");

                var roleEntity = _manager.Role.Select().FirstOrDefault(r => r.Name == "Manager");
                var userRole = _manager.Role.Select().FirstOrDefault(r => r.Name == "User");
                var employeeRole = _manager.Role.Select().FirstOrDefault(r => r.Name == "Employee");

                if (roleEntity != null)
                    manager.Role.Add(new UserRole { RoleId = roleEntity.Id });

                if (userRole != null)
                    manager.Role.Add(new UserRole { RoleId = userRole.Id });

                if (employeeRole != null)
                    manager.Role.Add(new UserRole { RoleId = employeeRole.Id });

                _manager.User.Create(manager);
                _manager.Save();
            }
        }
    }
}
