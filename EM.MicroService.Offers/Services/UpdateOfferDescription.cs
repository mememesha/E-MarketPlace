using System.Text;
using EM.Contracts;
using EM.MicroService.Offers.Data;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EM.MicroService.Offers.Services;

public class UpdateOfferDescription : BackgroundService
{
    private readonly IRabbitMqService _service;
    private readonly IServiceProvider _serviceProvider;

    public UpdateOfferDescription(IRabbitMqService service, IServiceProvider serviceProvider)
    {
        _service = service;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            Dispose();

        _service.AddConsumerEventHandler("update_offerdescription_queue", async (_, args) =>
        {
            string? response = null;

            if (args.Body.Length != 0)
            {
                var setting = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var offerDescriptionRequest = JsonConvert.DeserializeObject<OfferDescription>(Encoding.UTF8.GetString(args.Body.ToArray()));

                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<OffersDbContext>();

                var offerDescription = await _context.OfferDescriptions!.FirstOrDefaultAsync(o => o.Id == offerDescriptionRequest!.Id);
                if (offerDescription != null)
                {
                    offerDescription.Title = offerDescriptionRequest!.Title;
                    offerDescription.Description = offerDescriptionRequest!.Description;
                    offerDescription.CategoryId = offerDescriptionRequest!.Category!.Id;
                    offerDescription.IsSale = offerDescriptionRequest!.IsSale;
                    offerDescription.LastModify = DateTime.UtcNow;
                    _context.OfferDescriptions!.Update(offerDescription);
                    await _context.SaveChangesAsync();
                    response = JsonConvert.SerializeObject(true, setting);
                }
            }

            _service.RpcProduce(response, args, "update_offerdescription_reply");
        });

        _service.Consume("update_offerdescription_queue");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _service.Dispose();
        GC.SuppressFinalize(this);
    }
}