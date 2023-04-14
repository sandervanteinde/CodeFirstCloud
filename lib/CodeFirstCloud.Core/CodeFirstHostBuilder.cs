using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CodeFirstCloud;

public class CodeFirstHostBuilder : ICodeFirstHostBuilder
{
    private readonly WebApplicationBuilder _hostBuilder;
    private readonly Lazy<MethodInfo> _untypedAddBindingGenericMethod;

    public IServiceCollection Services => _hostBuilder.Services;

    internal CodeFirstHostBuilder(WebApplicationBuilder hostApplicationHostBuilder)
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
}