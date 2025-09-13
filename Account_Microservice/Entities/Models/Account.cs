using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Account
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int Balance { get; set; }

        public bool Active { get; set; } = true;

        public DateTime CreatedDate { get; set; }

        public DateTime UpgratedDate { get; set; }

    }
}
