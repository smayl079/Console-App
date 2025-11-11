using System.Media;
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
                return null; 
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
    public static void PlaySound(string fileName, bool async = true)
    {
        try
        {
            // Səs faylının yolu
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", fileName);

            if (!File.Exists(path))
            {
                // Əgər səs tapılmasa, fallback beep
                Console.Beep(700, 150);
                return;
            }

            using (SoundPlayer player = new SoundPlayer(path))
            {
                if (async)
                    player.Play();      // arxa planda oxuyur
                else
                    player.PlaySync();  // proqramı dayandırır, səs bitəndə davam edir
            }
        }
        catch
        {
            // fallback (səs oynatmaq alınmasa)
            Console.Beep(500, 200);
        }
    }
    public static int? ReadIntInput(string prompt)
    {
        while (true)
        {
            var input = ReadInput(prompt);
            if (input == null)
                return null; 
            
            if (int.TryParse(input, out int result))
                return result;
            DisplayError("Invalid input. Please enter a valid number.");
        }
    }
}

