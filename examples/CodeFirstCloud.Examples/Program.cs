// See https://aka.ms/new-console-template for more information

using CodeFirstCloud;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder();

builder.AddCodeFirstCloud(codeFirstCloudBuilder =>
    codeFirstCloudBuilder.AddTimerHandlerFromEntryAssembly()
       .AddAzureServiceBusMessageHandlersFromEntryAssembly("sander-testing-servicebus")
       .AddDevelopmentSwagger());

var app = builder.Build();

app.UseCodeFirstCloud();

await app.RunAsync();

