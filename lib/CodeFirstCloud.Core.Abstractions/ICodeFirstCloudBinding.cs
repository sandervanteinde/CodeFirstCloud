using Microsoft.AspNetCore.Builder;

namespace CodeFirstCloud;

public interface ICodeFirstCloudBinding
{
    Task StartAsync(CancellationToken cancellationToken);
    Task ExecuteAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}

public interface IInvokableInDevelopment
{
    static abstract void AddEndpoint(WebApplication app);
}
