using EM.MicroService.Location.Data;
using EM.MicroService.Location.Options;
using EM.MicroService.Location.Services;
using EM.Shared.Connections.Broker.RabbitMQ;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddControllers();
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// Singleton - так как AddHostedService создает Singleton
builder.Services.AddDbContext<LocationDbContext>(
    context => { context.UseNpgsql(builder.Configuration.GetConnectionString("Location")); }, 
    ServiceLifetime.Singleton);

builder.Services.Configure<ExternalIpServiceOptions>(builder.Configuration.GetSection("ExternalIpServiceOptions"));

// RabbitMq
builder.Services.Configure<IRabbitMqOptions>(builder.Configuration.GetSection("RabbitMqOptions"));
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();

// Listeners
builder.Services.AddHostedService<GetLocationListener>();

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
app.Run();
