using Microsoft.Extensions.Configuration;
namespace Infrastructure.MassTransit;

public static class RabbitMqConstants
{
    public static string Host;
    public static string VirtualHost;
    public static string UserName;
    public static string Password;
    public static string ConsumerQueueName;
    public static string ProducerQueueName;

    public static void InitializeMasstransitSettings(IConfigurationRoot? config)
    {
        Host = config["RabbitMqSettings:Host"];
        VirtualHost = config["RabbitMqSettings:VirtualHost"];
        UserName = config["RabbitMqSettings:UserName"];
        Password = config["RabbitMqSettings:Password"];
        ConsumerQueueName = config["RabbitMqSettings:ConsumerQueueName"];
        ProducerQueueName = config["RabbitMqSettings:ProducerQueueName"];
    }
}

