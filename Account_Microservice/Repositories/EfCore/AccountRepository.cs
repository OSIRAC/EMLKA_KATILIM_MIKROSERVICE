using Entities.Models;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EfCore
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(RepositoryContext context) : base(context)
        {

        }
        public Account GetOneAccount(int ?id)
        {
            var account = Select().Where(x => x.Id == id && x.Active).SingleOrDefault();
            return account;
        }
    }
}
