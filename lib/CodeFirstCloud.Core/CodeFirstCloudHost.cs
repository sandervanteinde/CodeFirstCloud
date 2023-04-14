using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace CodeFirstCloud;

public static class CodeFirstCloudHost
{
    public static IHost Create(Action<ICodeFirstHostBuilder> codeFirstHostBuilderFn)
    {
        var builder = WebApplication.CreateBuilder();


        var codeFirstHostBuilder = new CodeFirstHostBuilder(builder);
        codeFirstHostBuilderFn.Invoke(codeFirstHostBuilder);

        var app = builder.Build();

        return app;
    }
}