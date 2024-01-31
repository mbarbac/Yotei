namespace Yotei.ORM.Internal.Code;

// ========================================================
/// <inheritdoc cref="IMetadataTag"/>
/// <typeparam name="T"></typeparam>
[Cloneable]
public sealed partial class MetadataTag<T> : IMetadataTag<T>
{
    readonly List<string> Items = [];

    /// <summary>
    /// Initializes a new instance with the given name.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="name"></param>
    public MetadataTag(bool caseSensitiveTags, string name)
    {
        CaseSensitiveTags = caseSensitiveTags;
        AddInternal(name);
    }

    /// <summary>
    /// Initializes a new instance with the names from the given range.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="range"></param>
    public MetadataTag(bool caseSensitiveTags, IEnumerable<string> range)
    {
        CaseSensitiveTags = caseSensitiveTags;
        AddRangeInternal(range);

        if (Count == 0) throw new ArgumentException(
            "Cannot initialize with an empty range of names.");
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    MetadataTag(MetadataTag<T> source)
    {
        if (source.HasValue) DefaultValue = source.DefaultValue;

        CaseSensitiveTags = source.CaseSensitiveTags;
        AddRangeInternal(source);
    }

    /// <inheritdoc/>
    public IEnumerator<string> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();
        if (Count == 1) sb.Append(DefaultName);
        else
        {
            sb.Append('(');
            sb.Append(string.Join(", ", Items));
            sb.Append(')');
        }
        if (HasValue) sb.Append($" = {DefaultValue.Sketch()}");
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool CaseSensitiveTags { get; }

    bool Compare(string x, string y) => string.Compare(x, y, !CaseSensitiveTags) == 0;

    /// <inheritdoc/>
    public T DefaultValue
    {
        get => _DefaultValue;
        init { _DefaultValue = value; HasValue = true; }
    }
    T _DefaultValue = default!;

    object? IMetadataTag.DefaultValue => DefaultValue;

    /// <inheritdoc/>
    public bool HasValue { get; private set; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public string DefaultName => Items[0];

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public bool Contains(string name)
    {
        name = name.NotNullNotEmpty();
        return Items.Find(x => Compare(x, name)) != null;
    }

    /// <inheritdoc/>
    public bool ContainsAny(IEnumerable<string> range)
    {
        range.ThrowWhenNull();
        foreach (var name in range) if (Contains(name)) return true;
        return false;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IMetadataTag<T> Replace(string oldname, string newname)
    {
        var clone = Clone();
        var num = clone.ReplaceInternal(oldname, newname);
        return num > 0 ? clone : this;
    }
    int ReplaceInternal(string oldname, string newname)
    {
        oldname = oldname.NotNullNotEmpty();
        newname = newname.NotNullNotEmpty();

        if (oldname == newname) return 0;

        var index = Items.FindIndex(x => Compare(x, oldname));
        if (index < 0) return 0;

        Items.RemoveAt(index);
        if (Contains(newname)) throw new DuplicateException("Duplicated name.").WithData(newname);

        Items.Insert(index, newname);
        return 1;
    }

    /// <inheritdoc/>
    public IMetadataTag<T> Add(string name)
    {
        var clone = Clone();
        var num = clone.AddInternal(name);
        return num > 0 ? clone : this;
    }
    int AddInternal(string name)
    {
        name = name.NotNullNotEmpty();

        if (Contains(name)) throw new DuplicateException("Duplicated name.").WithData(name);
        Items.Add(name);
        return 1;
    }

    /// <inheritdoc/>
    public IMetadataTag<T> AddRange(IEnumerable<string> range)
    {
        var clone = Clone();
        var num = clone.AddRangeInternal(range);
        return num > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection<string> trange && trange.Count == 0) return 0;
        if (range is ICollection irange && irange.Count == 0) return 0;

        var num = 0; foreach (var name in range)
        {
            var r = AddInternal(name);
            num += r;
        }
        return num;
    }

    /// <inheritdoc/>
    public IMetadataTag<T> Remove(string name)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(name);
        return num > 0 ? clone : this;
    }
    int RemoveInternal(string name)
    {
        name = name.NotNullNotEmpty();

        var index = Items.FindIndex(x => Compare(x, name));
        if (index < 0) return 0;

        if (Count <= 1) throw new InvalidOperationException(
            "Default name cannot be removed.")
            .WithData(name);

        Items.RemoveAt(index);
        return 1;
    }

    /// <inheritdoc/>
    public IMetadataTag<T> Clear()
    {
        var clone = Clone();
        var num = clone.ClearInternal();
        return num > 0 ? clone : this;
    }
    int ClearInternal()
    {
        if (Count > 1)
        {
            var count = Count;
            var item = Items[0];

            Items.Clear();
            Items.Add(item);
            return count - 1;
        }
        return 0;
    }

    // ----------------------------------------------------

    IMetadataTag IMetadataTag.Replace(string oldname, string newname) => Replace(oldname, newname);
    IMetadataTag IMetadataTag.Add(string name) => Add(name);
    IMetadataTag IMetadataTag.AddRange(IEnumerable<string> range) => AddRange(range);
    IMetadataTag IMetadataTag.Remove(string name) => Remove(name);
    IMetadataTag IMetadataTag.Clear() => Clear();
}