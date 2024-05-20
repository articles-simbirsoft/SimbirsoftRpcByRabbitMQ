namespace Services.Extensions;

public class RabbitConfig
{
    public string Host { get; set; }
    public string VirtualHost { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ConsumerQueueName { get; set; }
    public string ProducerQueueName { get; set; }
}
