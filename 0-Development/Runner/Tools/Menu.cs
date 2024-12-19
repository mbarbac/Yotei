using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
/// <summary>
/// Represents a console menu that permits selecting one among its entries and execute its
/// associated action, if any.
/// </summary>
public static class Menu
{
    /// <summary>
    /// Runs a console menu for the given set of menu entries, that the user can select using
    /// the [Up] and [Down] keys, and the [Enter] one to execute its associated action.
    /// <br/> Returns the index of the selected menu entry, or -1 if the [Escape] key was used
    /// to cancel the selection, or if the collection of entries was an empty one.
    /// </summary>
    /// <param name="colot"></param>
    /// <param name="timeout"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public static int Run(params MenuEntry[] entries)
    {
        return Run(Console.ForegroundColor, Timeout.InfiniteTimeSpan, entries);
    }

    /// <summary>
    /// Runs a console menu for the given set of menu entries, that the user can select using
    /// the [Up] and [Down] keys, and the [Enter] one to execute its associated action, waiting
    /// for at most the given amount of time.
    /// <br/> Returns the index of the selected menu entry, or -1 if the [Escape] key was used
    /// to cancel the selection, or if the timeout period has expired, or if the collection of
    /// entries was an empty one.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public static int Run(TimeSpan timeout, params MenuEntry[] entries)
    {
        return Run(Console.ForegroundColor, timeout, entries);
    }

    /// <summary>
    /// Runs a console menu for the given set of menu entries, that the user can select using
    /// the [Up] and [Down] keys, and the [Enter] one to execute its associated action, waiting
    /// for at most the given amount of time, and using the given color for each entry header.
    /// <br/> Returns the index of the selected menu entry, or -1 if the [Escape] key was used
    /// to cancel the selection, or if the timeout period has expired, or if the collection of
    /// entries was an empty one.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public static int Run(ConsoleColor color, params MenuEntry[] entries)
    {
        return Run(color, Timeout.InfiniteTimeSpan, entries);
    }

    /// <summary>
    /// Runs a console menu for the given set of menu entries, that the user can select using
    /// the [Up] and [Down] keys, and the [Enter] one to execute its associated action, waiting
    /// for at most the given amount of time, and using the given color for each entry header.
    /// <br/> Returns the index of the selected menu entry, or -1 if the [Escape] key was used
    /// to cancel the selection, or if the timeout period has expired, or if the collection of
    /// entries was an empty one.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="timeout"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public static int Run(ConsoleColor color, TimeSpan timeout, params MenuEntry[] entries)
    {
        entries.ThrowWhenNull();
        if (entries.Length == 0) return -1;

        if (Console.CursorTop >= (Console.BufferHeight - entries.Length - 1))
        {
            WriteLine();
            WriteLine(Red, "Screen buffer exhausted!");
            Write(Red, "Press [Enter] to clear the buffer... ");
            ReadLine();
            Clear();
        }

        var top = Console.CursorTop;
        var pos = 0;

        foreach (var entry in entries)
        {
            Write(color, "[ ] ");
            WriteLine(entry.Header());
        }

        while (true)
        {
            Console.CursorTop = top + pos;
            Console.CursorLeft = 1;

            // Timeout may have expired...
            var info =
                ReadKey(timeout, false) ??
                new ConsoleKeyInfo('\0', ConsoleKey.Escape, false, false, false);

            // Keys...
            switch (info.Key)
            {
                case ConsoleKey.Enter:
                    Console.Write("\u2588");

                    Console.CursorTop = top + entries.Length;
                    Console.CursorLeft = 0;
                    entries[pos].Execute();
                    return pos;

                case ConsoleKey.Escape:
                    Console.CursorTop = top + entries.Length;
                    Console.CursorLeft = 0;
                    return -1;

                case ConsoleKey.UpArrow:
                    if (pos > 0) pos--;
                    break;

                case ConsoleKey.DownArrow:
                    if (pos < (entries.Length - 1)) pos++;
                    break;

                case ConsoleKey.Home:
                    pos = 0;
                    break;

                case ConsoleKey.End:
                    pos = entries.Length - 1;
                    break;
            }
        }
    }
}