using CodeFirstCloud.MessageBroker;
using Microsoft.Extensions.Logging;

namespace CodeFirstCloud.Examples;

[ServiceBusHandler(TopicName = "sander-testing", SubscriptionName = "my-second-subscription")]
public class MySucceedingHandler : JsonServiceBusMessageHandler<MyMessage>
{
    private readonly ILogger<MySucceedingHandler> _logger;

    public MySucceedingHandler(ILogger<MySucceedingHandler> logger)
    {
        _logger = logger;
    }

    protected override Task ProcessTypedMessageAsync(MyMessage body, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message received: {MessageText}", body.Text);
        return Task.CompletedTask;
    }
}