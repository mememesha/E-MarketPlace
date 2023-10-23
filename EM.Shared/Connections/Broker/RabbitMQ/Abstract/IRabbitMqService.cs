using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EM.Shared.Connections.Broker.RabbitMQ.Abstract;
public interface IRabbitMqService : IDisposable
{
    void AddConsumerEventHandler(string queue, EventHandler<BasicDeliverEventArgs> handler);
    void Produce(string message, string queue);
    void RpcProduce(string? message, BasicDeliverEventArgs args, string exchange);
    void Consume(string queue);
    Task<string> RpcCallAsync(string exchange, string? message = null);
}