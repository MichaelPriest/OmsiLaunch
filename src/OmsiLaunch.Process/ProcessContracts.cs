namespace OmsiLaunch.Process;

public sealed record OmsiProcessState(string State, int? ProcessId, bool Responding);
public interface IOmsiProcessController
{
    OmsiProcessState Observe();
    Task<int> StartAsync(string installation, CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}
