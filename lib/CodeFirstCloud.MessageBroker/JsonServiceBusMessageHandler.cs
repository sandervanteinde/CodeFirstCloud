namespace CodeFirstCloud.MessageBroker;

public abstract class JsonServiceBusMessageHandler<TBody> : IServiceBusMessageHandler
{
    private IServiceBusMessage? _message;
    protected IServiceBusMessage Message => _message ?? throw new InvalidOperationException("Message should have been filled prior to invoking inherited class.");

    public async Task ProcessMessageAsync(IServiceBusMessage serviceBusMessage, CancellationToken cancellationToken)
    {
        _message = serviceBusMessage;

        try
        {
            var body = serviceBusMessage.ReadFromJson<TBody>();
            await ProcessTypedMessageAsync(body, cancellationToken);
        }
        finally
        {
            _message = null;
        }
    }

    protected abstract Task ProcessTypedMessageAsync(TBody body, CancellationToken cancellationToken);
}