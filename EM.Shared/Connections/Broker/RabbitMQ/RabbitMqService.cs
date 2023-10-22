using System.Text;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using EM.Shared.Connections.Broker.RabbitMQ.Model;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using EM.Shared.Connections.Broker.RabbitMQ.Clients;

namespace EM.Shared.Connections.Broker.RabbitMQ;

public class RabbitMqService : IRabbitMqService
{
    public void AddConsumerEventHandler(string queue, EventHandler<BasicDeliverEventArgs> handler)
        => ConsumerHandlerCollection.Add(queue, handler);
    private readonly Dictionary<string, EventHandler<BasicDeliverEventArgs>> ConsumerHandlerCollection = new();
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly Dictionary<string, EventingBasicConsumer> Consumers = new();
    private readonly Dictionary<string, IBasicProperties?> Producers = new();
    private readonly Dictionary<string, RabbitMqRpcClient?> RpcClients = new();

    public RabbitMqService(IOptions<IRabbitMqOptions> options)
    {
        var factory = new ConnectionFactory()
        {
            HostName = options.Value.HostName,
            UserName = options.Value.User,
            Password = options.Value.Password
        };

        _connection = factory.CreateConnection();

        _channel = _connection.CreateModel();

        _channel.BasicQos(
            prefetchSize: 0,
            prefetchCount: options.Value.PrefetchCount,
            global: false);

        if (options.Value.Producers != null && options.Value.Producers.Count > 0)
            CreateProducers(options.Value.Producers);

        if (options.Value.Consumers != null && options.Value.Consumers.Count > 0)
            CreateConsumers(options.Value.Consumers);

        if (options.Value.RpcCalls != null && options.Value.RpcCalls.Count > 0)
            CreateRpcCalls(options.Value.RpcCalls);

    }

    private void CreateRpcCalls(List<RpcCall> rpcCalls)
    {
        foreach (var rpcCall in rpcCalls)
        {
            RabbitMqRpcClient? rpcClient = null;
            try
            {
                rpcClient = new RabbitMqRpcClient(_channel, rpcCall);
            }
            catch (Exception)
            {
                // Ловим ошибки из создания клиента
            }
            finally
            {
                RpcClients.Add(rpcCall.Exchange!.Name!, rpcClient);
            }
        }
    }

    private void CreateConsumers(List<Consumer> consumers)
    {
        foreach (var consumer in consumers)
        {
            // var queueName = _channel.QueueDeclare().QueueName;
            try
            {
                QueueDeclare(_channel, consumer.Queue!);

                _channel.ExchangeDeclare(
                    exchange: consumer.Exchange!.Name,
                    type: consumer.Exchange!.Type);

                _channel.QueueBind(
                    queue: consumer.Queue!.Name,
                    exchange: consumer.Exchange!.Name,
                    // TODO Теоретически возможно стоит задавать
                    routingKey: string.Empty);

            }
            catch (Exception)
            {
                // Ловим ошибку создания уже имеющейся очереди с другими параметрами
            }
            finally
            {
                // TODO возможно в будущем нужно задавать тип consumer
                var _consumer = new EventingBasicConsumer(_channel);

                _channel.BasicConsume(
                    queue: consumer.Queue!.Name,
                    autoAck: consumer.AutoAck,
                    consumerTag: consumer.ConsumerTag,
                    noLocal: consumer.NoLocal,
                    exclusive: consumer.Exclusive,
                    arguments: consumer.Arguments,
                    consumer: _consumer);

                Consumers.Add(consumer.Queue.Name!, _consumer);
            }
        }
    }

    private void CreateProducers(List<Producer> producers)
    {
        foreach (var producer in producers)
        {
            try
            {
                QueueDeclare(_channel, producer.Queue!);
            }
            catch (Exception)
            {
                // Ловим ошибку создания уже имеющейся очереди с другими параметрами
            }
            finally
            {
                IBasicProperties? _props = null;

                // TODO реализовать добавление Properties, если они есть в настройках.
                // if (producer.Props != null)
                // {
                //     _props = _channel.CreateBasicProperties();
                // }

                Producers.Add(producer.Queue!.Name!, _props);
            }
        }
    }

    public void Produce(string message, string queue)
    {
        // TODO нужно ловить ошибку, когда никого не найдется 
        var producer = Producers.FirstOrDefault(pr => pr.Key == queue);

        var messageBytes = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: "",
            routingKey: queue,
            basicProperties: producer.Value,
            body: messageBytes);
    }

    public void RpcProduce(string message, BasicDeliverEventArgs args, string exchange)
    {    
        var messageBytes = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: exchange,
            routingKey: args.BasicProperties.ReplyTo,
            basicProperties: args.BasicProperties,
            body: messageBytes);
    }

    public void Consume(string queue)
    {
        // TODO нужно ловить ошибку, когда никого не найдется 
        var consumer = Consumers.GetValueOrDefault(queue);

        consumer!.Received += (obj, args) =>
        {
            var handler = ConsumerHandlerCollection.First(q => q.Key == queue).Value;
            handler.Invoke(null, args);
            _channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
        };
    }

    public Task<string> RpcCallAsync(string exchange, string message)
    {
        var rpcClient = RpcClients.GetValueOrDefault(exchange);
        return rpcClient!.CallAsync(message);
    }

    internal static void QueueDeclare(IModel _channel, Queue queue)
        => _channel.QueueDeclare(
            queue: queue.Name,
            durable: queue.Durable,
            exclusive: queue.Exclusive,
            autoDelete: queue.AutoDelete,
            arguments: queue.Arguments);

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
        GC.SuppressFinalize(this);
    }
}