using LeaveManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace LeaveManagementSystem.Data.Seeding;

public static class DbSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Seed Roles
        var roles = new[] { "Admin", "Manager", "Employee" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed Admin User
        var adminEmail = "admin@leavemanagement.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRolesAsync(adminUser, new[] { "Admin", "Manager", "Employee" });
            }
        }
    }

    public static async Task SeedPublicHolidaysAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.PublicHolidays.Any())
        {
            var holidays = new List<PublicHoliday>
            {
                new() { Id = Guid.NewGuid(), Date = new DateTime(2025, 1, 1), Name = "New Year's Day", IsRecurring = true },
                new() { Id = Guid.NewGuid(), Date = new DateTime(2025, 3, 21), Name = "Human Rights Day", IsRecurring = true },
                new() { Id = Guid.NewGuid(), Date = new DateTime(2025, 4, 27), Name = "Freedom Day", IsRecurring = true },
                new() { Id = Guid.NewGuid(), Date = new DateTime(2025, 5, 1), Name = "Workers' Day", IsRecurring = true },
                new() { Id = Guid.NewGuid(), Date = new DateTime(2025, 6, 16), Name = "Youth Day", IsRecurring = true },
                new() { Id = Guid.NewGuid(), Date = new DateTime(2025, 8, 9), Name = "National Women's Day", IsRecurring = true },
                new() { Id = Guid.NewGuid(), Date = new DateTime(2025, 9, 24), Name = "Heritage Day", IsRecurring = true },
                new() { Id = Guid.NewGuid(), Date = new DateTime(2025, 12, 16), Name = "Day of Reconciliation", IsRecurring = true },
                new() { Id = Guid.NewGuid(), Date = new DateTime(2025, 12, 25), Name = "Christmas Day", IsRecurring = true },
                new() { Id = Guid.NewGuid(), Date = new DateTime(2025, 12, 26), Name = "Day of Goodwill", IsRecurring = true }
            };

            context.PublicHolidays.AddRange(holidays);
            await context.SaveChangesAsync();
        }
    }
}