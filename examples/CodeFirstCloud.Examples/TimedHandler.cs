using CodeFirstCloud.Timers;
using Microsoft.Extensions.Logging;

namespace CodeFirstCloud.Examples;

[Timer("* * * * *")]
public class TimedHandler : ITimerHandler
{
    private readonly ILogger<TimedHandler> _logger;

    public TimedHandler(ILogger<TimedHandler> logger)
    {
        _logger = logger;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Current Time {Time}", DateTime.UtcNow);
        return Task.CompletedTask;
    }
}