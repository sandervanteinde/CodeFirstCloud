using CodeFirstCloud.AssemblyScanningExtensions;
using CodeFirstCloud.Timers;
using CodeFirstCloud.Timers.InProcess;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace CodeFirstCloud;

public static class TimerCodeFirstHostBuilderExtensions
{
    public static ICodeFirstCloudHostBuilder AddTimerHandlerFromEntryAssembly(this ICodeFirstCloudHostBuilder builder)
    {
        var entryAssembly = Assembly.GetEntryAssembly()
                            ?? throw new InvalidOperationException("No entry assembly found");

        foreach (var (handler, attr) in entryAssembly.ScanForClassesWithAttribute<ITimerHandler, TimerAttribute>())
        {
            builder.AddBinding(typeof(TimerCodeFirstCloudBinding<>).MakeGenericType(handler));
            builder.Services.AddTransient(handler);
        }

        return builder;
    }
}