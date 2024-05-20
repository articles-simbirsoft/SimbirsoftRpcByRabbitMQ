using ConsoleApp.Producer.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProducerApp;
using Services.Extensions;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<StartApp>();
builder.Services.Configure<RabbitConfig>(builder.Configuration.GetSection("RabbitMqSettings"));
builder.Services.AddBLLServices();
// with Saga By Masstransit
//builder.Services.AddSagaServices();
using IHost host = builder.Build();
await host.RunAsync();
