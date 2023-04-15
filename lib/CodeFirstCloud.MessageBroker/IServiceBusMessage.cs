using System.Text.Json.Serialization;

namespace CodeFirstCloud.MessageBroker;

[JsonConverter(typeof(ServiceBusMessageHandlerConverter))]
public interface IServiceBusMessage
{
    TBody ReadFromJson<TBody>();
}