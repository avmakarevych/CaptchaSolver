namespace CaptchaSolver.Utilities;

public class Logger
{
    public void PrintSuccess(string message)
    {
        Print(message, ConsoleColor.Green, "SUCCESS");
    }

    public void PrintError(string message)
    {
        Print(message, ConsoleColor.Red, "ERROR");
    }

    public void PrintInfo(string message)
    {
        Console.WriteLine($"{DateTime.Now} | INFO | {message}");
    }

    public void PrintException(Exception e)
    {
        Console.WriteLine($"{DateTime.Now} | EXCEPTION | {e}");
    }

    private void Print(string message, ConsoleColor color, string status)
    {
        Console.Write($"{DateTime.Now} | {message} - ");
        Console.ForegroundColor = color;
        Console.WriteLine(status);
        Console.ResetColor();
    }
}