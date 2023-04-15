using CodeFirstCloud.MessageBroker;

namespace CodeFirstCloud.Examples;

[ServiceBusHandler(TopicName = "sander-testing", SubscriptionName = "my-subscription")]
public class MyHandler : IServiceBusMessageHandler
{
    public Task HandleAsync(IServiceBusMessage serviceBusMessage, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}