using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EfCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RepositoryContext _context;
        private ITransactionRepository _transactionRepository;
        public UnitOfWork(RepositoryContext context)
        {
            _context = context;
        }
        public ITransactionRepository Transaction
        {
            get
            {
                if (_transactionRepository == null)
                    _transactionRepository = new TransactionRepository(_context);

                return _transactionRepository;
            }
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
