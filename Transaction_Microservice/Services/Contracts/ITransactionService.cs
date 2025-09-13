using Entities.Dtos;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface ITransactionService
    {
        Transaction GetById(int id);

        IEnumerable<Transaction> GetAll();

        public Task Deposit(TransactionDto dto, int userId);

        public void DeleteTransaction(int Id);
    }
}
