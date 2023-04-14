namespace CodeFirstCloud.Timers;

public interface ITimerHandler
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}