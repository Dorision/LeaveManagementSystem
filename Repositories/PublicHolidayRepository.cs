using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Repositories;

public class PublicHolidayRepository : GenericRepository<PublicHoliday>, IPublicHolidayRepository
{
    public PublicHolidayRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PublicHoliday>> GetHolidaysInRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(h => h.Date >= startDate && h.Date <= endDate)
            .ToListAsync();
    }

    public async Task<int> GetWorkingDaysAsync(DateTime startDate, DateTime endDate)
    {
        var holidays = await GetHolidaysInRangeAsync(startDate, endDate);
        var holidayDates = holidays.Select(h => h.Date.Date).ToHashSet();

        int workingDays = 0;
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && 
                date.DayOfWeek != DayOfWeek.Sunday && 
                !holidayDates.Contains(date))
            {
                workingDays++;
            }
        }

        return workingDays;
    }
}