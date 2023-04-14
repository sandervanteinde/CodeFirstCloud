using RabbitMQ.Client.Events;
using System.Text.Json;

namespace CodeFirstCloud.MessageBroker.RabbitMQ;

public class RabbitMqServiceBusMessage : IServiceBusMessage
{
    private readonly BasicDeliverEventArgs _basicDeliverEventArgs;

    public RabbitMqServiceBusMessage(BasicDeliverEventArgs basicDeliverEventArgs)
    {
        _basicDeliverEventArgs = basicDeliverEventArgs;
    }

    public TBody ReadFromJson<TBody>()
    {
        return JsonSerializer.Deserialize<TBody>(_basicDeliverEventArgs.Body.Span)!;
    }
}