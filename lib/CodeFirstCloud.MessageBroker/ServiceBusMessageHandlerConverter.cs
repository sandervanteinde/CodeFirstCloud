using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace CodeFirstCloud.MessageBroker;

internal class ServiceBusMessageHandlerConverter : JsonConverter<IServiceBusMessage>
{
    public override IServiceBusMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonNode = JsonSerializer.Deserialize<JsonNode>(ref reader, options);
        return new TestingServiceBusMessage(jsonNode!);
    }

    public override void Write(Utf8JsonWriter writer, IServiceBusMessage value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }
}