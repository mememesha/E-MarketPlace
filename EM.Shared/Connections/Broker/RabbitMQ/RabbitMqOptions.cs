using EM.Shared.Connections.Broker.RabbitMQ.Model;
using EM.Shared.Connections.Broker.RabbitMQ.Clients;

namespace EM.Shared.Connections.Broker.RabbitMQ;
public class IRabbitMqOptions
{
    // Connection settings
    public string? HostName { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }

    // Work settings
    public ushort PrefetchCount { get; set; }
    public List<Producer>? Producers { get; set; }
    public List<Consumer>? Consumers { get; set; }
    public List<RpcCall>? RpcCalls { get; set; }
}