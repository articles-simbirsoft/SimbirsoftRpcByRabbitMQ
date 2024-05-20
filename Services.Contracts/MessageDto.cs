namespace Services.Contracts;
public class MessageDto
{
    public MessageDto(int number, int start, int end, int attemptionNumber, int attemptionCount,
                      string content, bool isSuccess, Guid id)
    {
        Number = number;
        Start = start;
        End = end;
        AttemptionNumber = attemptionNumber;
        AttemptionCount = attemptionCount;
        Content = content;
        IsSuccess = isSuccess;
        Id = id;
    }
    public Guid Id { get; set; }
    /// <summary>
    /// число , отправляемое пользователем
    /// </summary>
    public int Number { get; init; }
    /// <summary>
    /// начало диапазона чисел для отгадывания
    /// </summary>
    public int Start { get; set; }
    /// <summary>
    /// конец диапазона чисел для отгадывания
    /// </summary>
    public int End { get; set; }
    /// <summary>
    /// номер попытки
    /// </summary>
    public int AttemptionNumber { get; set; }
    /// <summary>
    /// количество попыток
    /// </summary>
    public  int AttemptionCount { get; set; }
    /// <summary>
    /// ответ сервера (угадал или нет)
    /// </summary>
    public string Content { get; set; }
    /// <summary>
    /// отгадано ли число
    /// </summary>
    public bool IsSuccess { get; set; }
}

