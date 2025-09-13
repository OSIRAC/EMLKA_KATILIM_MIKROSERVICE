using Entities.Models;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EfCore
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository(RepositoryContext context) : base(context)
        {

        }
        public Transaction GetOneTransaction(int id)
        {
            var account = Select().Where(x => x.Id == id).SingleOrDefault();
            return account;
        }
    }
}
