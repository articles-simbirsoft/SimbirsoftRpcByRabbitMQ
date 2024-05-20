namespace Services.Contracts;
public class SettingDto
{
    public SettingDto(int start, int end, int attemptsCount)
    {
        Start = start;
        End = end;
        AttemptsCount = attemptsCount;
    }
    /// <summary>
    /// начало диапазона отгадывания 
    /// </summary>
    public int Start { get; set; }
    /// <summary>
    /// конец диапазона отгадывания 
    /// </summary>
    public int End { get; set; }
    /// <summary>
    /// количество попыток 
    /// </summary>
    public int AttemptsCount { get; set; }
}

