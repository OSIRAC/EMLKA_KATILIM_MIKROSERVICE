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
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AccountService : IAccountService
    {

        private readonly IUnitOfWork _manager;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        public AccountService(IUnitOfWork manager, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _manager = manager;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        public async Task CreateAsync(int Id)
        {
            //Account account = _mapper.Map<Account>(dto);
            Account account = new Account();
            account.UserId = Id;
            account.CreatedDate = DateTime.Now;
            account.UpgratedDate = DateTime.Now;
            _manager.Account.Create(account);
            _manager.Save();

            var client = _httpClientFactory.CreateClient();
            var userId = Id;

            var response = await client.PostAsync(
             $"http://localhost:5220/api/Auth/assign-role?userId={Id}&roleName=Customer", null);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Role atama başarısız!");
            }
        }

        public void DeleteAccount(int Id)
        {
            var account = _manager.Account.Select().SingleOrDefault(u => u.Id == Id);

            if (account == null)
                throw new Exception("Account bulunamadı");

            account.Active = false;
            _manager.Save();
        }

        public Account GetById(int id)
        {
            var entity = _manager.Account.GetOneAccount(id);
            if (entity == null)
                throw new Exception("Account bulunamadı");
            return _mapper.Map<Account>(entity);
        }

        public IEnumerable<Account> GetAll()
        {
            var entity = _manager.Account.Select();
            return _mapper.Map<IEnumerable<Account>>(entity);
        }
    }
}
