namespace LeaveManagementSystem.Models;

public class PublicHoliday
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string Name { get; set; } = null!;
    public bool IsRecurring { get; set; }
}