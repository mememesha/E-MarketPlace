namespace EM.Shared.Connections.Broker.RabbitMQ.Model;
public class RpcCall
{
    public Exchange? Exchange { get; set; }
    public Consumer? Consumer { get; set; }
}