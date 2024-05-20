using Infrastructure.MassTransit;
using MassTransit;
using Microsoft.Extensions.Options;
using Services.Extensions;
namespace WebApi.Consumer.Extensions;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Методы-расширения для регистрации сервисов бизнес-логики по работе с данными без саги
    /// </summary>
    public static IServiceCollection AddBLLServicesWithoutSaga(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
         {
             x.AddConsumer<RabbitMqConsumerWebApi>();
             x.AddBus(provider =>
             Bus.Factory.CreateUsingRabbitMq(cfg =>
             {
                 var setting = provider.GetRequiredService<IOptions<RabbitConfig>>().Value;
                 RabbitMQSettings.Configure(cfg, setting);
                 RabbitMQSettings.SetRecieveSettings(cfg, provider, setting);
             }));
         });
        return services;
    }
    /// <summary>
    /// Методы-расширения для регистрации сервисов бизнес-логики по работе с сагой
    /// </summary>
    public static IServiceCollection AddBLLServicesWithSaga(this IServiceCollection services)
    {
        services.AddMassTransit(cfg =>
        {
            cfg.SetKebabCaseEndpointNameFormatter();
            cfg.AddDelayedMessageScheduler();
            cfg.AddConsumer<RabbitMqConsumerWebApi>();
            cfg.UsingRabbitMq((brc, rbfc) =>
            {
                rbfc.UseInMemoryOutbox();
                rbfc.UseMessageRetry(r =>
                {
                    r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                });
                rbfc.UseDelayedMessageScheduler();
                RabbitMQSettings.Configure(rbfc, brc.GetRequiredService<IOptions<RabbitConfig>>().Value);
                rbfc.ConfigureEndpoints(brc);
            });
        });
        return services;
    }
}
