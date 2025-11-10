    namespace Service.Helpers;

public static class Helpers
{
    public static void DisplayMessage(string message, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void DisplayError(string message)
    {
        DisplayMessage($"Error: {message}", ConsoleColor.Red);
    }

    public static void DisplaySuccess(string message)
    {
        DisplayMessage($"Success: {message}", ConsoleColor.Green);
    }

    public static void DisplayInfo(string message)
    {
        DisplayMessage($"Info: {message}", ConsoleColor.Cyan);
    }

    public static string ReadInput(string prompt)
    {
        Console.Write($"{prompt}: ");
        return Console.ReadLine() ?? string.Empty;
    }

    public static int ReadIntInput(string prompt)
    {
        while (true)
        {
            var input = ReadInput(prompt);
            if (int.TryParse(input, out int result))
                return result;
            DisplayError("Invalid input. Please enter a valid number.");
        }
    }
}

