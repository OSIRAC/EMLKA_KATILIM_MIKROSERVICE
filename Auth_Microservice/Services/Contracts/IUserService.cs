using Entities.Dtos;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IUserService
    {
        void Register(RegisterDto userDto);
        User Login(LoginDto dto);

        User GetById(int id);

        IEnumerable<User> GetAll();

        void DeleteUser(int userId);

        void AssignRoleToUser(int userId, string roleName);

        void RemoveRoleToUser(int userId, string roleName);

        public void Logout(int userId);
    }
}
