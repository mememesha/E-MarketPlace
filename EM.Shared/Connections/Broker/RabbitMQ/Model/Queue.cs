namespace EM.Shared.Connections.Broker.RabbitMQ.Model;
public class Queue
{
    public string? Name { get; set; }
    public bool Durable { get; set; } = true; // Чтобы не заморачиваться с инициализацией
    public bool Exclusive { get; set; }
    public bool AutoDelete { get; set; }
    public Dictionary<string,object>? Arguments { get; set; }
}