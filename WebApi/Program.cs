using Services.Extensions;
using WebApi.Consumer.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<RabbitConfig>(builder.Configuration.GetSection("RabbitMqSettings"));
// without Saga
builder.Services.AddBLLServicesWithoutSaga();
// with Saga By Masstransit
//builder.Services.AddBLLServicesWithSaga();
var app = builder.Build();
app.UseHttpsRedirection();
app.Run();
