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
        private IUserRepository _userRepository;
        private IRoleRepository _roleRepository;
        public UnitOfWork(RepositoryContext context)
        {
            _context = context;
        }
        public IUserRepository User
        {
            get
            {
                if (_userRepository == null)
                    _userRepository = new UserRepository(_context);

                return _userRepository;
            }
        }
        public IRoleRepository Role
        {
            get
            {
                if (_roleRepository == null)
                    _roleRepository = new RoleRepository(_context);

                return _roleRepository;
            }
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
