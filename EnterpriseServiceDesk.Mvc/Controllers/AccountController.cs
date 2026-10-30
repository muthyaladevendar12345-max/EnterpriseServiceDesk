using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using EnterpriseServiceDesk.Core.Dtos.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseServiceDesk.Mvc.Controllers;

public sealed class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Profile));
        }

        return View(new LoginRequest());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest model,CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        LoginResponse? login;

        try
        {
            var client = _httpClientFactory.CreateClient("ServiceDeskApi");

            using var response = await client.PostAsJsonAsync("api/auth/login",model,cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError(string.Empty,"Unable to sign in with these credentials.");

                return View(model);
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty,
                    "Sign-in is currently unavailable. Please try again.");

                return View(model);
            }

            login = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty,"Cannot connect to the API. Ensure it is running.");

            return View(model);
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            ModelState.AddModelError( string.Empty,"The sign-in request timed out. Please try again.");

            return View(model);
        }

        if (login is null ||
            string.IsNullOrWhiteSpace(login.AccessToken) ||
            login.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            ModelState.AddModelError( string.Empty,"The API returned an invalid sign-in response.");

            return View(model);
        }

        var claims = new List<Claim>
        {
            new("employee_id", login.EmployeeId.ToString()),
            new(ClaimTypes.Name, login.FullName),
            new(ClaimTypes.Email, login.Email)
        };

        claims.AddRange(login.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var properties = new AuthenticationProperties
        {
            IsPersistent = false,
            AllowRefresh = false,
            ExpiresUtc = login.ExpiresAt
        };

        properties.StoreTokens(new[]
        {
            new AuthenticationToken
            {
                Name = "access_token",
                Value = login.AccessToken
            }
        });

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(identity),properties);

        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpGet]
    public IActionResult Profile()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return StatusCode(StatusCodes.Status403Forbidden,"You do not have permission to access this page.");
    }
}