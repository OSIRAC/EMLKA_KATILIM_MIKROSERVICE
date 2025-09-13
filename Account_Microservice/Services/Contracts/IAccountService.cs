using Entities.Dtos;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IAccountService
    {
        Task CreateAsync(int Id);

        Account GetById(int id);

        IEnumerable<Account> GetAll();

        void DeleteAccount(int Id);

    }
}
