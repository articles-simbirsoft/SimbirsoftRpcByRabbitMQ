using Infrastructure.MassTransit;
using MassTransit.Testing;
using MassTransit;
using Newtonsoft.Json.Linq;
using Services.Contracts;
using Xunit;
using Services.Extensions.Exceptions;
using Microsoft.Extensions.Options;
using Tests.Resources;
using Services.Extensions;

namespace Tests;

public class RabbitMQ_Test
{
    private IOptions<RabbitConfig> _options;

    public RabbitMQ_Test()
    {
        _options = Options.Create(GetConfig());
    }
    [Fact]
    public async Task Consumer_test_without_saga()
    {
        // Arrange
        //MasstransitSettingsInitialize();
        MessageDto dto = new MessageDto(12, 0, 100, 4, 5, "тест", false, Guid.NewGuid());

        // Act
        ConsumeFlag.IsFlag = true;
        await SendMessageAsync(dto, _options?.Value.ProducerQueueName);
        await ReceiveMessageAsync();

        // Assert
        Assert.Equal(ConsumeFlag.Number, dto.Number);
    }

    private async Task ReceiveMessageAsync()
    {
        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            RabbitMQSettings.Configure(cfg, _options);
            RegisterEndPoints(cfg, _options?.Value.ProducerQueueName);
        });
        await busControl.StartAsync();
        try
        {
            while (ConsumeFlag.IsFlag) ;
        }
        finally
        {
            await busControl.StopAsync();
        }
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
            e.Consumer(() => new ConsumerTest());
            e.UseMessageRetry(r =>
            {
                r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            });
        });
    }

    private async Task SendMessageAsync(MessageDto dto, string queueName)
    {
        ConsumeFlag.Number = dto.Number;
        var busControl = Bus.Factory.CreateUsingRabbitMq(x=>RabbitMQSettings.Configure(x, _options));
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

    private RabbitConfig GetConfig()
    {
        RabbitConfig config = new RabbitConfig();
        var bytes = System.Text.Encoding.UTF8.GetBytes(Properties.Resources.json_txt);
        string text = System.Text.Encoding.UTF8.GetString(bytes);
        var json = JToken.Parse(text);
        config.Host = json["MassTransitHost"].ToString();
        config.VirtualHost= json["MassTransitVirtualHost"].ToString();
        config.UserName= json["MassTransitUserName"].ToString();
        config.Password = json["MassTransitPswrd"].ToString();
        config.ConsumerQueueName = json["MassTransitConsumerQueueName"].ToString();
        config.ProducerQueueName= json["MassTransitProducerQueueName"].ToString();
        return config;
    }
}
