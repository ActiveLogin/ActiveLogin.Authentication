using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phone.ConsoleSample;
public static class ConsoleHelper
{
    public static void WriteHeader(string text)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(text);
        Console.ResetColor();
    }
    
    public static string DisplayMenuAndGetSelectedKey(List<(string Key, string DisplayName)> menuItems)
    {
        int currentIndex = 0;
        ConsoleKeyInfo keyInfo;
        int menuStartRow = Console.CursorTop;

        do
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                Console.SetCursorPosition(0, menuStartRow + i);

                if (i == currentIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine("* " + menuItems[i].DisplayName.PadRight(Console.WindowWidth - 1));
                Console.ResetColor();
            }

            keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                currentIndex = (currentIndex - 1 + menuItems.Count) % menuItems.Count;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                currentIndex = (currentIndex + 1) % menuItems.Count;
            }

        } while (keyInfo.Key != ConsoleKey.Enter);

        return menuItems[currentIndex].Key;
    }
}
