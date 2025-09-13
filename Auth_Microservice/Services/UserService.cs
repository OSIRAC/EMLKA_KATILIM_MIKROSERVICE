using AutoMapper;
using Entities.Dtos;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserService : IUserService
    {

        private readonly IUnitOfWork _manager;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        public UserService(IUnitOfWork manager, IMapper mapper, IPasswordHasher<User> passwordHasher)
        {
            _manager = manager;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public void Register(RegisterDto dto)
        {

            if (dto.HashPassword != dto.ConfirmPassword)
                throw new Exception("Şifreler uyuşmuyor.");

            if (_manager.User.Select().Any(u => u.Username == dto.Username))
                throw new Exception("Username already exists");

            var user = _mapper.Map<User>(dto);

            user.HashPassword = _passwordHasher.HashPassword(user, dto.HashPassword);

            var roleEntity = _manager.Role.Select().FirstOrDefault(r => r.Name == "User");
            if (roleEntity == null)
                throw new Exception("Rol bulunamadı");

            user.Role.Add(new UserRole
            {
                RoleId = roleEntity.Id,
                UserId = user.Id
            });
            _manager.User.Create(user);
            _manager.Save();
        }

        public User Login(LoginDto dto)
        {
            var entity = _mapper.Map<User>(dto);

            var user = _manager.User.Select()
                            .Include(u => u.Role)
                            .ThenInclude(ur => ur.Role)
                            .SingleOrDefault(x => x.Username == dto.Username && x.Active);

            var result = _passwordHasher.VerifyHashedPassword(user, user.HashPassword, dto.HashPassword);
            if (result == PasswordVerificationResult.Success)
            {         
                return user;
            }
            return null;
        }

        public void DeleteUser(int userId)
        {
            var user = _manager.User.Select().SingleOrDefault(u => u.Id == userId);

            if (user == null)
                throw new Exception("Kullanıcı bulunamadı");

            user.Active = false;
            _manager.Save();
        }

        public User GetById(int id)
        {
            var entity = _manager.User.GetOneUser(id);
            return _mapper.Map<User>(entity);
        }

        public IEnumerable<User> GetAll()
        {
            var entity = _manager.User.Select();
            return _mapper.Map<IEnumerable<User>>(entity);
        }

        public void AssignRoleToUser(int userId, string roleName)
        {
            var user = _manager.User.GetOneUser(userId);

            var userRole = _manager.User
                                .Select()
                                .Include(u => u.Role)
                                .FirstOrDefault(u => u.Id == userId);

            var role = _manager.Role.Select().FirstOrDefault(x => x.Name == roleName);

            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

            if (role == null)
                throw new Exception("Rol bulunamadı.");

            var exists = userRole.Role.Any(r => r.RoleId == role.Id);

            if (exists)
                throw new Exception("Kullanıcı zaten bu role sahip.");

            user.Role.Add(new UserRole()
            {
                UserId = user.Id,
                RoleId = role.Id
            });

            _manager.User.Update(user);
            _manager.Save();
        }

        public void RemoveRoleToUser(int userId, string roleName)
        {
            var user = _manager.User.GetOneUser(userId);
            var role = _manager.Role.Select().FirstOrDefault(x => x.Name == roleName);

            if (user == null || role == null)
                throw new Exception();

            var User = _manager.User.Select().Include(x => x.Role).FirstOrDefault(x => x.Id == userId);

            var role_user = User.Role.FirstOrDefault(x => x.RoleId == role.Id);

            User.Role.Remove(role_user);
            _manager.Save();
        }

    }
}
