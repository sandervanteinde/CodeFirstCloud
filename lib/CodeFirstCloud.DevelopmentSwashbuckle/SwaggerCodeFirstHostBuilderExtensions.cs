using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
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
                else if (IsAssignableToGenericType(service, typeof(ICodeFirstCloudHandler<>), out var @interface))
                {
                    registerMethod = typeof(SwaggerCodeFirstHostBuilderExtensions)
                       .GetMethod(nameof(AddInvokableAsEndpointWithOneArg), BindingFlags.Static | BindingFlags.NonPublic)!
                       .MakeGenericMethod(service, @interface.GetGenericArguments()[0]);
                }

                registerMethod?.Invoke(null, parameters);
            }
        });
        return builder;
    }

    private static void AddInvokableAsEndpointWithoutArgs<TInvokable>(WebApplication app)
        where TInvokable : ICodeFirstCloudHandler
    {
        var path = ParseClassAsRoute(typeof(TInvokable));
        app.MapPost(path, async ([FromServices] ICodeFirstCloudHandlerPipeline<TInvokable> pipeline, CancellationToken cancellationToken) => await pipeline.HandleAsync(cancellationToken));
    }

    private static void AddInvokableAsEndpointWithOneArg<TInvokable, T1>(WebApplication app)
        where TInvokable : ICodeFirstCloudHandler<T1>
    {
        var path = ParseClassAsRoute(typeof(TInvokable));
        app.MapPost(path, async ([FromServices] ICodeFirstCloudHandlerPipeline<TInvokable, T1> pipeline, [FromBody] T1 body, CancellationToken cancellationToken) => await pipeline.HandleAsync(body, cancellationToken));
    }

    private static string ParseClassAsRoute(Type type)
    {
        return $"/{type.Namespace?.Replace('.', '/') ?? "RootNamespace"}/{type.Name}";
    }

    private static bool IsAssignableToGenericType(Type givenType, Type genericType, [NotNullWhen(true)] out Type? @interface)
    {
        @interface = null;
        var interfaceTypes = givenType.GetInterfaces();

        foreach (var it in interfaceTypes)
        {
            if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
            {
                @interface = it;
                return true;
            }
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
        {
            @interface = givenType;
            return true;
        }

        var baseType = givenType.BaseType;

        if (baseType == null)
        {
            return false;
        }

        return IsAssignableToGenericType(baseType, genericType, out @interface);
    }
}