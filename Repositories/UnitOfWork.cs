using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LeaveManagementSystem.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private ILeaveRequestRepository? _leaveRequestRepository;
    private IPublicHolidayRepository? _publicHolidayRepository;
    private IUserRepository? _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public UnitOfWork(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public ILeaveRequestRepository LeaveRequests
    {
        get { return _leaveRequestRepository ??= new LeaveRequestRepository(_context); }
    }

    public IPublicHolidayRepository PublicHolidays
    {
        get { return _publicHolidayRepository ??= new PublicHolidayRepository(_context); }
    }

    public IUserRepository Users
    {
        get { return _userRepository ??= new UserRepository(_context, _userManager); }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}