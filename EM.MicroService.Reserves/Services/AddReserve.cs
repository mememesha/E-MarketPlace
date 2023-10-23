using System.Text;
using EM.Contracts;
using EM.MicroService.Reserves.Data;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EM.MicroService.Offers.Services;

public class AddReserve : BackgroundService
{
    private readonly IRabbitMqService _service;
    private readonly IServiceProvider _serviceProvider;

    public AddReserve(IRabbitMqService service, IServiceProvider serviceProvider)
    {
        _service = service;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            Dispose();

        _service.AddConsumerEventHandler("add_reserve_queue", async (_, args) =>
        {
            string? response = null;

            if (args.Body.Length != 0)
            {
                var reserve = JsonConvert.DeserializeObject<Reserve>(Encoding.UTF8.GetString(args.Body.ToArray()));

                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ReservesDbContext>();

                if (reserve != null)
                {
                    _context.Reserves!.Add(reserve);
                    await _context.SaveChangesAsync();

                    response = JsonConvert.SerializeObject(true);
                }
            }

            _service.RpcProduce(response, args, "add_reserve_reply");
        });

        _service.Consume("add_reserve_queue");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _service.Dispose();
        GC.SuppressFinalize(this);
    }
}