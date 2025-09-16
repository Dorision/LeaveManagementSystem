using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.DTOs;

public class LeaveRequestDto
{
    public Guid Id { get; set; }
    public string EmployeeId { get; set; } = null!;
    public string EmployeeName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DaysRequested { get; set; }
    public LeaveRequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DecisionAt { get; set; }
    public string? DecisionBy { get; set; }
    public string? Comment { get; set; }
}

public class CreateLeaveRequestDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Comment { get; set; }
}

public class UpdateLeaveRequestDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Comment { get; set; }
}

public class LeaveRequestDecisionDto
{
    public bool IsApproved { get; set; }
    public string? Comment { get; set; }
}