using Microsoft.AspNetCore.Mvc;
using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult<ApiResponse<T>> ApiResponse<T>(T data, string? message = null)
    {
        return Ok(Models.ApiResponse<T>.CreateSuccess(data, message));
    }

    protected ActionResult<ApiResponse<T>> ApiError<T>(string message, List<string>? errors = null, int statusCode = 400)
    {
        var response = Models.ApiResponse<T>.CreateError(message, errors);
        return StatusCode(statusCode, response);
    }
}