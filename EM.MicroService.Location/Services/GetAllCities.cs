using System.Text.Json;
using EM.MicroService.Location.Data;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.EntityFrameworkCore;

namespace EM.MicroService.Location.Services;

public class GetAllCities : BackgroundService
{
    private readonly IRabbitMqService _service;
    private readonly IServiceProvider _serviceProvider;

    public GetAllCities(IRabbitMqService service, IServiceProvider serviceProvider)
    {
        _service = service;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            Dispose();

        _service.AddConsumerEventHandler("get_all_cities_queue", async (_, args) =>
        {
            using var scope = _serviceProvider.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<LocationDbContext>();

            var cities = await _context.Places!.Select(p => p.City).Distinct().ToListAsync();
            cities.Sort();

            string response = JsonSerializer.Serialize(cities);

            _service.RpcProduce(response!, args, "get_all_cities_reply");
        });

        _service.Consume("get_all_cities_queue");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _service.Dispose();
        GC.SuppressFinalize(this);
    }
}