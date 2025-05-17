namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a flatten and ordered collection of tokens.
/// </summary>
[Cloneable]
[DebuggerDisplay("{Items.ToDebugString(5)}")]
public partial class DbTokenChain : DbToken, IEnumerable<DbToken>
{
    readonly Builder Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public DbTokenChain() { }

    /// <summary>
    /// Initializes a new empty instance with the given capacity.
    /// </summary>
    /// <param name="capacity"></param>
    public DbTokenChain(int capacity) => Items.Capacity = capacity;

    /// <summary>
    /// Initializes a new instance with the given token.
    /// </summary>
    /// <param name="token"></param>
    public DbTokenChain(DbToken token) => Items.Add(token);

    /// <summary>
    /// Initializes a new instance with the tokens of the given range.
    /// </summary>
    /// <param name="range"></param>
    public DbTokenChain(IEnumerable<DbToken> range) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenChain(DbTokenChain source) => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    /// <summary>
    /// Returns a collection-alike string representation of this instance.
    /// </summary>
    /// <param name="rounded"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public string ToString(bool rounded, string separator = ", ")
    {
        separator.ThrowWhenNull();

        var head = rounded ? '(' : '[';
        var tail = rounded ? ')' : ']';

        var sb = new StringBuilder();
        sb.Append(head);

        for (int i = 0; i < Count; i++)
        {
            var item = Items[i].ToString();

            if (i > 0) sb.Append(separator);
            sb.Append(item);
        }

        sb.Append(tail);
        return sb.ToString();
    }

    /// <inheritdoc/>
    public IEnumerator<DbToken> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            var arg = Items[i].GetArgument();
            if (arg is not null) return arg;
        }
        return null;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (other is DbTokenChain xother)
        {
            if (Count != xother.Count) return false;
            for (int i = 0; i < Count; i++)
            {
                var item = Items[i];
                var temp = xother.Items[i];
                if (!item.Equals(temp)) return false;
            }
        }
        return ReferenceEquals(this, other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, this[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new builder populated with the elements of this collection.
    /// </summary>
    /// <returns></returns>
    public Builder GetBuilder() => Items.Clone();

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Gets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public DbToken this[int index] => Items[index];

    /// <summary>
    /// Determines if this collection contains the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(DbToken item) => Items.Contains(item);

    /// <summary>
    /// Gets the index of the first ocurrence of the given element in this collection, or -1 if
    /// any is found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(DbToken item) => Items.IndexOf(item);

    /// <summary>
    /// Gets the index of the last ocurrence of the given element in this collection, or -1 if
    /// any is found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(DbToken item) => Items.LastIndexOf(item);

    /// <summary>
    /// Gets the indexes of all the elements in this collection with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<int> IndexesOf(DbToken item) => Items.IndexesOf(item);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<DbToken> predicate) => Items.Contains(predicate);

    /// <summary>
    /// Gets the index of the first element in this collection that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<DbToken> predicate) => Items.IndexOf(predicate);

    /// <summary>
    /// Gets the index of the last element in this collection that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<DbToken> predicate) => Items.LastIndexOf(predicate);

    /// <summary>
    /// Gets the indexes of all the elements in this collection that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<DbToken> predicate) => Items.IndexesOf(predicate);

    /// <summary>
    /// Gets an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public DbToken[] ToArray() => Items.ToArray();

    /// <summary>
    /// Gets a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public List<DbToken> ToList() => Items.ToList();

    /// <summary>
    /// Returns a list with a shallow copy of the given number of elements, starting from the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<DbToken> ToList(int index, int count) => Items.ToList(index, count);

    /// <summary>
    /// Trims the internal structures of this collection, without affecting to the immutability
    /// of the collection.
    /// </summary>
    public void Trim() => Items.Trim();

    // ----------------------------------------------------

    /// <summary>
    /// Reduces this instance to a simpler form, if possible. Otherwise, returns the original one.
    /// </summary>
    /// <returns></returns>
    public DbToken Reduce() => Count == 1 ? this[0] : this;

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of elements, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public DbTokenChain GetRange(int index, int count)
    {
        if (index == 0 && count == Count) return this;
        if (index == 0 && count == 0) return Clear();

        var temp = ToList(index, count);
        return new(temp);
    }

    /// <summary>
    /// Returns a new instance where the element at the given index has been replaced by the
    /// new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public DbTokenChain Replace(int index, DbToken item)
    {
        var builder = GetBuilder();
        var done = builder.Replace(index, item);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <summary>
    /// Returns a new instance where the given element has been added to the collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public DbTokenChain Add(DbToken item)
    {
        var builder = GetBuilder();
        var done = builder.Add(item);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public DbTokenChain AddRange(IEnumerable<DbToken> range)
    {
        var builder = GetBuilder();
        var done = builder.AddRange(range);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <summary>
    /// Returns a new instance where the given element has been inserted into the collection
    /// at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public DbTokenChain Insert(int index, DbToken item)
    {
        var builder = GetBuilder();
        var done = builder.Insert(index, item);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <summary>
    /// Returns a new instance where the elements from the given range have been inserted into
    /// the collection, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public DbTokenChain InsertRange(int index, IEnumerable<DbToken> range)
    {
        var builder = GetBuilder();
        var done = builder.InsertRange(index, range);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <summary>
    /// Returns a new instance where the original element at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public DbTokenChain RemoveAt(int index)
    {
        var builder = GetBuilder();
        var done = builder.RemoveAt(index);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <summary>
    /// Returns a new instance where the given number of elements, starting at the given index,
    /// have been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public DbTokenChain RemoveRange(int index, int count)
    {
        var builder = GetBuilder();
        var done = builder.RemoveRange(index, count);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <summary>
    /// Returns a new instance where the first ocurrence of the given element has been removed.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public DbTokenChain Remove(DbToken item)
    {
        var builder = GetBuilder();
        var done = builder.Remove(item);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <summary>
    /// Returns a new instance where the last ocurrence of the given element has been removed.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public DbTokenChain RemoveLast(DbToken item)
    {
        var builder = GetBuilder();
        var done = builder.RemoveLast(item);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <summary>
    /// Returns a new instance where all the ocurrences of the given element have been removed.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public DbTokenChain RemoveAll(DbToken item)
    {
        var builder = GetBuilder();
        var done = builder.RemoveAll(item);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public DbTokenChain Remove(Predicate<DbToken> predicate)
    {
        var builder = GetBuilder();
        var done = builder.Remove(predicate);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <summary>
    /// Returns a new instance where the last element that matches the given predicate has been
    /// removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public DbTokenChain RemoveLast(Predicate<DbToken> predicate)
    {
        var builder = GetBuilder();
        var done = builder.RemoveLast(predicate);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <summary>
    /// Returns a new instance where all the elements that match the given predicate have been
    /// removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public DbTokenChain RemoveAll(Predicate<DbToken> predicate)
    {
        var builder = GetBuilder();
        var done = builder.RemoveAll(predicate);
        return done > 0 ? builder.ToInstance() : this;
    }

    /// <summary>
    /// Returns a new instance where all the original elements have been removed.
    /// </summary>
    /// <returns></returns>
    public DbTokenChain Clear()
    {
        var builder = GetBuilder();
        var done = builder.Clear();
        return done > 0 ? builder.ToInstance() : this;
    }
}