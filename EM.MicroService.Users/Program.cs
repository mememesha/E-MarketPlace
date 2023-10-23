using EM.Contracts;
using EM.MicroService.Users.Data;
using EM.MicroService.Users.Services;
using EM.Shared.Connections.Broker.RabbitMQ;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// builder.Services.AddControllers();
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// RabbitMq
builder.Services.Configure<IRabbitMqOptions>(builder.Configuration.GetSection("RabbitMqOptions"));
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();

builder.Services.AddDbContext<UsersDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Users")));

builder.Services.AddHostedService<GetUserById>();

var app = builder.Build();

// // Configure the HTTP request pipeline.
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
    var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

    context!.Database.EnsureDeleted();
    context!.Database.EnsureCreated();

    var usersResponse = await rabbitMq!.RpcCallAsync("get_fake_data", "Users");
    var users = JsonConvert.DeserializeObject<List<User>>(usersResponse);

    var usersWithRoleResponse = await rabbitMq!.RpcCallAsync("get_fake_data", "UsersWithRole");
    var usersWithRole = JsonConvert.DeserializeObject<List<UserWithRole>>(usersWithRoleResponse);

    context.Users!.AddRange(users!);
    context.UsersWithRole!.AddRange(usersWithRole!);
    context.SaveChanges();
}