using MassTransit;
using Microsoft.Extensions.Options;
using Services.Abstractions;
using Services.Contracts;
using Services.Extensions;
using Services.Extensions.Exceptions;
namespace Infrastructure.MassTransit;

public class MassTransitHelper : IMassTransitHelper
{
    private static IConsoleAction _actionPrint;
    //readonly IRequestClient<MessageDto> _submitClient;
    public MassTransitHelper(IConsoleAction actionPrint/*, IRequestClient<MessageDto> submitClient*/)
    {
        _actionPrint = actionPrint ?? throw new ArgumentNullException(nameof(actionPrint));
        //_submitClient = submitClient ?? throw new ArgumentNullException(nameof(submitClient));
    }
    /// <summary>
    /// слушаем сообщения из очереди
    /// </summary>
    /// <param name="rabbitConfig"></param>
    /// <returns></returns>
    public async Task ReceiveMessageAsync(RabbitConfig rabbitConfig)
    {
        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            RabbitMQSettings.Configure(cfg, rabbitConfig);
            RegisterEndPoints(cfg, rabbitConfig?.ConsumerQueueName);
        });
        await busControl.StartAsync();
        SpinWait sw = new SpinWait();
        try
        {
            while (true) sw.SpinOnce();
        }
        finally
        {
            await busControl.StopAsync();
        }
    }
    /// <summary>
    /// отправка сообщений в очередь
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="queueName"></param>
    /// <param name="rabbitConfig"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task SendMessageAsync(MessageDto dto, string queueName, RabbitConfig rabbitConfig)
    {
        var busControl = Bus.Factory.CreateUsingRabbitMq(x => RabbitMQSettings.Configure(x, rabbitConfig));
        await busControl.StartAsync();
        try
        {
            var sendEndpoint = await busControl.GetSendEndpoint(new Uri($"queue:{queueName}"));
            if (sendEndpoint is null)
                throw new Exception(string.Concat(ExceptionConstants.QueueExceptionMessage, queueName));

            await sendEndpoint.Send(dto, CancellationToken.None);
        }
        finally
        {
            await busControl.StopAsync();
        }
    }
    /// <summary>
    /// отправка сообщений в очередь через сагу и вывод значения клиенту
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<string> SendMessageWithSagaAsync(MessageDto dto)
    {
        //var response = await _submitClient.GetResponse<MessageDto>(dto);
        //if (!string.IsNullOrEmpty(response?.Message?.Content))
        //    return response.Message.Content;
        return ExceptionConstants.ResponceExceptionMessage;
    }
    /// <summary>
    /// регистрация эндпоинтов
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="queueName"></param>
    private static void RegisterEndPoints(IRabbitMqBusFactoryConfigurator configurator, string queueName)
    {
        configurator.ReceiveEndpoint(queueName, e =>
        {
            e.Consumer(() => new RabbitMqConsumer(_actionPrint));
            e.UseMessageRetry(r =>
            {
                //r.Ignore<ArithmeticException>(); игнориование исключения
                r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)); ///повторные отправки , при возникновении исключения
            });
        });
    }
}
