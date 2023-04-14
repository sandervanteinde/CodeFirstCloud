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
    abstract static void AddEndpoint(WebApplication app);
}
