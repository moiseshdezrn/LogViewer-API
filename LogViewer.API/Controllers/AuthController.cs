using LogViewer.Core.DTOs;
using LogViewer.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LogViewer.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.LoginAsync(request);

            if (response is null)
            {
                _logger.LogWarning("Failed login attempt for user: {Email}", request.Email);
                return Unauthorized(new { message = "Invalid credentials" });
            }

            _logger.LogInformation("User {Username} logged in successfully", request.Email);
            return Ok(response);
        }
    }
}
