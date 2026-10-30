using EnterpriseServiceDesk.Core.Interfaces;
using EnterpriseServiceDesk.Infrastructure.Tools;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace EnterpriseServiceDesk.Infrastructure.Agents;

public sealed class HrAgentService : IHrAgentService
{
    private readonly IChatClient _chatClient;
    private readonly HrTools _hrTools;

    public HrAgentService(IChatClient chatClient,HrTools hrTools)
    {
        _chatClient = chatClient;
        _hrTools = hrTools;
    }

    public async Task<string> AskAsync(string message,CancellationToken cancellationToken = default)
    {
        AIAgent agent = _chatClient.AsAIAgent(name: "HrAgent",
            instructions: """
                You are an employee HR assistant.

                For the employee's current-year leave balance,
                call the available leave balance tool.
                Never invent balances or claim you checked data
                unless the tool was executed.

                Report the leave type, allocated, used, and remaining days
                from the tool result.

                If no records are returned, explain that no balances
                were found.

                You can access only the authenticated employee's data.
                Refuse requests for another employee's private information.

                Company policy retrieval is not connected yet.
                Do not invent policy rules or treat an employee's
                allocated balance as evidence of company policy.

                If a tool fails, explain that the information could
                not be retrieved. Do not fabricate a result.
                """,
            tools:
            [
                AIFunctionFactory.Create(_hrTools.GetLeaveBalanceAsync)
            ]);

        var response = await agent.RunAsync(message,cancellationToken: cancellationToken);

        return response.ToString();
    }
}