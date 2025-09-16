using Microsoft.AspNetCore.Identity;

namespace LeaveManagementSystem.Models;

public class ApplicationUser : IdentityUser
{
    public string? ManagerId { get; set; }
    
    // Navigation property for manager
    public virtual ApplicationUser? Manager { get; set; }
    
    // Navigation property for employees (inverse of Manager)
    public virtual ICollection<ApplicationUser> Employees { get; set; } = new List<ApplicationUser>();
    
    // Navigation property for leave requests
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    
    // Navigation property for leave requests as manager
    public virtual ICollection<LeaveRequest> ManagedLeaveRequests { get; set; } = new List<LeaveRequest>();
}