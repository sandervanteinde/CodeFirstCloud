using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace CodeFirstCloud.MessageBroker;

public abstract class BaseMessageBrokerBinding<THandler>
    where THandler : IServiceBusMessageHandler
{
    private static Delegate CreateUnknownBodyEndpoint()
    {
        return async ([FromBody] JsonNode body, [FromServices] THandler handler, CancellationToken cancellationToken) =>
        {
            var testingMessage = new TestingServiceBusMessage(body);
            await handler.HandleAsync(testingMessage, cancellationToken);
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