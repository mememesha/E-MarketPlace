using System.Text;
using Newtonsoft.Json;
using EM.MicroService.Location.Data;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.EntityFrameworkCore;

namespace EM.MicroService.Location.Services;

public class GetPlaceById : BackgroundService
{
    private readonly IRabbitMqService _service;
    private readonly IServiceProvider _serviceProvider;

    public GetPlaceById(IRabbitMqService service, IServiceProvider serviceProvider)
    {
        _service = service;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            Dispose();

        _service.AddConsumerEventHandler("get_place_by_id_queue", async (_, args) =>
        {
            string? response = null;

            if (args.Body.Length != 0)
            {
                var id = Guid.Parse(Encoding.UTF8.GetString(args.Body.ToArray()));

                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<LocationDbContext>();

                var place = await _context.Places!.FirstOrDefaultAsync(p => p.Id == id);

                if (place != null)
                {
                    var setting = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };
                    response = JsonConvert.SerializeObject(place, setting);
                }
            }

            _service.RpcProduce(response, args, "get_place_by_id_reply");
        });

        _service.Consume("get_place_by_id_queue");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _service.Dispose();
        GC.SuppressFinalize(this);
    }
}