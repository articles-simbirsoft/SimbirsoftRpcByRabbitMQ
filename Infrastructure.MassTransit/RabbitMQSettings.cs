using MassTransit;
using Services.Extensions;
namespace Infrastructure.MassTransit;

public sealed class RabbitMQSettings
{
    /// <summary>
    /// подключение к облаку RabbitMQ
    /// </summary>
    /// <param name="cfg"></param>
    /// <param name="_rabbitConfig"></param>
    public static void Configure(IRabbitMqBusFactoryConfigurator cfg, RabbitConfig _rabbitConfig)
    {
        cfg.Host(_rabbitConfig?.Host,
                 _rabbitConfig?.VirtualHost,
                 h =>
                   {
                       h.Username(_rabbitConfig?.UserName);
                       h.Password(_rabbitConfig?.Password);
                   });

    }
    /// <summary>
    /// Настройка класса-прослушивания входящх сообщений
    /// </summary>
    /// <param name="cfg"></param>
    /// <param name="ctx"></param>
    /// <param name="rabbitConfig"></param>
    public static void SetRecieveSettings(IRabbitMqBusFactoryConfigurator cfg, IBusRegistrationContext ctx, RabbitConfig rabbitConfig)
    {
        cfg.ReceiveEndpoint(rabbitConfig?.ProducerQueueName, ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(r => r.Interval(2, 100));
            ep.ConfigureConsumer<RabbitMqConsumerWebApi>(ctx);
        });
    }
}
