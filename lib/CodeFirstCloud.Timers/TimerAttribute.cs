namespace CodeFirstCloud.Timers;

[AttributeUsage(AttributeTargets.Class)]
public class TimerAttribute : Attribute
{
    public string CronExpression { get; }

    public TimerAttribute(string cronExpression)
    {
        CronExpression = cronExpression;
    }
}