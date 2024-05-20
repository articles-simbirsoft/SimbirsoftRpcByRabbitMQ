using MassTransit;
using Microsoft.Extensions.Logging;
using Services.Contracts;
namespace Infrastructure.MassTransit.WithSaga;

public sealed class Saga : MassTransitStateMachine<SagaState>
{
    private readonly ILogger<Saga> _logger;

    public Saga(ILogger<Saga> logger)
    {
        _logger = logger;
        InstanceState(x => x.CurrentState);
        Event(() => GetNumberEvent, x => x.CorrelateById(y => y.Message.Id));
        //инициализация запроса из саги
        Request(() => GetNumber);
        Initially(
            When(GetNumberEvent)
            .Then(x =>
            {
                if (!x.TryGetPayload(out SagaConsumeContext<SagaState, MessageDto> payload))
                    throw new Exception("Нет данных для ответа");
                x.Saga.RequestId = payload.RequestId;
                x.Saga.ResponseAddress = payload.ResponseAddress;
            })
           .TransitionTo(GetNumber.Pending));
        During(GetNumber.Pending,
            When(GetNumber.Completed)
               .TransitionTo(Finished));
    }
    //отправка числа
    public Request<SagaState, MessageDto, MessageDtoResponce> GetNumber { get; set; }
    public Event<MessageDto> GetNumberEvent { get; set; }
    //состояния саги
    public State Failed { get; set; }
    public State Received { get; private set; }
    public State Started { get; private set; }
    public State Cancelling { get; private set; }
    public State Finished { get; private set; }
}
