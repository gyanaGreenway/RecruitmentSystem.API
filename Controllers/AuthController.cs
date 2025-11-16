using Microsoft.AspNetCore.Mvc;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Services;

namespace RecruitmentSystem.API.Controllers;

[ApiController]
[Route("recruitment/api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Login with email and password.
    /// Returns200 with LoginResponseDto when successful,401 when invalid.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(loginDto);
        if (result == null)
        {
            // Do not reveal whether email or password was incorrect
            return Unauthorized(new { message = "Invalid email or password" });
        }

        return Ok(result);
    }
}
