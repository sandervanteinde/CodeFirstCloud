namespace CodeFirstCloud;

public interface ICodeFirstCloudBinding
{
    Task StartAsync(CancellationToken cancellationToken);
    Task ExecuteAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}