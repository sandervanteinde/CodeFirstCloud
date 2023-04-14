namespace CodeFirstCloud.MessageBroker;

public interface IServiceBusMessage
{
    TBody ReadFromJson<TBody>();
}