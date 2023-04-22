using CodeFirstCloud.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CodeFirstCloud;

public static class CodeFirstCloudHost
{
    public static void AddCodeFirstCloud(this WebApplicationBuilder builder, Action<ICodeFirstCloudHostBuilder> codeFirstHostBuilderFn)
    {
        var codeFirstHostBuilder = new CodeFirstCloudHostBuilder(builder);
        codeFirstHostBuilderFn.Invoke(codeFirstHostBuilder);
        builder.Services.AddTransient(typeof(ICodeFirstCloudHandlerPipeline<>), typeof(DefaultCodeFirstCloudHandlerPipeline<>));
        builder.Services.AddTransient(typeof(ICodeFirstCloudHandlerPipeline<,>), typeof(DefaultCodeFirstCloudHandlerPipeline<,>));
        codeFirstHostBuilder.AddBindingInterceptor<ExceptionBindingInterceptor>();
        builder.Services.AddSingleton(codeFirstHostBuilder);
    }

    public static void UseCodeFirstCloud(this WebApplication webApplication)
    {
        var codeFirstHostBuilder = webApplication.Services.GetRequiredService<CodeFirstCloudHostBuilder>();

        foreach (var middlewareRegistration in codeFirstHostBuilder.MiddlewareRegistrations)
        {
            middlewareRegistration.Invoke(webApplication);
        }
    }

    public static IHost Create(Action<ICodeFirstCloudHostBuilder> codeFirstHostBuilderFn)
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddCodeFirstCloud(codeFirstHostBuilderFn);

        var app = builder.Build();

        app.UseCodeFirstCloud();

        return app;
    }
}