namespace Dev.Tools;

// ========================================================
/// <summary>
/// Represents a console menu.
/// </summary>
public class Menu
{
    /// <summary>
    /// Presents a menu with the given items from which the user can select what action to run
    /// using the up and down arrow keys. A default exit option is always injected as the first
    /// one. The menu is run until that first action is selected or the [Escape] key is used.
    /// The header action is invoked each time the menu is run.
    /// </summary>
    /// <param name="header"></param>
    /// <param name="items"></param>
    public void Run(Action header, params MenuItem[] items) => Run(header, TimeSpan.Zero, items);

    /// <summary>
    /// Presents a menu with the given items from which the user can select what action to run
    /// using the up and down arrow keys, waiting for at most the given wait period. A default
    /// exit option is always injected as the first one. The menu is run until that first action
    /// is selected, the wait period is over, or the [Escape] key is used. The header action is
    /// invoked each time the menu is run. A wait period of zero means wait forever.
    /// </summary>
    /// <param name="header"></param>
    /// <param name="wait"></param>
    /// <param name="items"></param>
    public void Run(Action header, TimeSpan wait, params MenuItem[] items)
    {
        header = header.ThrowIfNull();
        items = items.ThrowIfNull();

        var exit = new MenuItem(print: () => WriteLine("Exit"), execute: () => { });
        var list = new List<MenuItem> { exit };
        foreach (var item in items) list.Add(item.ThrowIfNull());
        items = list.ToArray();

        while (true)
        {
            header();
            var ini = GetCursorTop();            
            foreach (var item in items) { Write(Color.Magenta, "[ ] "); item.Print(); }

            var size = GetCursorSize(); SetCursorSize(100);
            var pos = 0;
            var done = false; do
            {
                SetCursorTop(ini + pos);
                SetCursorLeft(1);

                var info = ReadKey(wait, intercept: true);

                if (info == null) { SelectionDone(); return; }
                switch (info.Value.Key)
                {
                    case ConsoleKey.Escape: SelectionDone(); return;
                    case ConsoleKey.UpArrow: if (pos > 0) pos--; break;
                    case ConsoleKey.DownArrow: if (pos < (items.Length - 1)) pos++; break;
                    case ConsoleKey.Enter:
                        SelectionDone(true); if (pos == 0) return;
                        items[pos].Execute();
                        done = true;
                        break;
                }
            }
            while (!done);

            void SelectionDone(bool show = false)
            {
                if (show) Write("\u2588");
                SetCursorTop(ini + items.Length); WriteLine();
                SetCursorSize(size);
            }
        }

        static int GetCursorTop() => _Console.CursorTop;
        static void SetCursorTop(int value) => _Console.CursorTop = value;
        static void SetCursorLeft(int value) => _Console.CursorLeft = value;
        static int GetCursorSize() => _Console.CursorSize;
        static void SetCursorSize(int value) { if (OperatingSystem.IsWindows()) _Console.CursorSize = value; }
    }
}