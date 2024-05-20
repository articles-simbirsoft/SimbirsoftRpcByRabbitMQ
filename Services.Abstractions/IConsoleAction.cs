namespace Services.Abstractions;
public interface IConsoleAction
{
    void PrintMessage(string message);
    string GetConsoleInput();
    void PressKey();
}

