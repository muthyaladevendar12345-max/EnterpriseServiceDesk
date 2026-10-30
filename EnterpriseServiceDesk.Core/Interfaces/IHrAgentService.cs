namespace EnterpriseServiceDesk.Core.Interfaces;

public interface IHrAgentService
{
    Task<string> AskAsync(string message,CancellationToken cancellationToken = default);
}