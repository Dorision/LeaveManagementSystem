namespace LeaveManagementSystem.Models;

public class LeaveRequest
{
    public Guid Id { get; set; }
    
    public string EmployeeId { get; set; } = null!;
    public virtual ApplicationUser Employee { get; set; } = null!;
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public int DaysRequested { get; set; }
    
    public LeaveRequestStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? DecisionAt { get; set; }
    
    public string? DecisionById { get; set; }
    public virtual ApplicationUser? DecisionBy { get; set; }
    
    public string? Comment { get; set; }
}