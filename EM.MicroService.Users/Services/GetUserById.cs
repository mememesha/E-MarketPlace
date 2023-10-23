using System.Text;
using EM.MicroService.Users.Data;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EM.MicroService.Users.Services;

public class GetUserById : BackgroundService
{
    private readonly IRabbitMqService _service;
    private readonly IServiceProvider _serviceProvider;

    public GetUserById(IRabbitMqService service, IServiceProvider serviceProvider)
    {
        _service = service;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            Dispose();

        _service.AddConsumerEventHandler("get_user_by_id_queue", async (_, args) =>
        {
            string? response = null;

            if (args.Body.Length != 0)
            {
                var id = Guid.Parse(Encoding.UTF8.GetString(args.Body.ToArray()));

                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

                var user = await _context.Users!.Include(u => u.UsersWithRole).FirstOrDefaultAsync(o => o.Id == id);
                if (user != null)
                {
                    var setting = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };
                    response = JsonConvert.SerializeObject(user, setting);
                }
            }

            _service.RpcProduce(response, args, "get_user_by_id_reply");
        });

        _service.Consume("get_user_by_id_queue");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _service.Dispose();
        GC.SuppressFinalize(this);
    }
}