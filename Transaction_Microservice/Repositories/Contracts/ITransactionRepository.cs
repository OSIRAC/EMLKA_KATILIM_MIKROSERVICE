using Entities.Models;
using Repositories.EfCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface ITransactionRepository : IRepositoryBase<Transaction>
    {
        Transaction GetOneTransaction(int id);
    }
}
