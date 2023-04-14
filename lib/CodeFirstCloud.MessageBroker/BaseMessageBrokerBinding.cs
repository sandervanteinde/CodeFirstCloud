using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json.Nodes;

namespace CodeFirstCloud.MessageBroker;

public abstract class BaseMessageBrokerBinding<THandler> : IInvokableInDevelopment
    where THandler : IServiceBusMessageHandler
{
    static void IInvokableInDevelopment.AddEndpoint(WebApplication app)
    {
        var jsonType = GetJsonServiceBusMessageHandlerType(typeof(THandler));
        var attr = typeof(THandler).GetCustomAttribute<ServiceBusHandlerAttribute>()!;
        var path = $"/send-message-broker-message/{attr.TopicName}/{attr.SubscriptionName}";


        app.MapPost(path, jsonType is not null 
            ? CreateJsonSupportedEndpoint(jsonType)
            : CreateUnknownBodyEndpoint()
        )
           .WithTags("MessageBroker");
    }

    private static Delegate CreateJsonSupportedEndpoint(Type type)
    {
        var method = typeof(BaseMessageBrokerBinding<THandler>)
           .GetMethod(nameof(BaseMessageBrokerBinding<THandler>.CreateJsonSupportedEndpoint), BindingFlags.Static | BindingFlags.NonPublic, Array.Empty<Type>())!
           .MakeGenericMethod(type)
           .Invoke(null, Array.Empty<object>());

        return (Delegate)method!;
    }

    private static Delegate CreateJsonSupportedEndpoint<TBody>()
        where TBody : notnull
    {
        return async ([FromBody] TBody body, [FromServices] THandler handler, CancellationToken cancellationToken) =>
        {
            var testingServiceBusMessage = new TestingServiceBusMessage(body);
            await handler.ProcessMessageAsync(testingServiceBusMessage, cancellationToken);
        };
    }

    private static Delegate CreateUnknownBodyEndpoint()
    {
        return async ([FromBody] JsonNode body, [FromServices] THandler handler, CancellationToken cancellationToken) =>
        {
            var testingMessage = new TestingServiceBusMessage(body);
            await handler.ProcessMessageAsync(testingMessage, cancellationToken);
        };
    }

    private static Type? GetJsonServiceBusMessageHandlerType(Type type)
    {
        if (type is { IsAbstract: true, GenericTypeArguments: [Type genericType] })
        {
            if (type.GetGenericTypeDefinition() == typeof(JsonServiceBusMessageHandler<>))
            {
                return genericType;
            }
        }

        if (type.BaseType is Type baseType)
        {
            return GetJsonServiceBusMessageHandlerType(baseType);
        }

        return null;
    }
}