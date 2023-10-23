using EM.MicroService.Offers.Data;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EM.MicroService.Offers.Services;

public class GetAllCategories : BackgroundService
{
    private readonly IRabbitMqService _service;
    private readonly IServiceProvider _serviceProvider;

    public GetAllCategories(IRabbitMqService service, IServiceProvider serviceProvider)
    {
        _service = service;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            Dispose();

        _service.AddConsumerEventHandler("get_all_categories_queue", async (_, args) =>
        {
            using var scope = _serviceProvider.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<OffersDbContext>();

            var categories = await _context.Categories!.AsNoTracking()
                                .IgnoreAutoIncludes()
                                .ToListAsync();

            var setting = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            _service.RpcProduce(JsonConvert.SerializeObject(categories, setting), args, "get_all_categories_reply");
        });

        _service.Consume("get_all_categories_queue");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _service.Dispose();
        GC.SuppressFinalize(this);
    }
}