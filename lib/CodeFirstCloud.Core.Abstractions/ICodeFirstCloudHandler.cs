namespace CodeFirstCloud;

public interface ICodeFirstCloudHandler<in T1>
{
    Task HandleAsync(T1 serviceBusMessage, CancellationToken cancellationToken);
}

public interface ICodeFirstCloudHandler
{
    Task HandleAsync(CancellationToken cancellationToken);
}