// AuthController - API контроллер аутентификации.
// Обрабатывает запросы на вход (login), регистрацию (register) и выход (logout).
// Использует SessionManager для управления пользовательской сессией. Возвращает HTTP статусы с сообщениями об ошибках

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
            if (string.IsNullOrWhiteSpace(request.Phone) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { error = "Поля телефон и пароль обязательны к заполнению! . _." });

            if (_sessionManager.Login(request.Phone, request.Password))
            {
                var user = _sessionManager.CurrentUser;
                return Ok(new { userId = user!.Id, fullName = user.FullName });
            }

            return Unauthorized(new { error = "Братан, ты ошибся или с телефоном или с паролем" });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName) || 
                string.IsNullOrWhiteSpace(request.Phone) || 
                string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { error = "Поля имя, телефон и пароль обязательны к заполнению . _." });

            if (_sessionManager.Register(request.FullName, request.Phone, request.Password))
            {
                var user = _sessionManager.CurrentUser;
                return Ok(new { userId = user!.Id, fullName = user.FullName });
            }

            return BadRequest(new { error = "Упс, регистрация не прошла, бб . _." });
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
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}