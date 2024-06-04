using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Repositories;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using SqlServer Db.");
    builder
        .Services
        .AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
else
{
    Console.WriteLine("--> Using InMemory Db.");
    builder
        .Services
        .AddDbContext<AppDbContext>(opt =>
            opt.UseInMemoryDatabase("InMemory"));
}


builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


Console.WriteLine($"--> Command Service url: {builder.Configuration["CommandServiceUrl"]}");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/protos/platforms.proto", async context =>
{
    await context.Response.WriteAsync(File.ReadAllText("Protos/Platforms.proto"));
});


PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();