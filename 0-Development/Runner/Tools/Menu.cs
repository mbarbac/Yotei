namespace Runner;

// ========================================================
/// <summary>
/// Represents a console menu from which the actions associated with its entries can be executed.
/// </summary>
public static class Menu
{
    /// <summary>
    /// Runs a console menu for the given collection of entries. The user can select among them
    /// by using the [Up] and [Down] keys, and then the [Enter] one to execute the associated
    /// action. Returns the index of the selected menu entry, or -1 if the [Escape] key was
    /// pressed, or if the collection of entries was an empty one.
    /// </summary>
    /// <param name="entries"></param>
    /// <returns></returns>
    public static int Run(params MenuEntry[] entries)
    {
        return Run(Console.ForegroundColor, Timeout.InfiniteTimeSpan, entries);
    }

    /// <summary>
    /// Runs a console menu for the given collection of entries. The user can select among them
    /// by using the [Up] and [Down] keys, and then the [Enter] one to execute the associated
    /// action. Returns the index of the selected menu entry, or -1 if the [Escape] key was
    /// pressed, if the timeout period has expired, or if the collection of entries was an
    /// empty one.
    /// <para>A timeout period of -1 milliseconds means wait indefinitely.</para>
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public static int Run(TimeSpan timeout, params MenuEntry[] entries)
    {
        return Run(Console.ForegroundColor, timeout, entries);
    }

    /// <summary>
    /// Runs a console menu for the given collection of entries. The user can select among them
    /// by using the [Up] and [Down] keys, and then the [Enter] one to execute the associated
    /// action. Returns the index of the selected menu entry, or -1 if the [Escape] key was
    /// pressed, or if the collection of entries was an empty one.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public static int Run(ConsoleColor color, params MenuEntry[] entries)
    {
        return Run(color, Timeout.InfiniteTimeSpan, entries);
    }

    /// <summary>
    /// Runs a console menu for the given collection of entries. The user can select among them
    /// by using the [Up] and [Down] keys, and then the [Enter] one to execute the associated
    /// action. Returns the index of the selected menu entry, or -1 if the [Escape] key was
    /// pressed, if the timeout period has expired, or if the collection of entries was an
    /// empty one.
    /// <para>A timeout period of -1 milliseconds means wait indefinitely.</para>
    /// </summary>
    /// <param name="color"></param>
    /// <param name="timeout"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public static int Run(ConsoleColor color, TimeSpan timeout, params MenuEntry[] entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        if (entries.Length == 0) return -1;

        if (Console.CursorTop >= (Console.BufferHeight - entries.Length - 1))
        {
            ConsoleWrapper.WriteLine();
            ConsoleWrapper.WriteLine(ConsoleColor.Red, "Screen buffer exhausted!");
            ConsoleWrapper.Write(ConsoleColor.Red, "Press [Enter] to clear the buffer... ");
            Console.ReadLine();
            Console.Clear();
        }

        var top = Console.CursorTop;
        var pos = 0;

        foreach (var entry in entries)
        {
            ConsoleWrapper.Write(color, "[ ] ");
            ConsoleWrapper.WriteLine(entry.Header());
        }

        while (true)
        {
            Console.CursorTop = top + pos;
            Console.CursorLeft = 1;

            // Timeout may have expired...
            var info =
                ConsoleWrapper.ReadKey(timeout, false) ??
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