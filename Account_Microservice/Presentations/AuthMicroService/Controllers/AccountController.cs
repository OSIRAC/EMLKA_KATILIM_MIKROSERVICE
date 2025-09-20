using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Contracts;
using System.Security.Claims;

namespace AuthMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _userService;

        public AccountController(IAccountService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpPost("create")]
        public IActionResult Create()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                _userService.CreateAsync(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-account/{AccountId}")]
        public IActionResult GetAccount([FromRoute] int AccountId)
        {
            try
            {   
                var account = _userService.GetById(AccountId);
                if (account == null)
                    return NotFound("Hesap bulunamadı.");
                return Ok(account);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("delete-account")]
        public IActionResult DeleteAccount()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                _userService.DeleteAccount(userId);
                return Ok(new { message = "Hesabınız pasif edildi. Çıkış yapılıyor." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
