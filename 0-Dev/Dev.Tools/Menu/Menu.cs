namespace Dev.Tools;

// ========================================================
/// <summary>
/// Represents a console menu.
/// </summary>
public static class Menu
{
    /// <summary>
    /// Presents a menu with the given entries so that the user can select one among them using
    /// the up and down arrow keys, until either the [Enter] or [Escape] keys are used. Returns
    /// the index of the entry selected, or -1 if the [Escape] key was used.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public static int Run(
        Color color, params MenuEntry[] entries) => Run(TimeSpan.Zero, color, entries);

    /// <summary>
    /// Presents a menu with the given entries so that the user can select one among them using
    /// the up and down arrow keys, waiting for at most the given wait period to either press
    /// the [Enter] or [Escape] keys. Returns the index of the entry selected, or -1 if the
    /// wait period is over, or if the [Escape] key was used. A wait period of zero means wait
    /// forever.
    /// </summary>
    /// <param name="wait"></param>
    /// <param name="color"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public static int Run(TimeSpan wait, Color color, params MenuEntry[] entries)
    {
        entries = entries.ThrowIfNull();
        if (entries.Length == 0) return -1;

        var size = GetCursorSize(); SetCursorSize(100);
        var pos = 0;
        var ini = GetCursorTop();
        foreach (var entry in entries) { Write(color, "[ ] "); entry.Print(); }
        
        while (true)
        {
            SetCursorTop(ini + pos);
            SetCursorLeft(1);

            var info = ReadKey(wait, intercept: true);

            if (info == null) { SelectionDone(); return -1; }

            switch (info.Value.Key)
            {
                case ConsoleKey.UpArrow: if (pos > 0) pos--; break;
                case ConsoleKey.DownArrow: if (pos < (entries.Length - 1)) pos++; break;
                case ConsoleKey.Home: pos = 0; break;
                case ConsoleKey.End: pos = entries.Length - 1; break;

                case ConsoleKey.Escape:
                    SelectionDone();
                    return -1;

                case ConsoleKey.Enter:
                    SelectionDone(true);
                    entries[pos].Execute();
                    return pos;
            }
        }

        static int GetCursorTop() => _Console.CursorTop;
        static void SetCursorTop(int value) => _Console.CursorTop = value;
        static void SetCursorLeft(int value) => _Console.CursorLeft = value;
        static int GetCursorSize() => _Console.CursorSize;
        static void SetCursorSize(int value)
        {
            if (OperatingSystem.IsWindows()) _Console.CursorSize = value;
        }
        void SelectionDone(bool show = false)
        {
            SetCursorSize(size);
            if (show) Write("\u2588");
            SetCursorTop(ini + entries.Length);
            SetCursorLeft(0);
        }
    }
}