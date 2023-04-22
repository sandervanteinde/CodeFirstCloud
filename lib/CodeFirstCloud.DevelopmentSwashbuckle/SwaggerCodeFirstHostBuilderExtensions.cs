using CodeFirstCloud.AssemblyScanningExtensions;
using CodeFirstCloud.Constructable;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace CodeFirstCloud;

public static class SwaggerCodeFirstHostBuilderExtensions
{
    public static ICodeFirstCloudHostBuilder AddDevelopmentSwagger(this ICodeFirstCloudHostBuilder builder)
    {
        var services = builder.Services;
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        builder.Use(app =>
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            var method = typeof(SwaggerCodeFirstHostBuilderExtensions)
               .GetMethod(nameof(AddInvokableAsEndpointWithoutArgs), BindingFlags.Static | BindingFlags.NonPublic)!;

            var parameters = new object[]
            {
                app
            };

            var servicesOfType = services
               .Where(service => service.ImplementationType is not null)
               .Select(service => service.ImplementationType!);

            foreach (var service in servicesOfType)
            {
                MethodInfo? registerMethod = null;

                if (service.IsAssignableTo(typeof(ICodeFirstCloudHandler)))
                {
                    registerMethod = method.MakeGenericMethod(service);
                }
                else if (service.IsAssignableToGenericType(typeof(ICodeFirstCloudHandler<>), out var @interface))
                {
                    if (service.IsAssignableToGenericType(typeof(IConstructable<,>), out var constructable) && constructable.GetGenericArguments()[1] == @interface.GetGenericArguments()[0])
                    {
                        registerMethod = typeof(SwaggerCodeFirstHostBuilderExtensions)
                           .GetMethod(nameof(AddInvokableAsEndpointWithConstructable), BindingFlags.Static | BindingFlags.NonPublic)!
                           .MakeGenericMethod(service, @interface.GetGenericArguments()[0], constructable.GetGenericArguments()[0]);
                    }
                    else
                    {
                        registerMethod = typeof(SwaggerCodeFirstHostBuilderExtensions)
                           .GetMethod(nameof(AddInvokableAsEndpointWithOneArg), BindingFlags.Static | BindingFlags.NonPublic)!
                           .MakeGenericMethod(service, @interface.GetGenericArguments()[0]);
                    }
                }

                registerMethod?.Invoke(null, parameters);
            }
        });
        return builder;
    }

    private static void AddInvokableAsEndpointWithoutArgs<TInvokable>(IEndpointRouteBuilder app)
        where TInvokable : ICodeFirstCloudHandler
    {
        var path = ParseClassAsRoute(typeof(TInvokable));
        app.MapPost(path, async ([FromServices] ICodeFirstCloudHandlerPipeline<TInvokable> pipeline, CancellationToken cancellationToken) => await pipeline.HandleAsync(cancellationToken));
    }

    private static void AddInvokableAsEndpointWithOneArg<TInvokable, T1>(IEndpointRouteBuilder app)
        where TInvokable : ICodeFirstCloudHandler<T1>
    {
        var path = ParseClassAsRoute(typeof(TInvokable));
        app.MapPost(path, async ([FromServices] ICodeFirstCloudHandlerPipeline<TInvokable, T1> pipeline, [FromBody] T1 body, CancellationToken cancellationToken) => await pipeline.HandleAsync(body, cancellationToken));
    }

    private static void AddInvokableAsEndpointWithConstructable<TInvokable, T1, TBody>(IEndpointRouteBuilder app)
        where TInvokable : ICodeFirstCloudHandler<T1>, IConstructable<TBody, T1>
    {
        var path = ParseClassAsRoute(typeof(TInvokable));
        app.MapPost(path, async ([FromServices] ICodeFirstCloudHandlerPipeline<TInvokable, T1> pipeline, [FromBody] TBody body, CancellationToken cancellationToken) =>
        {
            var asT1 = TInvokable.CreateTestable(body);
            await pipeline.HandleAsync(asT1, cancellationToken);
        });
    }

    private static string ParseClassAsRoute(Type type)
    {
        return $"/{type.Namespace?.Replace('.', '/') ?? "RootNamespace"}/{type.Name}";
    }
}