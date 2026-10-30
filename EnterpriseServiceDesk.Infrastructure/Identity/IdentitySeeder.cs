using EnterpriseServiceDesk.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseServiceDesk.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(ServiceDeskDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, string hrPassword, string itPassword,CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(hrPassword) ||
            string.IsNullOrWhiteSpace(itPassword))
        {
            throw new InvalidOperationException("Configure both SeedUsers passwords in API User Secrets.");
        }

        const string employeeRole = "Employee";

        if (!await roleManager.RoleExistsAsync(employeeRole))
        {
            var roleResult = await roleManager.CreateAsync(
                new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = employeeRole
                });

            EnsureSucceeded(roleResult, "Create Employee role");
        }

        await EnsureUserAsync(
            db,
            userManager,
            "EMP001",
            hrPassword,
            employeeRole,
            cancellationToken);

        await EnsureUserAsync(
            db,
            userManager,
            "EMP002",
            itPassword,
            employeeRole,
            cancellationToken);
    }

    private static async Task EnsureUserAsync(
        ServiceDeskDbContext db,
        UserManager<ApplicationUser> userManager,
        string employeeNumber,
        string password,
        string role,
        CancellationToken cancellationToken)
    {
        var employee = await db.Employees.SingleOrDefaultAsync(
            x => x.EmployeeNumber == employeeNumber,
            cancellationToken)
            ?? throw new InvalidOperationException(
                $"Seed employee '{employeeNumber}' was not found.");

        var user = await userManager.FindByEmailAsync(employee.Email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = employee.Email,
                Email = employee.Email,
                EmployeeId = employee.Id,
                LockoutEnabled = true
            };

            var createResult =
                await userManager.CreateAsync(user, password);

            EnsureSucceeded(
                createResult,
                $"Create account for '{employeeNumber}'");
        }
        else if (user.EmployeeId != employee.Id)
        {
            throw new InvalidOperationException(
                $"Account for '{employeeNumber}' belongs to another employee.");
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            var roleResult = await userManager.AddToRoleAsync(user, role);

            EnsureSucceeded(
                roleResult,
                $"Assign role for '{employeeNumber}'");
        }
    }

    private static void EnsureSucceeded(
        IdentityResult result,
        string operation)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join(
            "; ",
            result.Errors.Select(x => $"{x.Code}: {x.Description}"));

        throw new InvalidOperationException($"{operation} failed. {errors}");
    }
}