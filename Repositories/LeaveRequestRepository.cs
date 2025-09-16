using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Repositories;

public class LeaveRequestRepository : GenericRepository<LeaveRequest>, ILeaveRequestRepository
{
    public LeaveRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<LeaveRequest>> GetEmployeeRequestsAsync(string employeeId)
    {
        return await _dbSet
            .Include(r => r.Employee)
            .Include(r => r.DecisionBy)
            .Where(r => r.EmployeeId == employeeId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<LeaveRequest>> GetSubordinateRequestsAsync(string managerId)
    {
        return await _dbSet
            .Include(r => r.Employee)
            .Include(r => r.DecisionBy)
            .Where(r => r.Employee.ManagerId == managerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<LeaveRequest>> GetOverlappingRequestsAsync(string employeeId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(r => r.EmployeeId == employeeId &&
                       r.Status != LeaveRequestStatus.Rejected &&
                       ((r.StartDate <= startDate && r.EndDate >= startDate) ||
                        (r.StartDate <= endDate && r.EndDate >= endDate) ||
                        (r.StartDate >= startDate && r.EndDate <= endDate)))
            .ToListAsync();
    }
}