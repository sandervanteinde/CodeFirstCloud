using System.Text.Json;
using System.Text.Json.Nodes;

namespace CodeFirstCloud.MessageBroker;

public readonly struct TestingServiceBusMessage : IServiceBusMessage
{
    private readonly object? _content;
    private readonly JsonNode? _stream;

    public TestingServiceBusMessage(object content)
    {
        _content = content;
    }

    public TestingServiceBusMessage(JsonNode stream)
    {
        _stream = stream;
    }

    public TExpected ReadFromJson<TExpected>()
    {
        if (_content is not null)
        {
            return (TExpected)_content;
        }

        if (_stream is not null)
        {
            return _stream.Deserialize<TExpected>()!;
        }

        throw new NotSupportedException();
    }
}