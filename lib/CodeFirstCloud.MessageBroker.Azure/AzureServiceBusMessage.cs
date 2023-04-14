using Azure.Messaging.ServiceBus;

namespace CodeFirstCloud.MessageBroker.Azure;

public readonly struct AzureServiceBusMessage : IServiceBusMessage
{
    private readonly ServiceBusReceivedMessage _serviceBusReceivedMessage;

    public AzureServiceBusMessage(ServiceBusReceivedMessage serviceBusReceivedMessage)
    {
        _serviceBusReceivedMessage = serviceBusReceivedMessage;
    }

    public TBody ReadFromJson<TBody>()
    {
        return _serviceBusReceivedMessage.Body.ToObjectFromJson<TBody>();
    }
}