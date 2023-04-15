using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;

namespace CodeFirstCloud.MessageBroker.RabbitMQ;

internal class RabbitMqBinding<THandler> : ICodeFirstCloudBinding, IDisposable
    where THandler : IServiceBusMessageHandler
{
    private readonly RabbitMqConnection _connection;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _subscriptionName;
    private readonly string _topicName;
    private IModel? _channel;
    private string? _consumerTag;

    public RabbitMqBinding(RabbitMqConnection connection, IServiceProvider serviceProvider)
    {
        var attr = typeof(THandler)
                      .GetCustomAttribute<ServiceBusHandlerAttribute>()
                   ?? throw new InvalidOperationException("Attribute missing.");
        _connection = connection;
        _serviceProvider = serviceProvider;
        _topicName = attr.TopicName;
        _subscriptionName = attr.SubscriptionName;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _connection.Connect();
        _channel = _connection.Connection.CreateModel();

        var deadLetterQueueExchange = "dead-letter-exchange";
        var queueName = $"{_topicName}-{_subscriptionName}";
        var dlqQueueName = $"{queueName}-dlq";

        // create Topic exchange / queue
        _channel.ExchangeDeclare(_topicName, ExchangeType.Fanout);
        _channel.QueueDeclare(queueName, false, false, false, null);
        _channel.QueueBind(queueName, _topicName, string.Empty, null);

        // Create a fake queue for dead letter messages
        _channel.ExchangeDeclare(deadLetterQueueExchange, ExchangeType.Direct);
        _channel.QueueDeclare(dlqQueueName, false, false, false, null);
        _channel.QueueBind(dlqQueueName, deadLetterQueueExchange, queueName, null);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (sender, args) =>
        {
            try
            {
                await ProcessMessageAsync(args);
                _channel.BasicAck(args.DeliveryTag, false);
            }
            catch
            {
                _channel.BasicPublish(deadLetterQueueExchange, queueName, args.BasicProperties, args.Body);
            }
            finally
            {
                _channel.BasicAck(args.DeliveryTag, false);
            }
        };
        _consumerTag = _channel.BasicConsume(queueName, true, consumer);
        return Task.CompletedTask;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null && _consumerTag is not null)
        {
            _channel.BasicCancel(_consumerTag);
        }

        _channel?.Close();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }

    private async Task ProcessMessageAsync(BasicDeliverEventArgs basicDeliverEventArgs)
    {
        using var sp = _serviceProvider.CreateScope();
        var handler = sp.ServiceProvider.GetRequiredService<ICodeFirstCloudHandlerPipeline<THandler, IServiceBusMessage>>();

        await handler.HandleAsync(new RabbitMqServiceBusMessage(basicDeliverEventArgs), CancellationToken.None);
    }
}