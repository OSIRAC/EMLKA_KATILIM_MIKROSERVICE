using Azure.Core;
using Entities.Dtos;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Contracts;
using System.Security.Claims;

namespace AuthMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        
        public TransactionController(ITransactionService transactionrService)
        {
            _transactionService = transactionrService;     
        }
        [Authorize]
        [HttpPost("operation")]
        public async Task<IActionResult> Deposit([FromBody] TransactionDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                await _transactionService.Deposit(dto, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("get-transaction")]
        public IActionResult GetTransaction([FromRoute] int Id)
        {
            try
            {
                _transactionService.GetById(Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("delete-transaction")]
        public IActionResult DeleteAccount()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                _transactionService.DeleteTransaction(userId);
                return Ok(new { message = "Hesabınız pasif edildi. Çıkış yapılıyor." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
