using CodeFirstCloud.MessageBroker;

namespace CodeFirstCloud.Examples;

[ServiceBusHandler(TopicName = "sander-testing", SubscriptionName = "my-subscription")]
public class MyHandler : IServiceBusMessageHandler
{
    public Task ProcessMessageAsync(IServiceBusMessage serviceBusMessage, CancellationToken cancellationToken)
    {
        // Test what happens when exceptions occur, deadlettered?
        throw new Exception();
    }
}