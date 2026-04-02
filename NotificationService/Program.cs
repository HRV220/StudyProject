using NotificationService;
using StackExchange.Redis;
using Scalar.AspNetCore;

//await Task.Delay(60000);
var builder = WebApplication.CreateBuilder(args);

var destroy_flag = false;

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var redisConnection = builder.Configuration["REDIS_CONNECTION"] ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnection));

builder.Services.AddHostedService<NotificationSubscriber>();

var app = builder.Build();

app.MapPost("/switchFlag", async destroy =>
{
    destroy_flag = !destroy_flag;
    await destroy.Response.WriteAsync($"Flag:{destroy_flag}");

});

app.MapGet("/health", async health =>
{
    if(!destroy_flag)
    {
        health.Response.StatusCode = 200;
        await health.Response.WriteAsync("Ok");
    }
    else
    {
        health.Response.StatusCode = 503;
        await health.Response.WriteAsync("Destroy");
    }
});

// Configure the HTTP request pipeline.
app.MapOpenApi();
app.MapScalarApiReference();

app.UseAuthorization();

app.MapControllers();

app.Run();
