using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Entities.Models
{
    public class User
    {

        public User()
        {   
            Role = new HashSet<UserRole>();
        }

        public int Id { get; set; }

        public string? Username { get; set; }

        public string? HashPassword { get; set; }

        public bool Active { get; set; } = true;

        public ICollection<UserRole> Role { get; set; }
    }
}
