using System.Text;
using EM.MicroService.Offers.Data;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EM.MicroService.Offers.Services;

public class GetNewOffers : BackgroundService
{
    private readonly IRabbitMqService _service;
    private readonly IServiceProvider _serviceProvider;

    public GetNewOffers(IRabbitMqService service, IServiceProvider serviceProvider)
    {
        _service = service;
        _serviceProvider = serviceProvider;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            Dispose();

        _service.AddConsumerEventHandler("get_new_offers_queue", async (_, args) =>
        {
            bool IsSale = false;

            if (args.Body.Length != 0)
                IsSale = JsonConvert.DeserializeObject<bool>(Encoding.UTF8.GetString(args.Body.ToArray()));

            using var scope = _serviceProvider.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<OffersDbContext>();

            var newOffers = await _context.Offers!.AsNoTracking()
                                    .Where(o => o.OfferDescription!.IsSale == IsSale)
                                    .Take(6)
                                    .ToListAsync();

            var setting = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            _service.RpcProduce(JsonConvert.SerializeObject(newOffers, setting), args, "get_new_offers_reply");
        });

        _service.Consume("get_new_offers_queue");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _service.Dispose();
        GC.SuppressFinalize(this);
    }
}