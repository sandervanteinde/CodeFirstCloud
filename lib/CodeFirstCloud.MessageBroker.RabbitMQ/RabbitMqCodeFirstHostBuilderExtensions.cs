using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace CodeFirstCloud.MessageBroker.RabbitMQ;

public static class RabbitMqCodeFirstHostBuilderExtensions
{
    public static void AddLocalHostRabbitMqHandlersFromEntryAssembly(this ICodeFirstHostBuilder builder)
    {
        builder.AddLocalHostRabbitMqHandlers(Assembly.GetEntryAssembly()!);
    }

    public static void AddLocalHostRabbitMqHandlers(this ICodeFirstHostBuilder builder, Assembly assembly)
    {
        var services = builder.Services;
        services.TryAddSingleton<RabbitMqConnection>();


        foreach (var item in assembly.GetTypes())
        {
            if (!item.IsAssignableTo(typeof(IServiceBusMessageHandler)))
            {
                continue;
            }

            var attr = item.GetCustomAttribute<ServiceBusHandlerAttribute>();

            if (attr is null)
            {
                throw new InvalidOperationException($"Type {item.Name} is extending {nameof(IServiceBusMessageHandler)} but is not decorated with {nameof(ServiceBusHandlerAttribute)}.");
            }

            var bindingType = typeof(RabbitMqBinding<>).MakeGenericType(item);
            builder.AddBinding(bindingType);
            builder.Services.TryAddTransient(item);
        }
    }
}