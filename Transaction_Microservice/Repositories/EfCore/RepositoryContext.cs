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
        public DbSet<Transaction> Transactions { get; set; }
  
        public RepositoryContext(DbContextOptions options) : base(options)
        {

        }
    }
}
