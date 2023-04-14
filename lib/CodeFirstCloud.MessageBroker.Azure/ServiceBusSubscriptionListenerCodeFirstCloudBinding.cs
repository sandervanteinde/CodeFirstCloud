using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CodeFirstCloud.MessageBroker.Azure;

internal sealed class ServiceBusSubscriptionListenerCodeFirstCloudBinding<THandler> : BaseMessageBrokerBinding<THandler>, ICodeFirstCloudBinding, IAsyncDisposable
    where THandler : IServiceBusMessageHandler
{
    private readonly ILogger<THandler> _logger;
    private readonly ServiceBusAdministrationClient _serviceBusAdministrationClient;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _subscriptionName;
    private readonly string _topicName;
    private ServiceBusProcessor? _processor;

    public ServiceBusSubscriptionListenerCodeFirstCloudBinding(
        IServiceProvider serviceProvider,
        ServiceBusClient serviceBusClient,
        ServiceBusAdministrationClient serviceBusAdministrationClient,
        ILogger<THandler> logger)
    {
        var attr = typeof(THandler)
                      .GetCustomAttribute<ServiceBusHandlerAttribute>()
                   ?? throw new InvalidOperationException("Attribute missing.");

        _serviceProvider = serviceProvider;
        _serviceBusClient = serviceBusClient;
        _serviceBusAdministrationClient = serviceBusAdministrationClient;
        _logger = logger;
        _topicName = attr.TopicName;
        _subscriptionName = attr.SubscriptionName;
    }

    public async ValueTask DisposeAsync()
    {
        if (_processor is null)
        {
            return;
        }

        await _processor.DisposeAsync();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // check if the topic / subscription exist, else create them
        if (!await _serviceBusAdministrationClient.TopicExistsAsync(_topicName, cancellationToken))
        {
            _logger.LogInformation("Topic {TopicName} does not exist. Creating it.", _topicName);
            await _serviceBusAdministrationClient.CreateTopicAsync(_topicName, cancellationToken);
        }

        if (!await _serviceBusAdministrationClient.SubscriptionExistsAsync(_topicName, _subscriptionName, cancellationToken))
        {
            _logger.LogInformation("Subscription {SubscriptionName} does not exist on Topic {TopicName}. Creating it.", _subscriptionName, _topicName);
            await _serviceBusAdministrationClient.CreateSubscriptionAsync(_topicName, _subscriptionName, cancellationToken);
        }

        // create the processor
        _processor = _serviceBusClient.CreateProcessor(_topicName, _subscriptionName);
        _processor.ProcessMessageAsync += ProcessorOnProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessorOnProcessErrorAsync;
        await _processor.StartProcessingAsync(cancellationToken);
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // handled by the processor, not required for us to keep listening to it.
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor is null)
        {
            return;
        }

        await _processor.StopProcessingAsync(cancellationToken);
    }

    private Task ProcessorOnProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        _logger.LogError(arg.Exception, "An error occured while handling a message.");
        return Task.CompletedTask;
    }

    private async Task ProcessorOnProcessMessageAsync(ProcessMessageEventArgs arg)
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<THandler>();
        var message = new AzureServiceBusMessage(arg.Message);
        await handler.ProcessMessageAsync(message, arg.CancellationToken);
    }
}