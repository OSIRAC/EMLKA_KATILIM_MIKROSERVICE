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
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly IUserService _userService;

        public AuthController(JwtTokenGenerator tokenGenerator, IUserService userService)
        {
            _tokenGenerator = tokenGenerator;
            _userService = userService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto model)
        {
            try
            {
                _userService.Register(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto model)
        {
            var user = _userService.Login(model);
            if (user == null)
            {
                return Unauthorized();
            }

            var roleNames = user.Role.Select(ur => ur.Role.Name).ToList();

            var token = _tokenGenerator.GenerateToken(user.Username, roleNames, user.Id);
            return Ok(new { token });
        }

        [HttpPost("assign-role")]
        public IActionResult AssignRoleToUser([FromQuery] int userId, [FromQuery] string roleName)
        {
            try
            {
                _userService.AssignRoleToUser(userId, roleName);
                return Ok("Rol atandı.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }

        }

        [Authorize(Roles = "Manager")]
        [HttpPost("remove-role")]
        public IActionResult RemoveRoleToUser([FromQuery] int userId, [FromQuery] string roleName)
        {
            try
            {
                _userService.RemoveRoleToUser(userId, roleName);
                return Ok("Rol atandı.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("delete-account")]
        public IActionResult DeleteAccount()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                _userService.DeleteUser(userId);
                return Ok(new { message = "Hesabınız pasif edildi. Çıkış yapılıyor." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
