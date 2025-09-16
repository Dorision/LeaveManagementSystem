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
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork, 
        ITokenService tokenService, 
        UserManager<ApplicationUser> userManager,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto model)
    {
        // Log the login attempt
        _logger.LogInformation("Login attempt for email: {Email}", model.Email);

        var user = await _unitOfWork.Users.GetByEmailAsync(model.Email);
        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found for email {Email}", model.Email);
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        _logger.LogInformation("User found: {UserId}, checking password", user.Id);
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Login failed: Invalid password for user {UserId}", user.Id);
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        _logger.LogInformation("Password valid for user {UserId}, retrieving roles", user.Id);
        var roles = (await _unitOfWork.Users.GetUserRolesAsync(user)).ToList();
        _logger.LogInformation("User {UserId} has roles: {Roles}", user.Id, string.Join(", ", roles));
        
        var token = _tokenService.GenerateToken(user, roles);
        _logger.LogInformation("Generated token for user {UserId}", user.Id);

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
            ManagerId = string.IsNullOrEmpty(model.ManagerId) ? null : model.ManagerId
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