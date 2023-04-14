using Microsoft.Extensions.DependencyInjection;
using NCrontab;
using System.Reflection;

namespace CodeFirstCloud.Timers;

public class TimerCodeFirstCloudBinding<THandler> : ICodeFirstCloudBinding
    where THandler : ITimerHandler
{
    private readonly string _cronExpression;
    private readonly IServiceProvider _serviceProvider;
    private CrontabSchedule? _schedule;

    public TimerCodeFirstCloudBinding(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        var timerAttribute = typeof(THandler)
           .GetCustomAttribute<TimerAttribute>();
        _cronExpression = timerAttribute?.CronExpression
                          ?? throw new InvalidOperationException("TimerAttribute is missing on handler.");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _schedule = CrontabSchedule.Parse(_cronExpression);
        return Task.CompletedTask;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (_schedule is null)
        {
            throw new InvalidOperationException("Expected StartAsync to be called first.");
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            var nextOccurrence = _schedule.GetNextOccurrence(DateTime.UtcNow);

            if (nextOccurrence <= DateTime.UtcNow)
            {
                await FireNextEvent(cancellationToken);
                continue;
            }

            await Task.Delay(nextOccurrence - DateTime.UtcNow, cancellationToken);
            await FireNextEvent(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task FireNextEvent(CancellationToken cancellationToken)
    {
        using var sp = _serviceProvider.CreateScope();
        var handler = sp.ServiceProvider.GetRequiredService<THandler>();
        await handler.ExecuteAsync(cancellationToken);
    }
}