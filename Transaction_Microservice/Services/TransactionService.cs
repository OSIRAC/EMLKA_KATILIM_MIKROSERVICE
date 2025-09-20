using AutoMapper;
using Entities.Dtos;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using RabbitMQ.Client;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text.Json;
using Microsoft.Identity.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Principal;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class TransactionService : ITransactionService
    {

        private readonly IUnitOfWork _manager;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        public TransactionService(IUnitOfWork manager, IMapper mapper, HttpClient httpClient,IHttpClientFactory httpClientFactory, IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _manager = manager;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        private void SaveTransaction(IUnitOfWork manager, Transaction transaction, Transaction_Status status)
        {
            transaction.Status = status;
            manager.Transaction.Create(transaction);
            manager.Save();
        }

        public async Task Deposit(TransactionDto dto, int userId)
        {
            using var scope = _scopeFactory.CreateScope();
            var _manager = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            Transaction transaction = _mapper.Map<Transaction>(dto);
            transaction.CreatedDate = DateTime.Now;
            transaction.Type = dto.Type;
            transaction.Status = Transaction_Status.Failed;

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(
                        $"http://accountservice:80/api/Account/get-account/{dto.AccountId}");
            try
            {    
                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException("Ana hesap bulunamadı.");          
            }
            catch
            {
                SaveTransaction(_manager, transaction, Transaction_Status.Failed);
                throw;
            }
            var account = await response.Content.ReadFromJsonAsync<AccountDto>();
            AccountDto? targetAccount = null;
            if (dto.Type == Transaction_Type.Transfer)
            {
                try
                {
                    if (!dto.TargetAccountId.HasValue)
                        throw new InvalidOperationException("Hedef hesap belirtilmeli.");

                    var target_response = await httpClient.GetAsync(
                                 $"http://accountservice:80/api/Account/get-account/{dto.TargetAccountId}");

                    if (!target_response.IsSuccessStatusCode)
                        throw new InvalidOperationException("Hedef hesap bulunamadı.");
                }
                catch
                {
                    SaveTransaction(_manager, transaction, Transaction_Status.Failed);
                    throw;
                }              
            }
            if (dto.Type == Transaction_Type.Deposit || dto.Type == Transaction_Type.Withdraw)
            {
                try
                {
                    if ((account.Balance < dto.Amount) && dto.Type == Transaction_Type.Withdraw)
                        throw new InvalidOperationException("Yetersiz bakiye.");

                    if (dto.TargetAccountId.HasValue)
                        throw new InvalidOperationException("TargetAccountId Alanı Gereksiz");

                    if (account.UserId != userId)
                        throw new InvalidOperationException("Sadece kendi hesabından para çeker ya da aktarabilirsin");
                }
                catch
                {
                    SaveTransaction(_manager, transaction, Transaction_Status.Failed);
                    throw;
                }
            }
            var rabbitConfig = _configuration.GetSection("RabbitMQ");

            var factory = new ConnectionFactory()
            {
                HostName = rabbitConfig["HostName"],
                UserName = rabbitConfig["UserName"],
                Password = rabbitConfig["Password"],
                Port = int.Parse(rabbitConfig["Port"])
            };

            IConnection connection = await factory.CreateConnectionAsync();
            IChannel channel = await connection.CreateChannelAsync();
            
            await channel.ExchangeDeclareAsync(
                    exchange: "account-exchange",
                    type: ExchangeType.Direct               
                );

            await channel.QueueDeclareAsync(queue: "account-deposit-queue", exclusive: false);
            await channel.QueueDeclareAsync(queue: "account-withdraw-queue", exclusive: false);
            await channel.QueueDeclareAsync(queue: "account-transfer-queue", exclusive: false);

            var message = new
            {
                AccountId = dto.AccountId,
                TargetAccountId = dto.TargetAccountId,
                Amount = dto.Amount,
                Type = dto.Type
            };
            string jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            string routingKey = message.Type.ToString();

            await channel.BasicPublishAsync(
                exchange: "account-exchange",
                routingKey: routingKey,
                body: body
            );
            SaveTransaction(_manager, transaction, Transaction_Status.Success);
        }

        public Transaction GetById(int id)
        {
            var entity = _manager.Transaction.GetOneTransaction(id);
            if (entity == null)
                throw new Exception("Account bulunamadı");
            return _mapper.Map<Transaction>(entity);
        }

        public IEnumerable<Transaction> GetAll()
        {
            var entity = _manager.Transaction.Select();
            return _mapper.Map<IEnumerable<Transaction>>(entity);
        }

        public void DeleteTransaction(int Id)
        {
            var account = _manager.Transaction.Select().SingleOrDefault(u => u.Id == Id);

            if (account == null)
                throw new Exception("Account bulunamadı");

            account.Active = false;
            _manager.Save();
        }
    }
}
