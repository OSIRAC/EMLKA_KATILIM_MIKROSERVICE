using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class TransactionDto
    {
        public int AccountId { get; set; }

        public int? TargetAccountId { get; set; }

        public int Amount { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Transaction_Type Type { get; set; }

    }
}
