namespace CodeFirstCloud;

public interface ICodeFirstCloudHandlerPipeline<THandler, in T1, in T2, in T3>
    where THandler : ICodeFirstCloudHandler<T1, T2, T3>
{
    Task HandleAsync(T1 t1, T2 t2, T3 t3, CancellationToken cancellationToken);
}

public interface ICodeFirstCloudHandlerPipeline<THandler, in T1, in T2>
    where THandler : ICodeFirstCloudHandler<T1, T2>
{
    Task HandleAsync(T1 t1, T2 t2, CancellationToken cancellationToken);
}

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