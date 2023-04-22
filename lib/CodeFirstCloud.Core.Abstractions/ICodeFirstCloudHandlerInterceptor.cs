namespace CodeFirstCloud;

public interface ICodeFirstCloudHandlerInterceptor
{
    Task Intercept<THandler>(Func<Task> next, CancellationToken cancellationToken)
        where THandler : ICodeFirstCloudHandler;

    Task Intercept<THandler, T1>(Func<Task> next, T1 t1, CancellationToken cancellationToken)
        where THandler : ICodeFirstCloudHandler<T1>;
}