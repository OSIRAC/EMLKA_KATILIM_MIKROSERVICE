using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Role
    {
        public Role()
        {
            User = new HashSet<UserRole>();
        }

        public int Id { get; set; }

        public string? Name { get; set; }

        public ICollection<UserRole>? User { get; set; }
    }
}
