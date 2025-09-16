using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Repositories.Interfaces;

public interface ILeaveRequestRepository : IGenericRepository<LeaveRequest>
{
    Task<IEnumerable<LeaveRequest>> GetEmployeeRequestsAsync(string employeeId);
    Task<IEnumerable<LeaveRequest>> GetSubordinateRequestsAsync(string managerId);
    Task<IEnumerable<LeaveRequest>> GetOverlappingRequestsAsync(string employeeId, DateTime startDate, DateTime endDate);
}