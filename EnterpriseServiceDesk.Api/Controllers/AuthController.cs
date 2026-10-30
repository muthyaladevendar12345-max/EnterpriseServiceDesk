using System.Security.Claims;
using EnterpriseServiceDesk.Api.Services;
using EnterpriseServiceDesk.Core.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseServiceDesk.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);

        if (result is null)
        {
            return Unauthorized(new
            {
                message = "Unable to sign in with these credentials."
            });
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            userId = User.FindFirstValue("sub"),
            employeeId = User.FindFirstValue("employee_id"),
            fullName = User.Identity?.Name,
            email = User.FindFirstValue("email"),
            roles = User.FindAll("role").Select(x => x.Value).ToArray()
        });
    }
}