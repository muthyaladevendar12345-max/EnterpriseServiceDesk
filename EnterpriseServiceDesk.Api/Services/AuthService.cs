using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EnterpriseServiceDesk.Core.Dtos.Auth;
using EnterpriseServiceDesk.Infrastructure.Data;
using EnterpriseServiceDesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EnterpriseServiceDesk.Api.Services;

public sealed class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ServiceDeskDbContext _db;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<ApplicationUser> userManager,ServiceDeskDbContext db, IConfiguration configuration)
    {
        _userManager = userManager;
        _db = db;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());

        if (user is null || await _userManager.IsLockedOutAsync(user))
        {
            return null;
        }

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            await _userManager.AccessFailedAsync(user);
            return null;
        }

        var employee = await _db.Employees.AsNoTracking().SingleOrDefaultAsync( x => x.Id == user.EmployeeId && x.IsActive,cancellationToken);

        if (employee is null)
        {
            return null;
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var roles = (await _userManager.GetRolesAsync(user)).ToArray();

        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiryMinutes"));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("employee_id", employee.Id.ToString()),
            new("name", employee.FullName),
            new("email", user.Email!)
        };

        claims.AddRange(roles.Select(role => new Claim("role", role)));

        var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( _configuration["Jwt:SigningKey"]!));

        var token = new JwtSecurityToken(issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(key,SecurityAlgorithms.HmacSha256));

        return new LoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt,
            employee.Id,
            employee.FullName,
            user.Email!,
            roles);
    }
}