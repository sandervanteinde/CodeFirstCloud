using Microsoft.Extensions.DependencyInjection;

namespace CodeFirstCloud;

public interface ICodeFirstHostBuilder
{
    IServiceCollection Services { get; }
    void AddBinding(Type type);

    void AddBinding<TBinding>()
        where TBinding : class, ICodeFirstCloudBinding;
}