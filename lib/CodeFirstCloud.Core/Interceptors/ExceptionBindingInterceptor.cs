using Microsoft.Extensions.Logging;

namespace CodeFirstCloud.Interceptors;

internal sealed class ExceptionBindingInterceptor : ICodeFirstCloudBindingInterceptor
{
    private readonly ILogger<ExceptionBindingInterceptor> _logger;
    private readonly TimeSpan _timeBetweenAttempts = TimeSpan.FromSeconds(5);

    public ExceptionBindingInterceptor(ILogger<ExceptionBindingInterceptor> logger)
    {
        _logger = logger;
    }

    public async Task StartAsync(Func<BindingInterceptorContext, Task> next, BindingInterceptorContext context)
    {
        await WrapInTryCatch(next, context, "start");
    }

    public async Task ExecuteAsync(Func<BindingInterceptorContext, Task> next, BindingInterceptorContext context)
    {
        await WrapInTryCatch(next, context, "execute");
    }

    public async Task StopAsync(Func<BindingInterceptorContext, Task> next, BindingInterceptorContext context)
    {
        // A stop has no point in catching or retrying. We must stop the application asap
        await next(context);
    }

    private async Task WrapInTryCatch(Func<BindingInterceptorContext, Task> next, BindingInterceptorContext context, string operation)
    {
        while (!context.CancellationToken.IsCancellationRequested)
        {
            try
            {
                await next(context);
                return;
            }
            catch (TaskCanceledException) when (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to {OperationName} binding {BindingName}. Trying again after {TimeInMilliseconds}ms.", operation, context.ActualBinding.GetType().Name, _timeBetweenAttempts.TotalMilliseconds);
            }

            await Task.Delay(_timeBetweenAttempts);
        }
    }
}