namespace CodeFirstCloud;

public interface ICodeFirstCloudHandlerInterceptor
{
    Task Intercept<THandler>(Func<Task> next, CancellationToken cancellationToken)
        where THandler : ICodeFirstCloudHandler;

    Task Intercept<THandler, T1>(Func<Task> next, T1 t1, CancellationToken cancellationToken)
        where THandler : ICodeFirstCloudHandler<T1>;

    Task Intercept<THandler, T1, T2>(Func<Task> next, T1 t1, T2 t2, CancellationToken cancellationToken)
        where THandler : ICodeFirstCloudHandler<T1, T2>;

    Task Intercept<THandler, T1, T2, T3>(Func<Task> next, T1 t1, T2 t2, T3 t3, CancellationToken cancellationToken)
        where THandler : ICodeFirstCloudHandler<T1, T2, T3>;
}