using Infrastructure.MassTransit;
using MassTransit;
using Microsoft.Extensions.Options;
using Services.Extensions;
namespace WebApi.Saga.Extensions;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Метод-расширение для регистрации сервисов бизнес-логики по работе с данными
    /// </summary>
    public static IServiceCollection AddBLLServices(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<RabbitMqConsumerWebApi>();
            x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var setting = provider.GetRequiredService<IOptions<RabbitConfig>>().Value;
                RabbitMQSettings.Configure(cfg, setting);
                RabbitMQSettings.SetRecieveSettings(cfg, provider, setting);
            }));
        });
        return services;
    }
}