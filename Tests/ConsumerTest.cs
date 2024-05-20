using MassTransit;
using Services.Contracts;
using Services.Implementations;
using Tests.Resources;

namespace Tests;

public sealed class ConsumerTest: IConsumer<MessageDto>
{
    public async Task Consume(ConsumeContext<MessageDto> context)
    {
        ConsumerService.IsSuccess = context.Message.IsSuccess;
        ConsumeFlag.Number = context.Message.Number;
        ConsumeFlag.IsFlag = false;
    }
}
