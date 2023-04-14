using Microsoft.AspNetCore.Builder;
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
               .GetMethod(nameof(AddInvokableAsEndpoint), BindingFlags.Static | BindingFlags.NonPublic)!;

            var parameters = new object[]
            {
                app
            };

            foreach (var service in services.Select(service => service.ImplementationType).Where(type => type is not null && type.IsAssignableTo(typeof(IInvokableInDevelopment))))
            {
                var genericMethod = method.MakeGenericMethod(service!);
                genericMethod.Invoke(null, parameters);
            }
        });
        return builder;
    }

    private static void AddInvokableAsEndpoint<TInvokable>(WebApplication app)
        where TInvokable : IInvokableInDevelopment
    {
        TInvokable.AddEndpoint(app);
    }
}