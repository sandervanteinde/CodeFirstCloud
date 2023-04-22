using CodeFirstCloud.Constructable;

namespace CodeFirstCloud.MessageBroker;

public abstract class JsonServiceBusMessageHandler<TBody> : IServiceBusMessageHandler, IConstructable<TBody, IServiceBusMessage>
{
    private IServiceBusMessage? _message;
    protected IServiceBusMessage Message => _message ?? throw new InvalidOperationException("Message should have been filled prior to invoking inherited class.");

    public static IServiceBusMessage CreateTestable(TBody input)
    {
        return new TestingServiceBusMessage(input!);
    }

    public async Task HandleAsync(IServiceBusMessage serviceBusMessage, CancellationToken cancellationToken)
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