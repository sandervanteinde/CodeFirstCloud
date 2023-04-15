using CodeFirstCloud.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CodeFirstCloud;

public static class CodeFirstCloudHost
{
    public static IHost Create(Action<ICodeFirstCloudHostBuilder> codeFirstHostBuilderFn)
    {
        var builder = WebApplication.CreateBuilder();


        var codeFirstHostBuilder = new CodeFirstCloudHostBuilder(builder);
        codeFirstHostBuilderFn.Invoke(codeFirstHostBuilder);
        builder.Services.AddTransient(typeof(ICodeFirstCloudHandlerPipeline<>), typeof(DefaultCodeFirstCloudHandlerPipeline<>));
        builder.Services.AddTransient(typeof(ICodeFirstCloudHandlerPipeline<,>), typeof(DefaultCodeFirstCloudHandlerPipeline<,>));
        builder.Services.AddTransient(typeof(ICodeFirstCloudHandlerPipeline<,,>), typeof(DefaultCodeFirstCloudHandlerPipeline<,,>));
        builder.Services.AddTransient(typeof(ICodeFirstCloudHandlerPipeline<,,,>), typeof(DefaultCodeFirstCloudHandlerPipeline<,,,>));
        codeFirstHostBuilder.AddBindingInterceptor<ExceptionBindingInterceptor>();

        var app = builder.Build();

        foreach (var middlewareRegistration in codeFirstHostBuilder.MiddlewareRegistrations)
        {
            middlewareRegistration.Invoke(app);
        }

        return app;
    }
}