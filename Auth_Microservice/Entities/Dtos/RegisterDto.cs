using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class RegisterDto
    {
        public string? Username { get; set; }
        public string? HashPassword { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? RoleName { get; set; } = "User";
    }
}
