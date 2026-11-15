namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a console menu where the user can select among the registered entries to execute
/// their associated actions, if any.
/// </summary>
public partial record ConsoleMenu : IEnumerable<ConsoleMenu.Entry>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ConsoleMenu() { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    public ConsoleMenu(ConsoleMenu other)
    {
        other.ThrowWhenNull();
        ToDebug = other.ToDebug;
        Timeout = other.Timeout;
        BackgroundColor = other.BackgroundColor;
        ForegroundColor = other.ForegroundColor;
        SelectorColor = other.SelectorColor;
        Entries = [.. other];
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    // ----------------------------------------------------

    /// <summary>
    /// Whether to write into the debug envirnoment the result of the selection, if any.
    /// </summary>
    public bool ToDebug { get; set; }

    /// <summary>
    /// The interval to wait for the user to make a selection. If not infinite (-1), when this
    /// interval expires, the menu returns with an invalid position.
    /// </summary>
    public TimeSpan Timeout
    {
        get;
        set => field = TimeSpan.FromMilliseconds(value.ValidatedTimeout);
    }
    = System.Threading.Timeout.InfiniteTimeSpan;

    /// <summary>
    /// The foreground color to use with the entries description.
    /// </summary>
    public ConsoleColor ForegroundColor { get; set; } = Console.ForegroundColor;

    /// <summary>
    /// The background color to use with the entries description.
    /// </summary>
    public ConsoleColor BackgroundColor { get; set; } = Console.BackgroundColor;

    /// <summary>
    /// The color to use with the menu selector.
    /// </summary>
    public ConsoleColor SelectorColor { get; set; } = Console.ForegroundColor == ConsoleColor.Green
        ? ConsoleColor.White
        : ConsoleColor.Green;

    // ----------------------------------------------------

    readonly List<Entry> Entries = [];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<Entry> GetEnumerator() => Entries.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Get the number of menu entries in this instance.
    /// </summary>
    public int Count => Entries.Count;

    /// <summary>
    /// Gets the menu entry at the given position.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Entry this[int index] => Entries[index];

    /// <summary>
    /// Gets the position at which the given menu entry is stored, or -1 if any.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public int IndexOf(Entry entry) => Entries.IndexOf(entry.ThrowWhenNull());

    /// <summary>
    /// Adds the given menu entry to this instance.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public ConsoleMenu Add(Entry entry)
    {
        if (!Entries.Contains(entry.ThrowWhenNull())) Entries.Add(entry);
        return this;
    }

    /// <summary>
    /// Adds the menu entries of the given range to this instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public ConsoleMenu AddRange(IEnumerable<Entry> range)
    {
        range.ThrowWhenNull();
        foreach (var entry in range) Add(entry);
        return this;
    }

    /// <summary>
    /// Inserts the given menu entry into this instance, at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public ConsoleMenu Insert(int index, Entry entry)
    {
        if (!Entries.Contains(entry.ThrowWhenNull())) Entries.Insert(index, entry);
        return this;
    }

    /// <summary>
    /// Inserts the menu entries of the given range into this instance, starting at the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public ConsoleMenu InsertRange(int index, IEnumerable<Entry> range)
    {
        range.ThrowWhenNull();
        foreach (var entry in range) Add(entry);
        return this;
    }

    /// <summary>
    /// Removes from this instance the entry at the given position.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ConsoleMenu RemoveAt(int index)
    {
        Entries.RemoveAt(index);
        return this;
    }

    /// <summary>
    /// Removes from this instance the given entry.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public ConsoleMenu Remove(Entry entry)
    {
        var index = IndexOf(entry);
        if (index >= 0) RemoveAt(index);
        return this;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Runs this menu, starting at the entry whose index is given, so that the user can select
    /// any of the registered ones, by pressing [Enter], and automatically execute its associated
    /// action. Returns the index of the selected action, or -1 if the timeout has expired, or if
    /// [Esc] has been pressed to cancel the menu.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public int Run(int position = 0)
    {
        // Hack to prevent xUnit to emit an annoying 'invalid handle' warning...
        if (Console.IsOutputRedirected) return -1;

        // Intercepting obvious cases...
        if (position < 0) throw new ArgumentOutOfRangeException(nameof(position)).WithData(position);
        if (Count == 0) return -1;

        // Initializing...
        Console.PreventBufferOverflow(Count + 5); // Plus 5 lines just by convention...
        var top = Console.CursorTop;

        for (int i = 0; i < Entries.Count; i++)
        {
            Console.WriteEx(ToDebug, SelectorColor, BackgroundColor, "[ ] ");
            Console.WriteLineEx(ToDebug, ForegroundColor, BackgroundColor, Entries[i].Headline());
        }

        // Executing...
        while (true)
        {
            Console.CursorTop = top + position;
            Console.CursorLeft = 1;

            var info = Console.ReadKey(Timeout, hide: true);
            info ??= new ConsoleKeyInfo('\0', ConsoleKey.Escape, false, false, false);

            switch (info.Value.Key)
            {
                case ConsoleKey.Enter:
                    ConsoleExtensions.TryDebugWriteLine(ToDebug, $"Selected: {position}.");
                    Console.Write(ForegroundColor, BackgroundColor, "\u2588");
                    Console.CursorTop = top + Entries.Count;
                    Console.CursorLeft = 0;
                    Entries[position].Execute();
                    return position;

                case ConsoleKey.Escape:
                    Console.CursorTop = top + Entries.Count;
                    Console.CursorLeft = 0;
                    return -1;

                case ConsoleKey.UpArrow:
                    if (position > 0) position--;
                    break;

                case ConsoleKey.DownArrow:
                    if (position < (Entries.Count - 1)) position++;
                    break;

                case ConsoleKey.Home:
                    position = 0;
                    break;

                case ConsoleKey.End:
                    position = Entries.Count - 1;
                    break;
            }
        }
    }
}