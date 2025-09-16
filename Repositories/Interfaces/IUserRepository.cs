using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<ApplicationUser>
{
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<bool> IsInRoleAsync(ApplicationUser user, string role);
    Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string role);
    Task<IEnumerable<string>> GetUserRolesAsync(ApplicationUser user);
}