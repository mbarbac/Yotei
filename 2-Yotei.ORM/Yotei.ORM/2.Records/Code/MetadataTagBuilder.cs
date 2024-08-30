namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IMetadataTagBuilder"/>
[Cloneable]
public partial class MetadataTagBuilder : IMetadataTagBuilder
{
    readonly List<string> Items = [];

    /// <summary>
    /// Initializes a new instance with the given default name.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="name"></param>
    public MetadataTagBuilder(bool sensitive, string name)
    {
        CaseSensitiveTags = sensitive;
        Add(name);        
    }

    /// <summary>
    /// Initializes a new instance with the names of the given range, using the first one as
    /// the default tag. If that range was an empty one, then an exception is thrown.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="range"></param>
    public MetadataTagBuilder(bool sensitive, IEnumerable<string> range)
    {
        CaseSensitiveTags = sensitive;
        AddRange(range);

        if (Items.Count == 0) throw new ArgumentException(
            "Range of tag names is empty.")
            .WithData(range);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected MetadataTagBuilder(MetadataTagBuilder source)
    {
        source.ThrowWhenNull();

        CaseSensitiveTags = source.CaseSensitiveTags;
        AddRange(source);
    }

    /// <inheritdoc/>
    public IEnumerator<string> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString()
    {
        if (Items.Count == 1) return Items[0];
        else
        {
            var sb = new StringBuilder();
            sb.Append(Items[0]);

            sb.Append(" ("); for (int i = 1; i < Items.Count; i++)
            {
                if (i > 1) sb.Append(", ");
                sb.Append(Items[i]);
            }
            sb.Append(')');
            return sb.ToString();
        }
    }

    /// <inheritdoc/>
    public IMetadataTag ToInstance() => new MetadataTag(CaseSensitiveTags, this);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool CaseSensitiveTags { get; }

    /// <inheritdoc/>
    public string Default
    {
        get => Items[0];
        set
        {
            var index = IndexOf(value = Validate(value));

            if (index == 0)
            {
                return;
            }
            else if (index > 0)
            {
                var temp = Items[index];

                Items.RemoveAt(index);
                Items.Insert(0, temp);
            }
            else throw new NotFoundException("Default tag not found.").WithData(value);
        }
    }

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public bool Contains(string name) => IndexOf(Validate(name)) >= 0;

    /// <inheritdoc/>
    public bool ContainsAny(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        foreach (var item in range) if (Contains(item)) return true;
        return false;
    }

    /// <inheritdoc/>
    [SuppressMessage("", "IDE0305")]
    public string[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<string> ToList() => new(Items);

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the given tag name.
    /// </summary>
    static string Validate(string name) => name.NotNullNotEmpty();

    /// <summary>
    /// Invoked to determine if the two given strings shall be considered the same, or not.
    /// </summary>
    bool Compare(string x, string y) => string.Compare(x, y, !CaseSensitiveTags) == 0;

    /// <summary>
    /// Returns the index of the given name, or -1 if not found.
    /// </summary>
    int IndexOf(string name)
    {
        for (int i = 0; i < Items.Count; i++) if (Compare(name, Items[i])) return i;
        return -1;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public int Replace(string name, string item)
    {
        var xname = Validate(name);
        var xitem = Validate(item);

        if (string.Compare(name, item) == 0) return 0;
        else
        {
            var index = IndexOf(xname);
            if (index >= 0)
            {
                Items.RemoveAt(index);
                Insert(index, xitem);
                return 1;
            }
            else return 0;
        }
    }

    int Insert(int index, string item)
    {
        item = Validate(item);

        var temp = IndexOf(item);
        if (temp >= 0) throw new DuplicateException("Item is duplicated.").WithData(item);

        Items.Insert(index, item);
        return 1;
    }

    /// <inheritdoc/>
    public int Add(string item)
    {
        item = Validate(item);

        var index = IndexOf(item);
        if (index >= 0) throw new DuplicateException("Item is duplicated.").WithData(item);

        Items.Add(item);
        return 1;
    }

    /// <inheritdoc/>
    public int AddRange(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range) num += Add(item);
        return num;
    }

    /// <inheritdoc/>
    public int Remove(string name)
    {
        name = Validate(name);

        var index = IndexOf(name);
        if (index >= 0)
        {
            if (Count == 1) throw new InvalidOperationException(
                "Cannot remove default tag.")
                .WithData(this);

            Items.RemoveAt(index);
        }

        return index >= 0 ? 1 : 0;
    }

    /// <inheritdoc/>
    public int Clear()
    {
        if (Count == 1) throw new InvalidOperationException(
            "Cannot remove default tag.")
                .WithData(this);

        var count = Count;

        var name = Items[0];
        Items.Clear();
        Items.Add(name);
        return count - 1;
    }
}