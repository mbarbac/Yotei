using static System.ConsoleColor;

namespace Yotei.Tools;

// =============================================================
/// <summary>
/// Represents a console menu where the user can select from multiple entries associated with their
/// respective actions.
/// </summary>
public class ConsoleMenu : IEnumerable<ConsoleMenuEntry>
{
    readonly List<ConsoleMenuEntry> Entries = [];

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

    // ---------------------------------------------------------

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

    // ---------------------------------------------------------

    // ---------------------------------------------------------

    /// <summary>
    /// Runs this menu, allowing the user to select an entry and press [Enter] to execute its
    /// associated action, or press [Esc] to exit the menu. Returns the cero-based position of
    /// the selected entry, or -1 if the user exited the menu without selecting any entry, or
    /// if the timeout expired.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="timeout"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public int Run(
        bool debug, ConsoleColor forecolor, ConsoleColor backcolor,
        TimeSpan timeout, int position = 0)
    {
        _ = timeout.ValidatedTimeout;
        if (position < 0 || position >= Entries.Count)
            throw new ArgumentException("Invalid menu index.").WithData(position);

        // Initializing...
        var top = Console.CursorTop;
        ValidateCursorTop(ref top);

        for (int i = 0; i < Entries.Count; i++)
        {
            Console.WriteEx(debug, forecolor, backcolor, "[] ");
            Console.WriteLineEx(debug, forecolor, backcolor, Entries[i].Header());
        }

        // Executing...

        throw null;

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
                Console.WriteLineEx(Red, "Screen buffer exhausted and cleared!");
                Console.WriteLine();
                top = Console.CursorTop;
            }
        }
    }
}