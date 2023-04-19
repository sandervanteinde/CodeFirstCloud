using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CodeFirstCloud;

internal class HandlerBindingRunner<TBinding> : BackgroundService
    where TBinding : ICodeFirstCloudBinding
{
    private const int StartState = 1;
    private const int ExecuteState = 2;
    private const int ExecutedState = 3;

    private readonly IServiceProvider _sp;
    private TBinding? _runningBinding;
    private IAsyncDisposable? _scope;
    private int _state = StartState;

    public HandlerBindingRunner(IServiceProvider sp)
    {
        _sp = sp;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // The start is intentionally not in the StartAsync, because we want any other part of the application to run, even if this would potentially fail
        while (!stoppingToken.IsCancellationRequested)
        {
            var scope = _sp.CreateAsyncScope();
            var binding = scope.ServiceProvider.GetRequiredService<TBinding>();
            var interceptors = scope.ServiceProvider
               .GetServices<ICodeFirstCloudBindingInterceptor>()
               .ToArray();

            var context = new BindingInterceptorContext
            {
                ActualBinding = binding,
                CancellationToken = stoppingToken
            };

            try
            {
                switch (_state)
                {
                    case StartState:
                        var startPipeline = ConstructPipeline(
                            interceptors,
                            interceptor => interceptor.StartAsync,
                            binding.StartAsync
                        );
                        await startPipeline.Invoke(context);
                        _state = ExecuteState;
                        // fall through intended
                        goto case ExecuteState;
                    case ExecuteState:
                        var executePipeline = ConstructPipeline(
                            interceptors,
                            interceptor => interceptor.ExecuteAsync,
                            binding.ExecuteAsync
                        );
                        await executePipeline.Invoke(context);
                        _state = ExecutedState;
                        break;
                }

                _scope = scope;
                _runningBinding = binding;
                return;
            }
            catch
            {
                await scope.DisposeAsync();
                throw;
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);

        if (_runningBinding is not null && _scope is not null)
        {
            var interceptors = ((AsyncServiceScope)_scope).ServiceProvider
               .GetRequiredService<IEnumerable<ICodeFirstCloudBindingInterceptor>>()
               .ToArray();

            var context = new BindingInterceptorContext
            {
                ActualBinding = _runningBinding,
                CancellationToken = cancellationToken
            };

            var stopPipeline = ConstructPipeline(
                interceptors,
                interceptor => interceptor.StopAsync,
                _runningBinding.StopAsync
            );
            await stopPipeline.Invoke(context);
        }

        if (_scope is not null)
        {
            await _scope.DisposeAsync();
        }

        _scope = null;
    }

    private static Func<BindingInterceptorContext, Task> ConstructPipeline(
        IReadOnlyCollection<ICodeFirstCloudBindingInterceptor> interceptors,
        Func<ICodeFirstCloudBindingInterceptor, Func<Func<BindingInterceptorContext, Task>, BindingInterceptorContext, Task>> interceptorFunc,
        Func<CancellationToken, Task> cloudBindingFunc
    )
    {
        if (interceptors.Count == 0)
        {
            return Seed;
        }


        return interceptors
           .Reverse()
           .Aggregate(
                Seed,
                (prev, next) =>
                {
                    var nextMethod = interceptorFunc.Invoke(next);
                    return async ct => await nextMethod.Invoke(prev, ct);
                }
            );

        async Task Seed(BindingInterceptorContext ctx)
        {
            await cloudBindingFunc(ctx.CancellationToken);
        }
    }
}