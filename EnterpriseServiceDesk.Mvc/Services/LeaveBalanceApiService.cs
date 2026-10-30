using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using EnterpriseServiceDesk.Core.Dtos;
using Microsoft.AspNetCore.Authentication;

namespace EnterpriseServiceDesk.Mvc.Services;

public sealed class LeaveBalanceApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LeaveBalanceApiService(IHttpClientFactory httpClientFactory,IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IReadOnlyList<LeaveBalanceDto>?> GetMineAsync(int year,CancellationToken cancellationToken = default)
    {
        var context = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException(
                "An HTTP request is required.");

        var token = await context.GetTokenAsync("access_token");

        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var client = _httpClientFactory.CreateClient("ServiceDeskApi");

        using var request = new HttpRequestMessage(
            HttpMethod.Get,$"api/leave-balances/me?year={year}");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.SendAsync(request,cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content .ReadFromJsonAsync<List<LeaveBalanceDto>>( cancellationToken: cancellationToken)
            ?? throw new HttpRequestException(
                "The API returned an empty response body.");
    }
}