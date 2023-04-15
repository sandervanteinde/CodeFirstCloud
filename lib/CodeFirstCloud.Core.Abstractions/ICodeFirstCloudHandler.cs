namespace CodeFirstCloud;

public interface ICodeFirstCloudHandler<in T1, in T2, in T3>
{
    Task HandleAsync(T1 valueOne, T2 valueTwo, T3 valueThree, CancellationToken cancellationToken);
}

public interface ICodeFirstCloudHandler<in T1, in T2>
{
    Task HandleAsync(T1 valueOne, T2 valueTwo, CancellationToken cancellationToken);
}

public interface ICodeFirstCloudHandler<in T1>
{
    Task HandleAsync(T1 serviceBusMessage, CancellationToken cancellationToken);
}

public interface ICodeFirstCloudHandler
{
    Task HandleAsync(CancellationToken cancellationToken);
}