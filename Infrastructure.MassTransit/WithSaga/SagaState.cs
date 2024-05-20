using MassTransit;
namespace Infrastructure.MassTransit.WithSaga;

public sealed class SagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    /// <summary>
    /// Текущее состояние саги.
    /// </summary>
    public string? CurrentState { get; set; }
    /// <summary>
    /// Id запроса,который стартанул нашу сагу
    /// </summary>
    public Guid? RequestId { get; set; }
    /// <summary>
    /// адрес для ответа на запрос
    /// </summary>
    public Uri? ResponseAddress { get; set; }
}
