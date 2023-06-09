﻿namespace CodeFirstCloud;

internal sealed class DefaultCodeFirstCloudHandlerPipeline<THandler> : ICodeFirstCloudHandlerPipeline<THandler>
    where THandler : ICodeFirstCloudHandler
{
    private readonly THandler _handler;
    private readonly IEnumerable<ICodeFirstCloudHandlerInterceptor> _interceptors;

    public DefaultCodeFirstCloudHandlerPipeline(
        THandler handler,
        IEnumerable<ICodeFirstCloudHandlerInterceptor> interceptors
    )
    {
        _handler = handler;
        _interceptors = interceptors;
    }

    public async Task HandleAsync(CancellationToken cancellationToken)
    {
        await _interceptors
           .Reverse()
           .Aggregate(
                () => _handler.HandleAsync(cancellationToken),
                (prev, curr) => () => curr.Intercept<THandler>(prev, cancellationToken))
           .Invoke();
    }
}

internal sealed class DefaultCodeFirstCloudHandlerPipeline<THandler, T1> : ICodeFirstCloudHandlerPipeline<THandler, T1>
    where THandler : ICodeFirstCloudHandler<T1>
{
    private readonly THandler _handler;
    private readonly IEnumerable<ICodeFirstCloudHandlerInterceptor> _interceptors;

    public DefaultCodeFirstCloudHandlerPipeline(
        THandler handler,
        IEnumerable<ICodeFirstCloudHandlerInterceptor> interceptors
    )
    {
        _handler = handler;
        _interceptors = interceptors;
    }

    public async Task HandleAsync(T1 t1, CancellationToken cancellationToken)
    {
        await _interceptors
           .Reverse()
           .Aggregate(
                () => _handler.HandleAsync(t1, cancellationToken),
                (prev, curr) => () => curr.Intercept<THandler, T1>(prev, t1, cancellationToken))
           .Invoke();
    }
}