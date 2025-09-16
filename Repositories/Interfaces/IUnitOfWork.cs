namespace LeaveManagementSystem.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ILeaveRequestRepository LeaveRequests { get; }
    IPublicHolidayRepository PublicHolidays { get; }
    IUserRepository Users { get; }
    
    Task<int> SaveChangesAsync();
}