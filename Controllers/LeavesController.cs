using LeaveManagementSystem.DTOs;
using LeaveManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeaveManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all endpoints
public class LeavesController : ControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;

    public LeavesController(ILeaveRequestService leaveRequestService)
    {
        _leaveRequestService = leaveRequestService;
    }

    [HttpPost]
    [Authorize(Policy = "EmployeePolicy")]
    public async Task<ActionResult<LeaveRequestDto>> CreateLeaveRequest(CreateLeaveRequestDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _leaveRequestService.CreateAsync(userId, dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "EmployeePolicy")]
    public async Task<ActionResult<LeaveRequestDto>> UpdateLeaveRequest(Guid id, UpdateLeaveRequestDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _leaveRequestService.UpdateAsync(userId, id, dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/retract")]
    [Authorize(Policy = "EmployeePolicy")]
    public async Task<ActionResult<LeaveRequestDto>> RetractLeaveRequest(Guid id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _leaveRequestService.RetractAsync(userId, id);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/decision")]
    [Authorize(Policy = "ManagerPolicy")]
    public async Task<ActionResult<LeaveRequestDto>> MakeDecision(Guid id, LeaveRequestDecisionDto dto)
    {
        try
        {
            var managerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _leaveRequestService.MakeDecisionAsync(managerId, id, dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("mine")]
    [Authorize(Policy = "EmployeePolicy")]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetMyLeaveRequests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _leaveRequestService.GetEmployeeRequestsAsync(userId);
        return Ok(result);
    }

    [HttpGet("subordinates")]
    [Authorize(Policy = "ManagerPolicy")]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetSubordinateLeaveRequests()
    {
        var managerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _leaveRequestService.GetSubordinateRequestsAsync(managerId);
        return Ok(result);
    }

    [HttpGet("workdays")]
    [Authorize(Policy = "EmployeePolicy")]
    public async Task<ActionResult<int>> CalculateWorkdays([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var workdays = await _leaveRequestService.CalculateWorkdaysAsync(startDate, endDate);
            return Ok(new { workdays });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}