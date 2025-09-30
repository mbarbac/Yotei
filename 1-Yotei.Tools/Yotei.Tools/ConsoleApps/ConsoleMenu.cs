namespace Yotei.Tools;

// =============================================================
/// <summary>
/// Represents a console menu where the user can select from multiple entries associated with
/// their respective actions.
/// </summary>
public class ConsoleMenu : IEnumerable<ConsoleMenuEntry>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ConsoleMenu() { }

    /// <summary>
    /// Initializes a new instance with the given collection of elements.
    /// </summary>
    /// <param name="entries"></param>
    public ConsoleMenu(params ConsoleMenuEntry[] entries) => AddRange(entries);

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Entries.Count}";

    // ----------------------------------------------------

    readonly List<ConsoleMenuEntry> Entries = [];

    /// <inheritdoc/>
    public IEnumerator<ConsoleMenuEntry> GetEnumerator() => Entries.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Gets the number of entries in this instance.
    /// </summary>
    public int Count => Entries.Count;

    /// <summary>
    /// Returns the position at which the given menu entry is stored.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public int IndexOf(ConsoleMenuEntry entry) => Entries.IndexOf(entry.ThrowWhenNull());

    /// <summary>
    /// Gets the entry stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ConsoleMenuEntry this[int index] => Entries[index];

    /// <summary>
    /// Adds the given menu entry to this instance, provided it is not a duplicated one.
    /// </summary>
    /// <br/> Returns this instance to use it in a fluent syntax fashion.
    /// <param name="entry"></param>
    public ConsoleMenu Add(ConsoleMenuEntry entry)
    {
        entry.ThrowWhenNull();

        if (!Entries.Contains(entry)) Entries.Add(entry);
        else throw new DuplicateException("Duplicated entry.").WithData(entry);

        return this;
    }

    /// <summary>
    /// Adds the given collection of menu entries to this instance, provided there are no
    /// duplicated ones.
    /// <br/> Returns this instance to use it in a fluent syntax fashion.
    /// </summary>
    /// <param name="range"></param>
    public ConsoleMenu AddRange(params ConsoleMenuEntry[] range)
    {
        range.ThrowWhenNull();
        foreach (var entry in range) Add(entry);

        return this;
    }

    /// <summary>
    /// Inserts the given menu entry into this instance at the given index, provided it is not
    /// a duplicated one.
    /// <br/> Returns this instance to use it in a fluent syntax fashion.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="entry"></param>
    public ConsoleMenu Insert(int index, ConsoleMenuEntry entry)
    {
        entry.ThrowWhenNull();

        if (!Entries.Contains(entry)) Entries.Insert(index, entry);
        else throw new DuplicateException("Duplicated entry.").WithData(entry);

        return this;
    }

    /// <summary>
    /// Inserts the given collection of menu entries into this instance, starting at the given
    /// index, provided there are no duplicated ones.
    /// <br/> Returns this instance to use it in a fluent syntax fashion.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    public ConsoleMenu InsertRange(int index, IEnumerable<ConsoleMenuEntry> range)
    {
        range.ThrowWhenNull();
        foreach (var entry in range) Insert(index++, entry);

        return this;
    }

    /// <summary>
    /// Removes from this menu the entry at the given index.
    /// <br/> Returns this instance to use it in a fluent syntax fashion.
    /// </summary>
    /// <param name="index"></param>
    public ConsoleMenu RemoveAt(int index)
    {
        Entries.RemoveAt(index);
        return this;
    }

    /// <summary>
    /// Removes from this menu the given entry.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public ConsoleMenu Remove(ConsoleMenuEntry entry)
    {
        Entries.Remove(entry.ThrowWhenNull());
        return this;
    }

    /// <summary>
    /// Removes all the menu entries from this collection.
    /// <br/> Returns this instance to use it in a fluent syntax fashion.
    /// </summary>
    public ConsoleMenu Clear()
    {
        Entries.Clear();
        return this;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Runs this menu so that the user can select an entry and press [Enter] to execute its
    /// associated action, or press [Cancel] to exit the menu. Returns the cero-based position
    /// of the selected entry, or -1 if the menu was cancelled or the timeout expired.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public int Run(int position = 0) => Run(new(), position);

    /// <summary>
    /// Runs this menu so that the user can select an entry and press [Enter] to execute its
    /// associated action, or press [Cancel] to exit the menu. Returns the cero-based position
    /// of the selected entry, or -1 if the menu was cancelled or the timeout expired.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public int Run(ConsoleMenuOptions options, int position = 0)
    {
        if (position < 0 || position >= Entries.Count)
            throw new ArgumentException("Invalid menu index.").WithData(position);

        // Initializing...
        var top = Console.CursorTop; ValidateCursorTop(ref top);
        var selector = options.SelectorColor;
        var descriptor = options.DescriptionColor;
        var background = options.BackgroundColor;
        var debug = options.Debug;
        var timeout = options.Timeout;

        for (int i = 0; i < Entries.Count; i++)
        {
            ConsoleEx.Write(debug, selector, background, "[ ] ");
            ConsoleEx.WriteLine(debug, descriptor, background, Entries[i].Header());
        }

        // Executing...
        while (true)
        {
            Console.CursorTop = top + position;
            Console.CursorLeft = 1;

            var info = ConsoleEx.ReadKey(true, false, timeout);
            info ??= new ConsoleKeyInfo('\0', ConsoleKey.Escape, false, false, false);

            switch (info.Value.Key)
            {
                case ConsoleKey.Enter:
                    ConsoleEx.Write(descriptor, background, "\u2588");
                    if (debug) ConsoleEx.WithNoConsoleListeners(
                        () => Debug.WriteLine($"Selected: {position}."));

                    Console.CursorTop = top + Entries.Count;
                    Console.CursorLeft = 0;
                    Entries[position].Execute();
                    return position;

                case ConsoleKey.Escape:
                    Console.CursorTop = top + Entries.Count;
                    Console.CursorLeft = 0;

                    if (debug) ConsoleEx.WithNoConsoleListeners(() => Debug.WriteLine("Cancelled."));
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

        /// <summary>
        /// Invoked to validate the top cursor position.
        /// </summary>
        void ValidateCursorTop(ref int top)
        {
            var max = Console.BufferHeight - Entries.Count - 1;
            if (top >= max)
            {
                Clear();
                Console.Clear();
                ConsoleEx.WriteLine(ConsoleColor.Red, "Screen buffer exhausted and cleared!");
                Console.WriteLine();
                top = Console.CursorTop;
            }
        }
    }
}