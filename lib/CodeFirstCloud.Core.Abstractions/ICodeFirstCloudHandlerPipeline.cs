namespace CodeFirstCloud;

public interface ICodeFirstCloudHandlerPipeline<THandler, in T1>
    where THandler : ICodeFirstCloudHandler<T1>
{
    Task HandleAsync(T1 t1, CancellationToken cancellationToken);
}

public interface ICodeFirstCloudHandlerPipeline<THandler>
    where THandler : ICodeFirstCloudHandler
{
    Task HandleAsync(CancellationToken cancellationToken);
}