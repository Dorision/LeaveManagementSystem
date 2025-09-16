using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories.Interfaces;

namespace LeaveManagementSystem.Services;

public interface IPublicHolidayService
{
    Task<List<DateTime>> GetHolidaysInRangeAsync(DateTime startDate, DateTime endDate);
    Task<bool> IsWorkingDayAsync(DateTime date);
}

public class PublicHolidayService : IPublicHolidayService
{
    private readonly IUnitOfWork _unitOfWork;

    public PublicHolidayService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<DateTime>> GetHolidaysInRangeAsync(DateTime startDate, DateTime endDate)
    {
        var holidays = await _unitOfWork.PublicHolidays.FindAsync(h => 
            (h.IsRecurring && 
             (h.Date.Month >= startDate.Month && h.Date.Day >= startDate.Day) && 
             (h.Date.Month <= endDate.Month && h.Date.Day <= endDate.Day)) ||
            (!h.IsRecurring && h.Date >= startDate && h.Date <= endDate));

        var result = new List<DateTime>();
        var currentYear = startDate.Year;
        var endYear = endDate.Year;

        while (currentYear <= endYear)
        {
            foreach (var holiday in holidays)
            {
                var holidayDate = holiday.IsRecurring 
                    ? new DateTime(currentYear, holiday.Date.Month, holiday.Date.Day)
                    : holiday.Date;

                if (holidayDate >= startDate && holidayDate <= endDate)
                {
                    result.Add(holidayDate);
                }
            }
            currentYear++;
        }

        return result.OrderBy(d => d).ToList();
    }

    public async Task<bool> IsWorkingDayAsync(DateTime date)
    {
        // Check if it's weekend
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            return false;
        }

        // Check if it's a holiday
        var isHoliday = await _unitOfWork.PublicHolidays.ExistsAsync(h => 
            (h.IsRecurring && h.Date.Month == date.Month && h.Date.Day == date.Day) ||
            (!h.IsRecurring && h.Date.Date == date.Date));

        return !isHoliday;
    }
}