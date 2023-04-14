using Azure.Core;
using Azure.Identity;
using CodeFirstCloud.AssemblyScanningExtensions;
using CodeFirstCloud.MessageBroker;
using CodeFirstCloud.MessageBroker.Azure;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace CodeFirstCloud;

public static class AzureServiceBusCodeFirstHostBuilderExtensions
{
    public static ICodeFirstCloudHostBuilder AddAzureServiceBusMessageHandlersFromEntryAssembly(this ICodeFirstCloudHostBuilder builder, string serviceBusName)
    {
        return builder.AddAzureServiceBusMessageHandlerFromAssembly(Assembly.GetEntryAssembly()!, serviceBusName);
    }

    public static ICodeFirstCloudHostBuilder AddAzureServiceBusMessageHandlerFromAssembly(this ICodeFirstCloudHostBuilder builder, Assembly assembly, string serviceBusName)
    {
        builder.Services
           .AddAzureClients(builder =>
            {
                builder.AddServiceBusClientWithNamespace(serviceBusName)
                   .WithCredential(sp => sp.GetRequiredService<TokenCredential>());
                builder.AddServiceBusAdministrationClientWithNamespace(serviceBusName)
                   .WithCredential(sp => sp.GetRequiredService<TokenCredential>());
            });

        foreach (var (item, _) in assembly.ScanForHandlerWithAttribute<IServiceBusMessageHandler, ServiceBusHandlerAttribute>())
        {
            var bindingType = typeof(ServiceBusSubscriptionListenerCodeFirstCloudBinding<>).MakeGenericType(item);
            builder.AddBinding(bindingType);
            builder.Services.TryAddTransient(item);
        }

        builder.Services.TryAddSingleton<TokenCredential, DefaultAzureCredential>();

        return builder;
    }
}