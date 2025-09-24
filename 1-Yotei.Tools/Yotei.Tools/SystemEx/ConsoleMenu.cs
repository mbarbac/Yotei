using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a console menu that permits the user to select among its entries, executing their
/// associated actions, if any.
/// </summary>
public class ConsoleMenu : IEnumerable<ConsoleMenuEntry>
{
    List<ConsoleMenuEntry> Entries { get; } = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ConsoleMenu() { }

    /// <summary>
    /// Initializes a new instance with the given collection of elements.
    /// </summary>
    /// <param name="entries"></param>
    public ConsoleMenu(params IEnumerable<ConsoleMenuEntry> entries) => AddRange(entries);

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Entries.Count}";

    // ----------------------------------------------------

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
    /// <param name="entry"></param>
    public void Add(ConsoleMenuEntry entry)
    {
        entry.ThrowWhenNull();

        if (!Entries.Contains(entry)) Entries.Add(entry);
        else throw new DuplicateException("Duplicated entry.").WithData(entry);
    }

    /// <summary>
    /// Adds the given collection of menu entries to this instance, provided there are no
    /// duplicated ones.
    /// </summary>
    /// <param name="range"></param>
    public void AddRange(IEnumerable<ConsoleMenuEntry> range)
    {
        range.ThrowWhenNull();
        foreach (var entry in range) Add(entry);
    }

    /// <summary>
    /// Inserts the given menu entry into this instance at the given index, provided it is not
    /// a duplicated one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="entry"></param>
    public void Insert(int index, ConsoleMenuEntry entry)
    {
        entry.ThrowWhenNull();

        if (!Entries.Contains(entry)) Entries.Insert(index, entry);
        else throw new DuplicateException("Duplicated entry.").WithData(entry);
    }

    /// <summary>
    /// Inserts the given collection of menu entries into this instance, starting at the given
    /// index, provided there are no duplicated ones.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    public void InsertRange(int index, IEnumerable<ConsoleMenuEntry> range)
    {
        range.ThrowWhenNull();
        foreach (var entry in range) Insert(index++, entry);
    }

    /// <summary>
    /// Removes from this menu the entry at the given index.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveAt(int index) => Entries.RemoveAt(index);

    /// <summary>
    /// Removes from this menu the given entry.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public bool Remove(ConsoleMenuEntry entry) => Entries.Remove(entry.ThrowWhenNull());

    /// <summary>
    /// Removes all the menu entries from this collection.
    /// </summary>
    public void Clear() => Entries.Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Runs this menu so that the user can select the desired entry and press [Enter] to execute
    /// its associated action, or [Escape] to cancel the menu. Selection is done with the [Up],
    /// [Down], [Ini] and [End] keys. Cancellation also happens automatically when this menu is
    /// an empty one. Returns either the index of the selected menu, or -1 if cancellation has
    /// happened.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public int Run(
        ConsoleColor color, int position = 0)
        => Run(false, color, Timeout.InfiniteTimeSpan, position);

    /// <summary>
    /// Runs this menu so that the user can select the desired entry and press [Enter] to execute
    /// its associated action, or [Escape] to cancel the menu. Selection is done with the [Up],
    /// [Down], [Ini] and [End] keys. Cancellation also happens automatically when the timeout
    /// period expires, or when this menu is an empty one. Returns either the index of the selected
    /// menu, or -1 if cancellation has happened.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="timeout"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public int Run(
        ConsoleColor color, TimeSpan timeout, int position = 0)
        => Run(false, color, timeout, position);

    /// <summary>
    /// Runs this menu so that the user can select the desired entry and press [Enter] to execute
    /// its associated action, or [Escape] to cancel the menu. Selection is done with the [Up],
    /// [Down], [Ini] and [End] keys. Cancellation also happens automatically when this menu is
    /// an empty one. Returns either the index of the selected menu, or -1 if cancellation has
    /// happened.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="color"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public int Run(bool debug, ConsoleColor color, int position = 0)
        => Run(debug, color, Timeout.InfiniteTimeSpan, position);

    /// <summary>
    /// Runs this menu so that the user can select the desired entry and press [Enter] to execute
    /// its associated action, or [Escape] to cancel the menu. Selection is done with the [Up],
    /// [Down], [Ini] and [End] keys. Cancellation also happens automatically when the timeout
    /// period expires, or when this menu is an empty one. Returns either the index of the selected
    /// menu, or -1 if cancellation has happened.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="color"></param>
    /// <param name="timeout"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public int Run(bool debug, ConsoleColor color, TimeSpan timeout, int position = 0)
    {
        if (position < 0) throw new ArgumentException("Menu index cannot be negative.").WithData(position);
        if (position >= Entries.Count) throw new ArgumentException("Menu index too big.").WithData(position);
        timeout.ValidateTimeout();

        // Initializing...
        var top = Console.CursorTop;
        if (top >= (Console.BufferHeight - Entries.Count - 1))
        {
            ConsoleEx.Clear();
            WriteLine(DarkYellow, "Screen buffer exhausted and cleared!");
            WriteLine();
            top = Console.CursorTop;
        }
        foreach (var entry in Entries)
        {
            Write(color, "[ ] "); if (debug) DebugEx.Write(false, "[ ] ");
            WriteLine(entry.Header()); if (debug) DebugEx.WriteLine(false, entry.Header());
        }

        // Executing...
        while (true)
        {
            Console.CursorTop = top + position;
            Console.CursorLeft = 1;

            var info = ReadKey(false, timeout);
            info ??= new ConsoleKeyInfo('\0', ConsoleKey.Escape, false, false, false);

            switch (info.Value.Key)
            {
                case ConsoleKey.Enter:
                    Console.Write("\u2588");
                    if (debug) DebugEx.WriteLine(false, $"Selected: {position}");

                    Console.CursorTop = top + Entries.Count;
                    Console.CursorLeft = 0;
                    Entries[position].Execute();
                    return position;

                case ConsoleKey.Escape:
                    Console.CursorTop = top + Entries.Count;
                    Console.CursorLeft = 0;

                    if (debug) DebugEx.WriteLine(false, "Cancelled");
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