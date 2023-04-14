using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CodeFirstCloud;

internal class HandlerBindingRunner<TBinding> : BackgroundService
    where TBinding : ICodeFirstCloudBinding
{
    private readonly ILogger<HandlerBindingRunner<TBinding>> _logger;
    private readonly TimeSpan _retryTime = TimeSpan.FromSeconds(5);
    private readonly IServiceProvider _sp;
    private TBinding? _runningBinding;
    private IAsyncDisposable? _scope;

    public HandlerBindingRunner(
        IServiceProvider sp,
        ILogger<HandlerBindingRunner<TBinding>> logger
    )
    {
        _sp = sp;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var scope = _sp.CreateAsyncScope();
            var binding = scope.ServiceProvider.GetRequiredService<TBinding>();

            try
            {
                await binding.StartAsync(stoppingToken);
                await binding.ExecuteAsync(stoppingToken);
                _scope = scope;
                _runningBinding = binding;
                return;
            }
            // only return out of this if our task was cancelled.
            // It could be that implementing bindings throw cancellation tokens on their own.
            // They should handle these exceptions
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to start handler binding {BindingName}. Trying again after {TimeInMilliseconds}ms.", binding.GetType(), _retryTime.TotalMilliseconds);
                await scope.DisposeAsync();
                // Do not rethrow. We always want to keep retrying to attempt starting the service
            }

            await Task.Delay(_retryTime, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);

        if (_runningBinding is not null)
        {
            await _runningBinding.StartAsync(cancellationToken);
        }

        if (_scope is not null)
        {
            await _scope.DisposeAsync();
        }

        _scope = null;
    }
}