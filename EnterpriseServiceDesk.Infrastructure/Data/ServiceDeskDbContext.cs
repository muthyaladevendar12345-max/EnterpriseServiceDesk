using EnterpriseServiceDesk.Core.Entities;
using EnterpriseServiceDesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseServiceDesk.Infrastructure.Data;

public sealed class ServiceDeskDbContext:IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ServiceDeskDbContext(DbContextOptions<ServiceDeskDbContext> options): base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.EmployeeNumber)
                .HasMaxLength(30)
                .IsRequired();

            entity.HasIndex(x => x.EmployeeNumber)
                .IsUnique();

            entity.Property(x => x.FullName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Email)
                .HasMaxLength(256)
                .IsRequired();

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.Property(x => x.Department)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();
        });

        modelBuilder.Entity<LeaveBalance>(entity =>
        {
            entity.ToTable("LeaveBalances", table =>
            {
                table.HasCheckConstraint(
                    "CK_LeaveBalances_Year",
                    "[Year] BETWEEN 2000 AND 9999");

                table.HasCheckConstraint(
                    "CK_LeaveBalances_Days",
                    "[AllocatedDays] >= 0 AND " +
                    "[UsedDays] >= 0 AND " +
                    "[UsedDays] <= [AllocatedDays]");
            });

            entity.HasKey(x => x.Id);

            entity.Property(x => x.LeaveType)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.AllocatedDays)
                .HasPrecision(6, 2);

            entity.Property(x => x.UsedDays)
                .HasPrecision(6, 2);

            entity.Ignore(x => x.RemainingDays);

            entity.HasIndex(x => new
            {
                x.EmployeeId,
                x.Year,
                x.LeaveType
            })
                .IsUnique();

            entity.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasOne(x => x.Employee)
                .WithOne()
                .HasForeignKey<ApplicationUser>(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.EmployeeId)
                .IsUnique();
        });
    }
}