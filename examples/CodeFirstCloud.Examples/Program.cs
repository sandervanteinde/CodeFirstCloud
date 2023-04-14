// See https://aka.ms/new-console-template for more information

using CodeFirstCloud;
using Microsoft.Extensions.Hosting;

var host = CodeFirstCloudHost.Create(builder =>
    builder.AddTimerHandlerFromEntryAssembly()
       .AddAzureServiceBusMessageHandlersFromEntryAssembly("sander-testing-servicebus"));

await host.RunAsync();