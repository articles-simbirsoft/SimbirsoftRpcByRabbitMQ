using Services.Extensions;
using WebApi.Saga.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<RabbitConfig>(builder.Configuration.GetSection("RabbitMqSettings"));
builder.Services.AddSwaggerGen();
builder.Services.AddBLLServices();

var app = builder.Build();
app.UseHttpsRedirection();
app.Run();

