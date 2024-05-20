using Services.Contracts;
namespace Services.Abstractions;
public interface IMassTransitHelper : IBaseMessageBrokerService
{
    /// <summary>
    /// отправка сообщений в очередь через сагу
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<string> SendMessageWithSagaAsync(MessageDto dto);
}
