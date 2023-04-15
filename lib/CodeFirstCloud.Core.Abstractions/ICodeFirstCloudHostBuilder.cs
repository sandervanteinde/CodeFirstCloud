using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CodeFirstCloud;

public interface ICodeFirstCloudHostBuilder
{
    IServiceCollection Services { get; }
    void AddBinding(Type type);

    void AddBinding<TBinding>()
        where TBinding : class, ICodeFirstCloudBinding;

    void AddBindingInterceptor<TInterceptor>()
        where TInterceptor : class, ICodeFirstCloudBindingInterceptor;

    void Use(Action<WebApplication> app);
}