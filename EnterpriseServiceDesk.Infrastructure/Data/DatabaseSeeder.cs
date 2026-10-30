using EnterpriseServiceDesk.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseServiceDesk.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ServiceDeskDbContext db,CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var year = DateTime.UtcNow.Year;

        await SeedEmployeeAsync(
            db,
            employeeNumber: "EMP001",
            fullName: "Sample HR Employee",
            email: "employee.hr@servicedesk.test",
            department: "HR",
            year: year,
            annualUsedDays: 8m,
            cancellationToken: cancellationToken);

        await SeedEmployeeAsync(
            db,
            employeeNumber: "EMP002",
            fullName: "Sample IT Employee",
            email: "employee.it@servicedesk.test",
            department: "IT",
            year: year,
            annualUsedDays: 5m,
            cancellationToken: cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private static async Task SeedEmployeeAsync(
        ServiceDeskDbContext db,
        string employeeNumber,
        string fullName,
        string email,
        string department,
        int year,
        decimal annualUsedDays,
        CancellationToken cancellationToken)
    {
        var employee = await db.Employees.SingleOrDefaultAsync(
            x => x.EmployeeNumber == employeeNumber,
            cancellationToken);

        if (employee is null)
        {
            employee = new Employee
            {
                EmployeeNumber = employeeNumber,
                FullName = fullName,
                Email = email,
                Department = department
            };

            db.Employees.Add(employee);
        }

        var existingLeaveTypes = await db.LeaveBalances
            .Where(x => x.EmployeeId == employee.Id && x.Year == year)
            .Select(x => x.LeaveType)
            .ToListAsync(cancellationToken);

        if (!existingLeaveTypes.Contains("Annual"))
        {
            db.LeaveBalances.Add(new LeaveBalance
            {
                EmployeeId = employee.Id,
                Employee = employee,
                Year = year,
                LeaveType = "Annual",
                AllocatedDays = 20m,
                UsedDays = annualUsedDays
            });
        }

        if (!existingLeaveTypes.Contains("Sick"))
        {
            db.LeaveBalances.Add(new LeaveBalance
            {
                EmployeeId = employee.Id,
                Employee = employee,
                Year = year,
                LeaveType = "Sick",
                AllocatedDays = 10m,
                UsedDays = 2m
            });
        }
    }
}