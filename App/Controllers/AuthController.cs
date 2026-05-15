using Microsoft.AspNetCore.Mvc;
using App.UI.Services;

namespace App.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly SessionManager _sessionManager;

        public AuthController(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Phone))
                return BadRequest(new { error = "Телефон обязателен" });

            if (_sessionManager.Login(request.Phone))
            {
                var user = _sessionManager.CurrentUser;
                return Ok(new { userId = user!.Id, fullName = user.FullName });
            }

            return Unauthorized();
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.Phone))
                return BadRequest(new { error = "Полное имя и телефон обязательны" });

            if (_sessionManager.Register(request.FullName, request.Phone))
            {
                var user = _sessionManager.CurrentUser;
                return Ok(new { userId = user!.Id, fullName = user.FullName });
            }

            return BadRequest(new { error = "Регистрация не пройдена . _." });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _sessionManager.Logout();
            return Ok();
        }
    }

    public class LoginRequest
    {
        public string Phone { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}