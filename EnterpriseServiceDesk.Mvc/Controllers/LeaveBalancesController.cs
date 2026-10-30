using EnterpriseServiceDesk.Mvc.Models;
using EnterpriseServiceDesk.Mvc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseServiceDesk.Mvc.Controllers;

[Authorize]
public sealed class LeaveBalancesController : Controller
{
    private readonly LeaveBalanceApiService _apiService;
    private readonly ILogger<LeaveBalancesController> _logger;

    public LeaveBalancesController(LeaveBalanceApiService apiService, ILogger<LeaveBalancesController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? year,CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || year is < 2000 or > 9999)
        {
            return BadRequest("Provide a year between 2000 and 9999.");
        }

        var model = new LeaveBalancesViewModel
        {
            Year = year ?? DateTime.UtcNow.Year
        };

        try
        {
            var balances = await _apiService.GetMineAsync(model.Year,cancellationToken);

            if (balances is null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return RedirectToAction("Login", "Account");
            }

            model.Balances = balances;
        }
        catch (HttpRequestException exception)
        {
            _logger.LogWarning(exception,"Unable to retrieve employee leave balances.");

            model.ErrorMessage = "Unable to load leave balances. Please try again.";
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            model.ErrorMessage ="The request timed out. Please try again.";
        }

        return View(model);
    }
}