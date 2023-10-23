using System.Text;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Newtonsoft.Json;

namespace EM.FakeData;

public class GetFakeData : BackgroundService
{
    private readonly IRabbitMqService _service;

    public GetFakeData(IRabbitMqService service)
    {
        _service = service;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        FakeDataFactory.Init();

        if (stoppingToken.IsCancellationRequested)
            Dispose();

        _service.AddConsumerEventHandler("get_fake_data_queue", (_, args) =>
        {
            if (args.Body.Length != 0)
            {
                var setting = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                switch (Encoding.UTF8.GetString(args.Body.ToArray()))
                {
                    case "Offers":
                        _service.RpcProduce(JsonConvert.SerializeObject(FakeDataFactory.Offers, setting), args, "get_fake_data_reply");
                        break;
                    case "OfferDescriptions":
                        _service.RpcProduce(JsonConvert.SerializeObject(FakeDataFactory.OfferDescriptions, setting), args, "get_fake_data_reply");
                        break;
                    case "Categories":
                        _service.RpcProduce(JsonConvert.SerializeObject(FakeDataFactory.Categories, setting), args, "get_fake_data_reply");
                        break;
                    case "Places":
                        _service.RpcProduce(JsonConvert.SerializeObject(FakeDataFactory.Places, setting), args, "get_fake_data_reply");
                        break;
                    case "Organizations":
                        _service.RpcProduce(JsonConvert.SerializeObject(FakeDataFactory.Organizations, setting), args, "get_fake_data_reply");
                        break;
                    case "Users":
                        _service.RpcProduce(JsonConvert.SerializeObject(FakeDataFactory.Users, setting), args, "get_fake_data_reply");
                        break;
                    case "UsersWithRole":
                        _service.RpcProduce(JsonConvert.SerializeObject(FakeDataFactory.UsersWithRole, setting), args, "get_fake_data_reply");
                        break;
                        // case "Organizations":
                        // _service.RpcProduce(JsonConvert.SerializeObject(FakeDataFactory.Organizations, setting), args, "get_fake_data_reply");
                        // break;
                        // default:
                        //     _service.RpcProduce(null, args, "get_fake_data_reply");
                        //     break;
                }
            }
        });

        _service.Consume("get_fake_data_queue");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _service.Dispose();
        GC.SuppressFinalize(this);
    }
}