using Entities.Models;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EfCore
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(RepositoryContext context) : base(context)
        {

        }

        public User GetOneUser(int id)
        {
            var appuser = Select().Where(x => x.Id == id).SingleOrDefault();
            return appuser;
        }

    }
}
