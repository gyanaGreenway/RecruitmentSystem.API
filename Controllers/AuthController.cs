using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentSystem.API.DTOs;
using RecruitmentSystem.API.Services;
using System.Security.Claims;

using RecruitmentSystem.API.Models;


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

    [HttpPost("password-reset/start")]
    public async Task<ActionResult<StartPasswordResetResponseDto>> StartPasswordReset([FromBody] StartPasswordResetDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.StartPasswordResetAsync(dto);
        return Ok(result);
    }

    [HttpPost("password-reset/verify-otp")]
    public async Task<ActionResult<VerifyOtpResponseDto>> VerifyOtp([FromBody] VerifyOtpDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var result = await _authService.VerifyOtpAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("password-reset/confirm")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var ok = await _authService.ResetPasswordAsync(dto);
        if (!ok) return BadRequest(new { message = "Invalid or expired reset token." });
        return NoContent();
    }

    [Authorize]
    [HttpPost("change-password")] // self-service change (Candidate and HR)
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();
        var ok = await _authService.ChangePasswordAsync(userId, dto);
        if (!ok) return BadRequest(new { message = "Current password is incorrect or user not found." });
        return NoContent();
    }

    [Authorize(Roles = "HR")]
    [HttpPost("admin/change-password")] // HR/admin changes a user's password
    public async Task<ActionResult> AdminChangePassword([FromBody] AdminChangePasswordDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var ok = await _authService.AdminChangePasswordAsync(dto);
        if (!ok) return NotFound(new { message = "User not found." });
        return NoContent();
    }


    [HttpPost("register/candidate")] // public candidate self-registration
    public async Task<ActionResult<RegisterUserResponseDto>> RegisterCandidate([FromBody] RegisterUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var result = await _authService.RegisterAsync(dto, UserRole.Candidate);
            return CreatedAtAction(nameof(Login), new { email = result.Email }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "HR")]
    [HttpPost("register/hr")] // HR creates another HR account
    public async Task<ActionResult<RegisterUserResponseDto>> RegisterHr([FromBody] RegisterUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var result = await _authService.RegisterAsync(dto, UserRole.HR);
            return CreatedAtAction(nameof(Login), new { email = result.Email }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

}
