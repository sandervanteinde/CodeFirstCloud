namespace CodeFirstCloud.MessageBroker;

public interface IServiceBusMessageHandler
{
    Task ProcessMessageAsync(IServiceBusMessage serviceBusMessage, CancellationToken cancellationToken);
}