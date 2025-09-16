using LeaveManagementSystem.DTOs;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LeaveManagementSystem.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto model);
    Task<AuthResponseDto> RegisterAsync(RegisterDto model);
}

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto model)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(model.Email);
        if (user == null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!isPasswordValid)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        var roles = (await _unitOfWork.Users.GetUserRolesAsync(user)).ToList();
        var token = _tokenService.GenerateToken(user, roles);

        return new AuthResponseDto
        {
            Success = true,
            Token = token,
            Roles = roles
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto model)
    {
        if (model.Password != model.ConfirmPassword)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Passwords do not match"
            };
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "User with this email already exists"
            };
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            ManagerId = model.ManagerId
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = result.Errors.First().Description
            };
        }

        // Assign the Employee role by default
        await _userManager.AddToRoleAsync(user, "Employee");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        return new AuthResponseDto
        {
            Success = true,
            Token = token,
            Roles = roles
        };
    }
}