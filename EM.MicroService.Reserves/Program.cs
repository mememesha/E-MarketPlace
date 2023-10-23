using EM.Contracts;
using EM.MicroService.Offers.Services;
using EM.MicroService.Reserves.Data;
using EM.Shared.Connections.Broker.RabbitMQ;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddControllers();
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
// RabbitMq
builder.Services.Configure<IRabbitMqOptions>(builder.Configuration.GetSection("RabbitMqOptions"));
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();

builder.Services.AddDbContext<ReservesDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Reserves")));

builder.Services.AddHostedService<AddReserve>();
builder.Services.AddHostedService<GetReservesByOfferId>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
// app.UseAuthorization();
// app.MapControllers();

app.UseHttpsRedirection();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

#if DEBUG
SeedDatabase();
#endif

app.Run();

async void SeedDatabase()
{
    var rabbitMq = app.Services.GetService<IRabbitMqService>();
    var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ReservesDbContext>();

    context!.Database.EnsureDeleted();
    context!.Database.EnsureCreated();

    //     var reservesResponse = await rabbitMq!.RpcCallAsync("get_fake_data", "Reserves");
    //     var reserves = JsonConvert.DeserializeObject<List<Reserve>>(reservesResponse);

    //     context.Reserves!.AddRange(reserves!);
    //     context.SaveChanges();
}