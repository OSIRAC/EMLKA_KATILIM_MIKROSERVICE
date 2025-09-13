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
        private IAccountRepository _accountRepository;
        public UnitOfWork(RepositoryContext context)
        {
            _context = context;
        }
        public IAccountRepository Account
        {
            get
            {
                if (_accountRepository == null)
                    _accountRepository = new AccountRepository(_context);

                return _accountRepository;
            }
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
