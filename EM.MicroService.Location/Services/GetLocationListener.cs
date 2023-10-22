using System.Text;
using EM.MicroService.Location.Data;
using EM.MicroService.Location.Options;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using EM.Shared.Models;
using Microsoft.Extensions.Options;

namespace EM.MicroService.Location.Services;

public class GetLocationListener : BackgroundService
{
    private readonly IRabbitMqService _service;
    private readonly LocationDbContext _locationDbContext;
    private readonly IOptions<ExternalIpServiceOptions> _options;

    public GetLocationListener(
        IRabbitMqService service, 
        LocationDbContext locationDbContext, 
        IOptions<ExternalIpServiceOptions> options)
    {
        _service = service;
        _locationDbContext = locationDbContext;
        _options = options;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            Dispose();

        _service.AddConsumerEventHandler("get_location_queue", async (_, args) =>
        {
            string response = "Россия";

            if (args.Body.Length != 0)
            {
                var ipAddress = Encoding.UTF8.GetString(args.Body.ToArray());

                // if localhost
                if (ipAddress == "::1") 
                    ipAddress = await GetIpFromExternalService();
                
                if(ipAddress != null)
                {
                    var addressInt = IpConverter.FromIpAddressToInteger(ipAddress);

                    var location = _locationDbContext.Ip_City!.FirstOrDefault(ip => ip.Ip_from < addressInt && ip.Ip_to > addressInt);

                    response = location?.City ?? response;
                }
            }
            _service.RpcProduce(response!, args, "get_location_reply");
        });

        _service.Consume("get_location_queue");

        return Task.CompletedTask;
    }

    private async Task<string> GetIpFromExternalService()
    {
        string? result = null;
        var client = new HttpClient();
        
        foreach(var externalService in _options.Value.ExternalServices!)
        {
            var response = await client.GetStringAsync(externalService);

            if(response != null)
            {
                result = response;
                break;
            }
        }

        return result!;
    }

    public override void Dispose()
    {
        _service.Dispose();
        GC.SuppressFinalize(this);
    }
}