using RabbitMQ.Client;

namespace CodeFirstCloud.MessageBroker.RabbitMQ;

internal sealed class RabbitMqConnection : IDisposable
{
    private IConnection? _connection;
    public IConnection Connection => _connection ?? throw new InvalidOperationException("Connection was not initialized");

    public void Dispose()
    {
        _connection?.Dispose();
    }

    public void Connect()
    {
        if (_connection is not null)
        {
            return;
        }

        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://rabbitmq:HelloWorld123@localhost:5672"),
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
    }
}