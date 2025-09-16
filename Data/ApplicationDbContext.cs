using LeaveManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<LeaveRequest> LeaveRequests { get; set; } = null!;
    public DbSet<PublicHoliday> PublicHolidays { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure ApplicationUser
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.HasOne(e => e.Manager)
                .WithMany(e => e.Employees)
                .HasForeignKey(e => e.ManagerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure LeaveRequest
        builder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.DaysRequested)
                .IsRequired();

            entity.Property(e => e.StartDate)
                .IsRequired();

            entity.Property(e => e.EndDate)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.HasOne(e => e.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.DecisionBy)
                .WithMany(e => e.ManagedLeaveRequests)
                .HasForeignKey(e => e.DecisionById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure PublicHoliday
        builder.Entity<PublicHoliday>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Date)
                .IsRequired();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.IsRecurring)
                .IsRequired();
        });
    }
}