using System.Text.Json;

namespace CodeFirstCloud.MessageBroker;

public readonly struct TestingServiceBusMessage : IServiceBusMessage
{
    private readonly object? _content;
    private readonly Stream? _stream;

    public TestingServiceBusMessage(object content)
    {
        _content = content;
    }

    public TestingServiceBusMessage(Stream stream)
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
            return JsonSerializer.Deserialize<TExpected>(_stream)!;
        }

        throw new NotSupportedException();
    }
}