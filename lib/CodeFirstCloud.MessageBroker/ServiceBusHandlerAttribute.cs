namespace CodeFirstCloud.MessageBroker;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceBusHandlerAttribute : Attribute
{
    public required string TopicName { get; init; }
    public required string SubscriptionName { get; init; }
}