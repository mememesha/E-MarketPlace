namespace EM.Shared.Connections.Broker.RabbitMQ.Model;
public class Consumer
{
    public Exchange? Exchange { get; set; }
    public Queue? Queue { get; set; }
    public bool AutoAck { get; set; }
    public string ConsumerTag { get; set; } = "";
    public bool NoLocal { get; set; }
    public bool Exclusive { get; set; }
    public Dictionary<string, object>? Arguments { get; set; }
}