using LeaveManagementSystem.DTOs;
using LeaveManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto model)
    {
        var result = await _authService.LoginAsync(model);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

/*     [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto model)
    {
        var result = await _authService.RegisterAsync(model);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    } */
}