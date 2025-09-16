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

    public static async Task SeedEmployeesAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Create CEO
        var ceo = new ApplicationUser
        {
            UserName = "lindajenkins@acme.com",
            Email = "lindajenkins@acme.com",
            EmailConfirmed = true,
            PhoneNumber = null,
            EmployeeNumber = "0001",
            FullName = "Linda Jenkins"
        };
        
        if (await userManager.FindByEmailAsync(ceo.Email) == null)
        {
            var result = await userManager.CreateAsync(ceo, "Linda@123");
            if (result.Succeeded)
            {
                await userManager.AddToRolesAsync(ceo, new[] { "Manager", "Employee" });
            }
        }

        // Create Management Team
        var managementTeam = new List<(string FullName, string EmpNo, string Email, string Phone)>
        {
            ("Milton Coleman", "0002", "miltoncoleman@amce.com", "+27 55 937 274"),
            ("Colin Horton", "0003", "colinhorton@amce.com", "+27 20 915 7545")
        };

        foreach (var manager in managementTeam)
        {
            var user = new ApplicationUser
            {
                UserName = manager.Email,
                Email = manager.Email,
                EmailConfirmed = true,
                PhoneNumber = manager.Phone,
                EmployeeNumber = manager.EmpNo,
                FullName = manager.FullName,
                ManagerId = ceo.Id
            };

            if (await userManager.FindByEmailAsync(user.Email) == null)
            {
                var result = await userManager.CreateAsync(user, $"{manager.FullName.Replace(" ", "")}@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(user, new[] { "Manager", "Employee" });
                }
            }
        }

        // Get Colin Horton's ID for the dev team
        var colinHorton = await userManager.FindByEmailAsync("colinhorton@amce.com");
        if (colinHorton != null)
        {
            // Create Dev Team
            var devTeam = new List<(string FullName, string EmpNo, string Email, string Phone)>
            {
                ("Ella Jefferson", "2005", "ellajefferson@acme.com", "+27 55 979 367"),
                ("Earl Craig", "2006", "earlcraig@acme.com", "+27 20 916 5608"),
                ("Marsha Murphy", "2008", "marshamurphy@acme.com", "+36 55 949 891"),
                ("Luis Ortega", "2009", "luisortega@acme.com", "+27 20 917 1339"),
                ("Faye Dennis", "2010", "fayedennis@acme.com", null),
                ("Amy Burns", "2012", "amyburns@acme.com", "+27 20 914 1775"),
                ("Darrel Weber", "2013", "darrelweber@acme.com", "+27 55 615 463")
            };

            foreach (var dev in devTeam)
            {
                var user = new ApplicationUser
                {
                    UserName = dev.Email,
                    Email = dev.Email,
                    EmailConfirmed = true,
                    PhoneNumber = dev.Phone,
                    EmployeeNumber = dev.EmpNo,
                    FullName = dev.FullName,
                    ManagerId = colinHorton.Id
                };

                if (await userManager.FindByEmailAsync(user.Email) == null)
                {
                    var result = await userManager.CreateAsync(user, $"{dev.FullName.Replace(" ", "")}@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Employee");
                    }
                }
            }
        }
    }
}