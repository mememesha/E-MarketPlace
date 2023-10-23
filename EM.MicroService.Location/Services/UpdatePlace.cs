using System.Text;
using EM.Contracts;
using EM.MicroService.Location.Data;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EM.MicroService.Location.Services;

public class UpdatePlace : BackgroundService
{
    private readonly IRabbitMqService _service;
    private readonly IServiceProvider _serviceProvider;

    public UpdatePlace(IRabbitMqService service, IServiceProvider serviceProvider)
    {
        _service = service;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            Dispose();

        _service.AddConsumerEventHandler("update_place_queue", async (_, args) =>
        {
            string? response = null;

            if (args.Body.Length != 0)
            {
                var setting = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var placeRequest = JsonConvert.DeserializeObject<Place>(Encoding.UTF8.GetString(args.Body.ToArray()));

                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<LocationDbContext>();

                var place = await _context.Places!.FirstOrDefaultAsync(o => o.Id == placeRequest!.Id);
                if (place != null)
                {
                    place.Title = placeRequest!.Title;
                    place.Description = placeRequest!.Description;
                    place.Region = placeRequest!.Region;
                    place.City = placeRequest!.City;
                    place.Address = placeRequest!.Address;
                    place.LastModify = DateTime.UtcNow;
                    _context.Places!.Update(place);
                    await _context.SaveChangesAsync();
                    response = JsonConvert.SerializeObject(true, setting);
                }
            }

            _service.RpcProduce(response, args, "update_place_reply");
        });

        _service.Consume("update_place_queue");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _service.Dispose();
        GC.SuppressFinalize(this);
    }
}