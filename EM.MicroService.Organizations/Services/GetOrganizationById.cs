using System.Text;
using EM.MicroService.Organizations.Data;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EM.MicroService.Organizations.Services;

public class GetOrganizationById : BackgroundService
{
    private readonly IRabbitMqService _service;
    private readonly IServiceProvider _serviceProvider;

    public GetOrganizationById(IRabbitMqService service, IServiceProvider serviceProvider)
    {
        _service = service;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            Dispose();

        _service.AddConsumerEventHandler("get_organization_by_id_queue", async (_, args) =>
        {
            string? response = null;

            if (args.Body.Length != 0)
            {
                var id = Guid.Parse(Encoding.UTF8.GetString(args.Body.ToArray()));

                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<OrganizationsDbContext>();

                var organization = await _context.Organizations!.FirstOrDefaultAsync(o => o.Id == id);
                if (organization != null)
                {
                    var setting = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };
                    response = JsonConvert.SerializeObject(organization, setting);
                }
            }

            _service.RpcProduce(response, args, "get_organization_by_id_reply");
        });

        _service.Consume("get_organization_by_id_queue");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _service.Dispose();
        GC.SuppressFinalize(this);
    }
}