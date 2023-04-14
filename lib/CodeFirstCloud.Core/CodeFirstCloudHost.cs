using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace CodeFirstCloud;

public static class CodeFirstCloudHost
{
    public static IHost Create(Action<ICodeFirstCloudHostBuilder> codeFirstHostBuilderFn)
    {
        var builder = WebApplication.CreateBuilder();


        var codeFirstHostBuilder = new CodeFirstCloudHostBuilder(builder);
        codeFirstHostBuilderFn.Invoke(codeFirstHostBuilder);

        var app = builder.Build();

        foreach (var middlewareRegistration in codeFirstHostBuilder.MiddlewareRegistrations)
        {
            middlewareRegistration.Invoke(app);
        }

        return app;
    }
}