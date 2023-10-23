using EM.Contracts;
using EM.MicroService.Location.Data;
using EM.MicroService.Location.Services;
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

// Singleton - так как AddHostedService создает Singleton
builder.Services.AddDbContext<LocationDbContext>(
    context => { context.UseNpgsql(builder.Configuration.GetConnectionString("Location")); });

// RabbitMq
builder.Services.Configure<IRabbitMqOptions>(builder.Configuration.GetSection("RabbitMqOptions"));
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();

// Listeners
builder.Services.AddHostedService<GetCurrentCity>();
builder.Services.AddHostedService<GetAllCities>();
builder.Services.AddHostedService<GetPlaceById>();
builder.Services.AddHostedService<UpdatePlace>();

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

#if DEBUG
SeedDatabase();
#endif

app.Run();

async void SeedDatabase()
{
    var rabbitMq = app.Services.GetService<IRabbitMqService>();
    var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<LocationDbContext>();

    context!.Database.EnsureDeleted();
    context!.Database.EnsureCreated();

    var placesResponse = await rabbitMq!.RpcCallAsync("get_fake_data", "Places");
    var places = JsonConvert.DeserializeObject<List<Place>>(placesResponse);

    context.Places!.AddRange(places!);
    context.SaveChanges();

}