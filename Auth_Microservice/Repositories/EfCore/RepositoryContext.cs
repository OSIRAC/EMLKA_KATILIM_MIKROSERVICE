using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EfCore
{
    public class RepositoryContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
       
        public RepositoryContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                        new Role { Id = 1, Name = "Manager" },
                        new Role { Id = 2, Name = "Employee" },
                        new Role { Id = 3, Name = "Customer" },
                        new Role { Id = 4, Name = "User" });

            modelBuilder.Entity<UserRole>()
                .HasKey(ky => new
                {
                    ky.RoleId,
                    ky.UserId
                });          
        }
    }
}
