using Infrastructure.ConsoleAction;
using Infrastructure.MassTransit;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Services.Abstractions;
using Services.Contracts;
using Services.Extensions;
using Services.Implementations;
namespace ConsoleApp.Producer.Extensions;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Метод-расширение для регистрации сервисов бизнес-логики по работе с данными
    /// </summary>
    public static IServiceCollection AddBLLServices(this IServiceCollection services)
    {
        services.AddScoped<IMassTransitHelper, MassTransitHelper>();
        services.AddScoped<IConsoleAction, ConsoleAction>();
        services.AddScoped<IProducerSettingService, ProducerSettingService>();
        return services;
    }
    /// <summary>
    /// Метод-расширение для работы с сагой Masstransit
    /// </summary>
    public static IServiceCollection AddSagaServices(this IServiceCollection services)
    {
        services.AddMassTransit(cfg =>
        {
            cfg.SetKebabCaseEndpointNameFormatter();
            cfg.AddRequestClient<MessageDto>();
            cfg.UsingRabbitMq((x, y) =>
            {
                RabbitMQSettings.Configure(y, x.GetRequiredService<IOptions<RabbitConfig>>().Value);
                y.ConfigureEndpoints(x);
            });
        });
        return services;
    }
}
