using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Repositories.Interfaces;

public interface IPublicHolidayRepository : IGenericRepository<PublicHoliday>
{
    Task<IEnumerable<PublicHoliday>> GetHolidaysInRangeAsync(DateTime startDate, DateTime endDate);
    Task<int> GetWorkingDaysAsync(DateTime startDate, DateTime endDate);
}