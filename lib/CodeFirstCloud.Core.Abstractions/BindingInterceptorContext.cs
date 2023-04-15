namespace CodeFirstCloud;

public class BindingInterceptorContext
{
    public required ICodeFirstCloudBinding ActualBinding { get; init; }
    public required CancellationToken CancellationToken { get; init; }
}