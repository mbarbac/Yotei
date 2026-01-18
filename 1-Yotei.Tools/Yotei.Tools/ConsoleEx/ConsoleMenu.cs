using System.Security.Cryptography;

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a console menu where the user can select among the registered entries to execute
/// their associated actions.
/// </summary>
public record ConsoleMenu : IEnumerable<ConsoleMenuEntry>
{
    /// <summary>
    /// Whether to replicate the result of the user selection in the debug environment.
    /// </summary>
    public bool ToDebug { get; set; }

    /// <summary>
    /// The interval to wait for the user to select an entry. If not set, then this instance
    /// will wait for an infinite amount of time.
    /// </summary>
    public TimeSpan Timeout
    {
        get;
        set
        {
            field = value.TotalMilliseconds is < (-1) or > int.MaxValue
                ? throw new ArgumentOutOfRangeException($"Invalid timeout value: {value}")
                : value;
        }
    }
    = System.Threading.Timeout.InfiniteTimeSpan;

    /// <summary>
    /// The background color to use with menu entries.
    /// </summary>
    public ConsoleColor BackgroundColor { get; set; } = Console.BackgroundColor;

    /// <summary>
    /// The foreground color to use with menu entries.
    /// </summary>
    public ConsoleColor ForegroundColor { get; set; } = Console.ForegroundColor;

    /// <summary>
    /// The color to use with the menu selector.
    /// </summary>
    public ConsoleColor SelectorColor { get; set; } = Console.ForegroundColor == ConsoleColor.Green
        ? ConsoleColor.White
        : ConsoleColor.Green;

    // ----------------------------------------------------

    readonly List<ConsoleMenuEntry> Entries = [];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<ConsoleMenuEntry> GetEnumerator() => Entries.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Gets the number of entries in this instance.
    /// </summary>
    public int Count => Entries.Count;

    /// <summary>
    /// Gets the entry stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ConsoleMenuEntry this[int index] => Entries[index];

    /// <summary>
    /// Returns the position at which the given menu entry is stored.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public int IndexOf(ConsoleMenuEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        return Entries.IndexOf(entry);
    }

    /// <summary>
    /// Adds the given entry to this collection.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns>This instance to allow its use in a fluent-syntax fashion.</returns>
    public ConsoleMenu Add(ConsoleMenuEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        Entries.Add(entry);
        return this;
    }

    /// <summary>
    /// Adds the entries of the given range to this collection.
    /// </summary>
    /// <param name="range">This instance to allow its use in a fluent-syntax fashion.</param>
    /// <returns></returns>
    public ConsoleMenu AddRange(params ConsoleMenuEntry[] range)
    {
        ArgumentNullException.ThrowIfNull(range);
        foreach (var entry in range) Add(entry);
        return this;
    }

    /// <summary>
    /// Inserts the given entry into this collection at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="entry"></param>
    /// <returns>This instance to allow its use in a fluent-syntax fashion.</returns>
    public ConsoleMenu Insert(int index, ConsoleMenuEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        Entries.Insert(index, entry);
        return this;
    }

    /// <summary>
    /// Inserts the entries of the given range into this collection, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range">This instance to allow its use in a fluent-syntax fashion.</param>
    /// <returns></returns>
    public ConsoleMenu AddRange(int index, params ConsoleMenuEntry[] range)
    {
        ArgumentNullException.ThrowIfNull(range);
        foreach (var entry in range) { Insert(index, entry); index++; }
        return this;
    }

    /// <summary>
    /// Removes from this collection the entry at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>This instance to allow its use in a fluent-syntax fashion.</returns>
    public ConsoleMenu RemoveAt(int index)
    {
        Entries.RemoveAt(index);
        return this;
    }

    /// <summary>
    /// Removes from this collection the given entry.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns>This instance to allow its use in a fluent-syntax fashion.</returns>
    public ConsoleMenu Remove(ConsoleMenuEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        Entries.Remove(entry);
        return this;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ConsoleMenu() { }

    /// <summary>
    /// Copy constructor to customize 'with' behavior.
    /// </summary>
    /// <param name="source"></param>
    protected ConsoleMenu(ConsoleMenu source)
    {
        ToDebug = source.ToDebug;
        Timeout = source.Timeout;
        BackgroundColor = source.BackgroundColor;
        ForegroundColor = source.ForegroundColor;
        SelectorColor = source.SelectorColor;
        Entries = [.. source];
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    // ----------------------------------------------------

    /// <summary>
    /// Runs this menu so that the user can select an entry and then press [Enter] to execute its
    /// associated action, if any, or [Escape] to exit the menu. Retuns the cero-based position of
    /// the selected entry, or -1 if the menu was cancelled or the timeout expired.
    /// </summary>
    /// <param name="position">The initial entry to be selected.</param>
    /// <returns>The position of the selected entry, or -1 if any.</returns>
    public int Run(int position = 0)
    {
        // Intercepting obvious cases...
        if (position < 0) ArgumentOutOfRangeException.ThrowIfNegative(position);
        if (Count == 0) return -1;

        // Initializing...
        var top = Console.CursorTop;

        for (int i = 0; i < Entries.Count; i++)
        {
            Console.WriteEx(ToDebug, SelectorColor, BackgroundColor, "[ ] ");
            Console.WriteLineEx(ToDebug, ForegroundColor, BackgroundColor, Entries[i].Header());
        }

        // Executing...
        while (true)
        {
            Console.CursorTop = top + position;
            Console.CursorLeft = 1;

            var info = Console.ReadKey(Timeout, intercept: true);
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