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
        PlayErrorSound();
        DisplayMessage($"Error: {message}", ConsoleColor.Red);
    }

    public static void DisplaySuccess(string message)
    {
        PlaySuccessSound();
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
            // Look for sound file in the output directory
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            if (!File.Exists(path))
            {
                // Try in Sounds folder as fallback
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", fileName);
                if (!File.Exists(path))
                {
                    // If sound not found, fallback beep
                    Console.Beep(700, 150);
                    return;
                }
            }

            using (SoundPlayer player = new SoundPlayer(path))
            {
                if (async)
                    player.Play();      // Play in background
                else
                    player.PlaySync();  // Wait for sound to finish
            }
        }
        catch
        {
            // Fallback if sound playback fails
            Console.Beep(500, 200);
        }
    }

    public static void PlaySuccessSound(bool async = true)
    {
        PlaySound("Succes.wav", async);
    }

    public static void PlayErrorSound(bool async = true)
    {
        PlaySound("Error.wav", async);
    }

    public static void PlayMenuSound(bool async = true)
    {
        // Play success sound when menu opens (or you can create a separate menu sound file)
        PlaySuccessSound(async);
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

