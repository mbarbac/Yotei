using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
/// <summary>
/// Represents a console menu that permits the user to select among its entries, executing
/// their associated action, if any.
/// </summary>
public class MenuConsole : IEnumerable<MenuEntry>
{
    List<MenuEntry> Entries { get; } = [];

    /// <summary>
    /// Initializes an empty instance.
    /// </summary>
    public MenuConsole() { }

    /// <summary>
    /// Initializes a new instance with the given collection of entries.
    /// </summary>
    /// <param name="entries"></param>
    public MenuConsole(params IEnumerable<MenuEntry> entries) => AddRange(entries);

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Entries.Count}";

    /// <inheritdoc/>
    public IEnumerator<MenuEntry> GetEnumerator() => Entries.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of entries in this instance.
    /// </summary>
    public int Count => Entries.Count;

    /// <summary>
    /// Gets the entry stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public MenuEntry this[int index] => Entries[index];

    /// <summary>
    /// Adds the given menu entry to this instance. This method throws an exception if that entry
    /// is a duplicated one.
    /// </summary>
    /// <param name="entry"></param>
    public void Add(MenuEntry entry)
    {
        entry.ThrowWhenNull();

        lock (Entries)
        {
            if (Entries.Contains(entry))
                throw new DuplicateException("Duplicated entry.").WithData(entry);

            Entries.Add(entry);
        }
    }

    /// <summary>
    /// Adds the given collection of menu entries to this instance. This method throws an exception
    /// if any entry is a duplicated one.
    /// </summary>
    /// <param name="entries"></param>
    public void AddRange(IEnumerable<MenuEntry> entries)
    {
        entries.ThrowWhenNull();

        lock (Entries)
        {
            foreach (var entry in entries) Add(entry);
        }
    }

    /// <summary>
    /// Inserts the given menu entry into this instance, at the given index. This method throws
    /// an exception if that entry is a duplicated one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="entry"></param>
    public void Insert(int index, MenuEntry entry)
    {
        entry.ThrowWhenNull();

        lock (Entries)
        {
            if (Entries.Contains(entry))
                throw new DuplicateException("Duplicated entry.").WithData(entry);

            Entries.Insert(index, entry);
        }
    }

    /// <summary>
    /// Inserts the given collection of menu entries into this instance, starting at the given
    /// index. This method throws an exception if any entry is a duplicated one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="entries"></param>
    public void InsertRange(int index, IEnumerable<MenuEntry> entries)
    {
        entries.ThrowWhenNull();

        lock (Entries)
        {
            foreach (var entry in entries)
            {
                Entries.InsertRange(index, entry);
                index++;
            }
        }
    }

    /// <summary>
    /// Tries to remove the given menu entry from this instance.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public bool Remove(MenuEntry entry) => Entries.Remove(entry.ThrowWhenNull());

    /// <summary>
    /// Clears all menu entries.
    /// </summary>
    public void Clear() => Entries.Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Runs this menu so that the user can select an entry using the [Up], [Down], [Ini] and
    /// [End] keys to move among them, the [Enter] one to select the one to execute, or the
    /// [Escape] to cancel the menu, waiting at most for the given amount of time, using the
    /// given foreground color for each entry description.
    /// <br/> The entry selected by default is given by the position parameter.
    /// <br/> Returns either the index of the selected menu entry, or -1 if the [Escape] key
    /// was pressed, if the timeout period has expired, or if the collection of entries was an
    /// empty one.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="timeout"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    [SuppressMessage("Interoperability", "CA1416")] // MoveBufferArea
    public int Run(ConsoleColor color, TimeSpan timeout, int position = 0)
    {
        if (position < 0) throw new ArgumentException("Negative position").WithData(position);
        if (position >= Entries.Count) throw new ArgumentException("Position too big.").WithData(position);
        timeout.ValidateTimeout();

        lock (Entries)
        {
            if (Entries.Count == 0) return -1;

            // Initializing...
            var top = Console.CursorTop;

            // It happens from time to time...
            if (Console.CursorTop >= (Console.BufferHeight - Entries.Count - 1))
            {
                ConsoleEx.Clear();
                WriteLine(Red, "Screen buffer exhausted and cleared!");
                WriteLine();
            }

            foreach (var entry in Entries)
            {
                Write(color, "[ ] ");
                WriteLine(entry.Header());
            }

            // Executing...
            while (true)
            {
                Console.CursorTop = top + position;
                Console.CursorLeft = 1;

                // Timeout expired is the same as pressing [Escape]...
                var info =
                    ReadKey(timeout, false) ??
                    new ConsoleKeyInfo('\0', ConsoleKey.Escape, false, false, false);

                // Processing pressed keys...
                switch (info.Key)
                {
                    case ConsoleKey.Escape:
                        Console.CursorTop = top + Entries.Count;
                        Console.CursorLeft = 0;
                        return -1;

                    case ConsoleKey.Enter:
                        Console.Write("\u2588");

                        Console.CursorTop = top + Entries.Count;
                        Console.CursorLeft = 0;
                        Entries[position].Execute();
                        return position;

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
}