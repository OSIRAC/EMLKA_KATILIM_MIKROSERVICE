using Entities.Models;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EfCore
{
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        public RoleRepository(RepositoryContext context) : base(context)
        {

        }
        public Role GetOneRole(int id)
        {
            var role = Select().Where(x => x.Id == id).SingleOrDefault();
            return role;
        }
    }
}
