namespace CodeFirstCloud;

public interface ICodeFirstCloudBindingInterceptor
{
    Task StartAsync(Func<BindingInterceptorContext, Task> next, BindingInterceptorContext context);
    Task ExecuteAsync(Func<BindingInterceptorContext, Task> next, BindingInterceptorContext context);
    Task StopAsync(Func<BindingInterceptorContext, Task> next, BindingInterceptorContext context);
}