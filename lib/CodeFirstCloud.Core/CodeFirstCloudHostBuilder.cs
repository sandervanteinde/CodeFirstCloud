using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CodeFirstCloud;

public class CodeFirstCloudHostBuilder : ICodeFirstCloudHostBuilder
{
    private readonly WebApplicationBuilder _hostBuilder;
    private readonly Lazy<MethodInfo> _untypedAddBindingGenericMethod;
    internal List<Action<WebApplication>> MiddlewareRegistrations { get; } = new();

    public IServiceCollection Services => _hostBuilder.Services;

    internal CodeFirstCloudHostBuilder(WebApplicationBuilder hostApplicationHostBuilder)
    {
        _hostBuilder = hostApplicationHostBuilder;
        _untypedAddBindingGenericMethod = new Lazy<MethodInfo>(() =>
            GetType()
               .GetMethod(nameof(AddBinding), BindingFlags.Instance | BindingFlags.Public, Array.Empty<Type>())
            ?? throw new InvalidOperationException("Could not find AddBinding method")
        );
    }

    public void AddBinding(Type type)
    {
        _untypedAddBindingGenericMethod
           .Value
           .MakeGenericMethod(type)
           .Invoke(this, Array.Empty<object>());
    }

    public void AddBinding<TBinding>()
        where TBinding : class, ICodeFirstCloudBinding
    {
        _hostBuilder.Services.AddTransient<TBinding>();
        _hostBuilder.Services.AddHostedService<HandlerBindingRunner<TBinding>>();
    }

    public void AddBindingInterceptor<TInterceptor>()
        where TInterceptor : class, ICodeFirstCloudBindingInterceptor
    {
        _hostBuilder.Services.AddTransient<ICodeFirstCloudBindingInterceptor, TInterceptor>();
    }

    public void Use(Action<WebApplication> appRegistration)
    {
        MiddlewareRegistrations.Add(appRegistration);
    }
}