using EnterpriseServiceDesk.Core.Dtos;
using EnterpriseServiceDesk.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseServiceDesk.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/agents/hr")]
public sealed class HrAgentController : ControllerBase
{
    private readonly IHrAgentService _hrAgentService;

    public HrAgentController(IHrAgentService hrAgentService)
    {
        _hrAgentService = hrAgentService;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] AgentQuestionRequest request, CancellationToken cancellationToken)
    {
        var reply = await _hrAgentService.AskAsync(request.Message,cancellationToken);

        return Ok(new
        {
            agentName = "HrAgent",
            reply
        });
    }
}