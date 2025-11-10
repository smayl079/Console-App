using System.Text;

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

    public static string? ReadInput(string prompt, bool allowEscape = true)
    {
        Console.Write($"{prompt} (Press ESC to cancel): ");
        
        var input = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(true);
            
            if (key.Key == ConsoleKey.Escape && allowEscape)
            {
                Console.WriteLine();
                return null; // ESC pressed - return null to indicate cancellation
            }
            
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return input.ToString();
            }
            
            if (key.Key == ConsoleKey.Backspace)
            {
                if (input.Length > 0)
                {
                    input.Remove(input.Length - 1, 1);
                    Console.Write("\b \b");
                }
            }
            else if (!char.IsControl(key.KeyChar))
            {
                input.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }
    }

    public static int? ReadIntInput(string prompt)
    {
        while (true)
        {
            var input = ReadInput(prompt);
            if (input == null)
                return null; // ESC pressed - return null to indicate cancellation
            
            if (int.TryParse(input, out int result))
                return result;
            DisplayError("Invalid input. Please enter a valid number.");
        }
    }
}

