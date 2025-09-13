using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class TransactionDto
    {
        public int UserId { get; set; }

        public int AccountId { get; set; }

        public int? TargetAccountId { get; set; }

        public int Amount { get; set; }

    }
}
