using System.Collections.Concurrent;
using System.Text;
using EM.Shared.Connections.Broker.RabbitMQ.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EM.Shared.Connections.Broker.RabbitMQ.Clients;

public class RabbitMqRpcClient
{
    private readonly IModel _channel;
    private readonly RpcCall _rpcCall;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> callbackMapper = new();
    private readonly string RoutingKey = Guid.NewGuid().ToString();

    public RabbitMqRpcClient(IModel channel, RpcCall rpcCall)
    {
        _channel = channel;
        _rpcCall = rpcCall;

        CreateConsumer(_rpcCall.Consumer!);
    }

    private void CreateConsumer(Consumer consumer)
    {
        var queueName = _channel.QueueDeclare().QueueName;

        try
        {
            // RabbitMqService.QueueDeclare(channel, consumer.Queue!);

            _channel.ExchangeDeclare(
                exchange: consumer.Exchange!.Name,
                type: consumer.Exchange!.Type);

            _channel.QueueBind(
                queue: queueName,
                exchange: consumer.Exchange!.Name,
                routingKey: RoutingKey);
        }
        catch (Exception)
        {
            // Ловим ошибки создания обменника и очереди
        }
        finally
        {
            // TODO возможно в будущем нужно задавать тип Consumer из настроек
            var _consumer = new EventingBasicConsumer(_channel);

            _consumer.Received += (model, args) =>
            {
                if (!callbackMapper.TryRemove(args.BasicProperties.CorrelationId, out TaskCompletionSource<string>? tcs))
                    return;

                var body = args.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                tcs.TrySetResult(response);
                _channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(
                queue: queueName,
                autoAck: consumer.AutoAck,
                consumerTag: consumer.ConsumerTag,
                noLocal: consumer.NoLocal,
                exclusive: consumer.Exclusive,
                arguments: consumer.Arguments,
                consumer: _consumer);

        }
    }

    public Task<string> CallAsync(string? message, CancellationToken cancellationToken = default)
    {
        _channel.ExchangeDeclare(
                exchange: _rpcCall.Exchange!.Name,
                type: _rpcCall.Exchange!.Type);

        var props = _channel.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        props.CorrelationId = correlationId;
        props.ReplyTo = RoutingKey;

        var messageBytes = message != null ? Encoding.UTF8.GetBytes(message) : null;

        var tcs = new TaskCompletionSource<string>();
        callbackMapper.TryAdd(correlationId, tcs);

        _channel.BasicPublish(
            exchange: _rpcCall.Exchange!.Name,
            routingKey: string.Empty,
            basicProperties: props,
            body: messageBytes);

        cancellationToken.Register(() => callbackMapper.TryRemove(correlationId, out var tmp));
        return tcs.Task;
    }
}