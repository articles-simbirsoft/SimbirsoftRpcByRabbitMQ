using Services.Abstractions;
namespace Infrastructure.ConsoleAction;

public sealed class ConsoleAction: IConsoleAction
{
    public void PrintMessage(string message)=>Console.WriteLine(message);
    public void PressKey()=>Console.ReadLine();
    public string GetConsoleInput()
    {
        string? input = Console.ReadLine();
        return input;
    }
}

