using AutoMapper;
using Entities.Dtos;
using Entities.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class AccountConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public AccountConsumerService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var rabbitConfig = _configuration.GetSection("RabbitMQ");

            var factory = new ConnectionFactory()
            {
                HostName = rabbitConfig["HostName"],
                UserName = rabbitConfig["UserName"],
                Password = rabbitConfig["Password"],
                Port = int.Parse(rabbitConfig["Port"])
            };

            IConnection ?connection = null;
            IChannel ?channel = null;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    connection = await factory.CreateConnectionAsync();
                    channel =  await connection.CreateChannelAsync();
                    Console.WriteLine("RabbitMQ bağlantısı başarılı!");
                    break;
                }
                catch
                {
                    Console.WriteLine("RabbitMQ bağlanılamadı, 5 saniye bekleniyor...");
                    await Task.Delay(5000, stoppingToken);
                }
            }

            await channel.ExchangeDeclareAsync(
                exchange: "account-exchange",
                type: ExchangeType.Direct
            );
            await channel.QueueDeclareAsync(queue: "account-deposit-queue", exclusive: false);
            await channel.QueueDeclareAsync(queue: "account-withdraw-queue", exclusive: false);
            await channel.QueueDeclareAsync(queue: "account-transfer-queue", exclusive: false);

            await channel.QueueBindAsync("account-deposit-queue", "account-exchange", "Deposit");
            await channel.QueueBindAsync("account-withdraw-queue", "account-exchange", "Withdraw");
            await channel.QueueBindAsync("account-transfer-queue", "account-exchange", "Transfer");

            var depositConsumer = new AsyncEventingBasicConsumer(channel);
            await channel.BasicConsumeAsync("account-deposit-queue", false, depositConsumer);
            depositConsumer.ReceivedAsync += async (sender, e) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _manager = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var transaction = JsonSerializer.Deserialize<TransactionDto>(Encoding.UTF8.GetString(e.Body.Span));
                var account = _manager.Account.GetOneAccount(transaction.AccountId);
                account.Balance += transaction.Amount;
                _manager.Account.Update(account);
                _manager.Save();
                await channel.BasicAckAsync(e.DeliveryTag, false);
                await Task.CompletedTask;
            };
            
            var withdrawConsumer = new AsyncEventingBasicConsumer(channel);
            await channel.BasicConsumeAsync("account-withdraw-queue", false, withdrawConsumer);
            withdrawConsumer.ReceivedAsync += async (sender, e) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _manager = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var transaction = JsonSerializer.Deserialize<TransactionDto>(Encoding.UTF8.GetString(e.Body.Span));
                var account = _manager.Account.GetOneAccount(transaction.AccountId);
                account.Balance -= transaction.Amount;
                _manager.Account.Update(account);
                _manager.Save();
                await channel.BasicAckAsync(e.DeliveryTag, false);
                await Task.CompletedTask;
            };
           
            var transferConsumer = new AsyncEventingBasicConsumer(channel);
            await channel.BasicConsumeAsync("account-transfer-queue", false, transferConsumer);
            transferConsumer.ReceivedAsync += async (sender, e) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _manager = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var transaction = JsonSerializer.Deserialize<TransactionDto>(Encoding.UTF8.GetString(e.Body.Span));
                var fromAccount = _manager.Account.GetOneAccount(transaction?.AccountId);
                var toAccount = _manager.Account.GetOneAccount(transaction?.TargetAccountId);
                fromAccount.Balance -= transaction.Amount;
                toAccount.Balance += transaction.Amount;
                _manager.Account.Update(fromAccount);
                _manager.Account.Update(toAccount);
                _manager.Save();
                await channel.BasicAckAsync(e.DeliveryTag, false);
                await Task.CompletedTask;
            };
            
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
