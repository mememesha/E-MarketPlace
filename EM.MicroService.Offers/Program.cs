using EM.Contracts;
using EM.MicroService.Offers.Data;
using EM.MicroService.Offers.Services;
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

// builder.Services.AddControllers()
//                 .AddNewtonsoftJson(x =>
//                     x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

builder.Services.AddDbContext<OffersDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Offers")));

builder.Services.AddHostedService<GetNewOffers>();
builder.Services.AddHostedService<GetAllCategories>();
builder.Services.AddHostedService<GetOfferById>();
builder.Services.AddHostedService<UpdateOffer>();
builder.Services.AddHostedService<GetOffersByOrganizationId>();
builder.Services.AddHostedService<UpdateOfferDescription>();
builder.Services.AddHostedService<AddReserveToOffer>();

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
    var context = scope.ServiceProvider.GetRequiredService<OffersDbContext>();

    context!.Database.EnsureDeleted();
    context!.Database.EnsureCreated();

    var offersResponse = await rabbitMq!.RpcCallAsync("get_fake_data", "Offers");
    var offers = JsonConvert.DeserializeObject<List<Offer>>(offersResponse);

    var offerDescriptionsResponse = await rabbitMq!.RpcCallAsync("get_fake_data", "OfferDescriptions");
    var offerDescriptions = JsonConvert.DeserializeObject<List<OfferDescription>>(offerDescriptionsResponse);

    var categoriesResponse = await rabbitMq!.RpcCallAsync("get_fake_data", "Categories");
    var categories = JsonConvert.DeserializeObject<List<Category>>(categoriesResponse);

    context.Offers!.AddRange(offers!);
    context.OfferDescriptions!.AddRange(offerDescriptions!);
    context.Categories!.AddRange(categories!);
    context.SaveChanges();

}
