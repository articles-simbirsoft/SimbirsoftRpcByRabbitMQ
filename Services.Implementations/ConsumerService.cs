namespace Services.Implementations;
public class ConsumerService
{
    private static int _randomNumber;
    /// <summary>
    /// отгадано ли число
    /// </summary>
    public static bool IsSuccess { get; set; }
    public static int RndNumber
    {
        get { return _randomNumber; }
    }
    public static void InitNumber(int start,int end)
    {
        Random rnd = new Random();
        _randomNumber = rnd.Next(start, end);
    }
}

