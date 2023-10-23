using System.Text;
using EM.MicroService.Reserves.Data;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EM.MicroService.Offers.Services;

public class GetReservesByOfferId : BackgroundService
{
    private readonly IRabbitMqService _service;
    private readonly IServiceProvider _serviceProvider;

    public GetReservesByOfferId(IRabbitMqService service, IServiceProvider serviceProvider)
    {
        _service = service;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            Dispose();

        _service.AddConsumerEventHandler("get_reserves_by_offer_id_queue", async (_, args) =>
        {
            string? response = null;

            if (args.Body.Length != 0)
            {
                var id = Guid.Parse(Encoding.UTF8.GetString(args.Body.ToArray()));

                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ReservesDbContext>();

                var reserve = await _context.Reserves!.Where(r => !r.IsApproved && !r.IsDeleted)
                                                        .FirstOrDefaultAsync(r => r.OfferId == id);
                if (reserve != null)
                {
                    var setting = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };
                    response = JsonConvert.SerializeObject(reserve, setting);
                }
            }

            _service.RpcProduce(response, args, "get_reserves_by_offer_id_reply");
        });

        _service.Consume("get_reserves_by_offer_id_queue");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _service.Dispose();
        GC.SuppressFinalize(this);
    }
}