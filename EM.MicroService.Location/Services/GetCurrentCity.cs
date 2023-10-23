using EM.MicroService.Location.Data;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.EntityFrameworkCore;

namespace EM.MicroService.Location.Services;

public class GetCurrentCity : BackgroundService
{
    private readonly IRabbitMqService _service;
    private readonly IServiceProvider _serviceProvider;

    public GetCurrentCity(IRabbitMqService service, IServiceProvider serviceProvider)
    {
        _service = service;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            Dispose();

        _service.AddConsumerEventHandler("get_current_city_queue", async (_, args) =>
        {
            using var scope = _serviceProvider.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<LocationDbContext>();

            string response = _context.Places!.Any()
                ? (await _context.Places!.ToArrayAsync())[new Random().Next(0, _context.Places!.Count() - 1)].City!
                : "Россия";

            _service.RpcProduce(response!, args, "get_current_city_reply");
        });

        _service.Consume("get_current_city_queue");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _service.Dispose();
        GC.SuppressFinalize(this);
    }
}