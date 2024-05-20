using Microsoft.Extensions.Options;
using Services.Contracts;
using Services.Extensions;
namespace Services.Abstractions;
public interface IBaseMessageBrokerService
{
    /// <summary>
    /// отправка сообщений в очередь
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="queueName"></param>
    /// <param name="rabbitConfig"></param>
    /// <returns></returns>
    Task SendMessageAsync(MessageDto dto, string queueName, RabbitConfig rabbitConfig);

    /// <summary>
    /// слушаем сообщения из очереди
    /// </summary>
    /// <param name="rabbitConfig"></param>
    /// <returns></returns>
    Task ReceiveMessageAsync(RabbitConfig rabbitConfig);
}

