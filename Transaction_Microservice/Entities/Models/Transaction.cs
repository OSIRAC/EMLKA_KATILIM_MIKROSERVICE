using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public int? TargetAccountId { get; set; }

        public int Amount { get; set; }

        public bool Active { get; set; } = true;

        public Transaction_Status Status { get; set; }

        public Transaction_Type Type { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
