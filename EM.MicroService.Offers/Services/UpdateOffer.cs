using System.Text;
using EM.Contracts;
using EM.MicroService.Offers.Data;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EM.MicroService.Offers.Services;

public class UpdateOffer : BackgroundService
{
    private readonly IRabbitMqService _service;
    private readonly IServiceProvider _serviceProvider;

    public UpdateOffer(IRabbitMqService service, IServiceProvider serviceProvider)
    {
        _service = service;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            Dispose();

        _service.AddConsumerEventHandler("update_offer_queue", async (_, args) =>
        {
            string? response = null;

            if (args.Body.Length != 0)
            {
                var setting = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var offerRequest = JsonConvert.DeserializeObject<Offer>(Encoding.UTF8.GetString(args.Body.ToArray()));

                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<OffersDbContext>();

                var offer = await _context.Offers!.FirstOrDefaultAsync(o => o.Id == offerRequest!.Id);
                if (offer != null)
                {
                    offer.CostOfUnit = offerRequest!.CostOfUnit;
                    offer.LastModify = DateTime.UtcNow;
                    _context.Offers!.Update(offer);
                    await _context.SaveChangesAsync();
                    response = JsonConvert.SerializeObject(true, setting);
                }
            }

            _service.RpcProduce(response, args, "update_offer_reply");
        });

        _service.Consume("update_offer_queue");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _service.Dispose();
        GC.SuppressFinalize(this);
    }
}