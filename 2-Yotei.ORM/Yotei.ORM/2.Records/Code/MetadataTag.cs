namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// Represents a tag by which a metadata entry is known, along with a set of secondary names that
/// can also be used to identify the same entry.
/// </summary>
[Cloneable]
public sealed partial class MetadataTag : FrozenList<string>
{
    protected override MetadataTagBuilder Items => _Items ??= new MetadataTagBuilder(CaseSensitiveNames);
    MetadataTagBuilder? _Items = null;

    void ValidateCount()
    {
        if (Count == 0) throw new InvalidOperationException(
            "A tag instance must have a primary name.");
    }

    /// <summary>
    /// Initializes a new instance with the given primary name.
    /// </summary>
    /// <param name="caseSensitiveNames"></param>
    /// <param name="name"></param>
    public MetadataTag(bool caseSensitiveNames, string name)
    {
        _CaseSensitiveNames = caseSensitiveNames;
        Items.Add(name);
    }

    /// <summary>
    /// Initializes a new instance with the names of the given not-empty range.
    /// </summary>
    /// <param name="caseSensitiveNames"></param>
    /// <param name="range"></param>
    public MetadataTag(bool caseSensitiveNames, IEnumerable<string> range)
    {
        _CaseSensitiveNames = caseSensitiveNames;
        Items.AddRange(range);
        ValidateCount();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    MetadataTag(MetadataTag source)
    {
        _CaseSensitiveNames = source.CaseSensitiveNames;
        Items.AddRange(source);
        ValidateCount();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() =>
        Count == 0 ? "<invalid>" :
        Count == 1 ? Items[0] :
        $"{Items[0]} ({string.Join(", ", Items.Skip(1))})";

    /// <summary>
    /// Determines if the names carried by this tag are case sensitive, or not.
    /// </summary>
    [WithGenerator]
    public bool CaseSensitiveNames
    {
        get => _CaseSensitiveNames;
        init
        {
            if (_CaseSensitiveNames == value) return;

            _CaseSensitiveNames = value;
            Items.CaseSensitiveNames = value;
        }
    }
    bool _CaseSensitiveNames;

    // ----------------------------------------------------

    public override MetadataTag GetRange(int index, int count)
    {
        var r = (MetadataTag)base.GetRange(index, count);
        r.ValidateCount();
        return r;
    }
    public override MetadataTag Replace(int index, string name)
    {
        var r = (MetadataTag)base.Replace(index, name);
        r.ValidateCount();
        return r;
    }
    public override MetadataTag Add(string name)
    {
        var r = (MetadataTag)base.Add(name);
        r.ValidateCount();
        return r;
    }
    public override MetadataTag AddRange(IEnumerable<string> range)
    {
        var r = (MetadataTag)base.AddRange(range);
        r.ValidateCount();
        return r;
    }
    public override MetadataTag Insert(int index, string name)
    {
        var r = (MetadataTag)base.Insert(index, name);
        r.ValidateCount();
        return r;
    }
    public override MetadataTag InsertRange(int index, IEnumerable<string> range)
    {
        var r = (MetadataTag)base.InsertRange(index, range);
        r.ValidateCount();
        return r;
    }
    public override MetadataTag RemoveAt(int index)
    {
        var r = (MetadataTag)base.RemoveAt(index);
        r.ValidateCount();
        return r;
    }
    public override MetadataTag RemoveRange(int index, int count)
    {
        var r = (MetadataTag)base.RemoveRange(index, count);
        r.ValidateCount();
        return r;
    }
    public override MetadataTag Remove(string name)
    {
        var r = (MetadataTag)base.Remove(name);
        r.ValidateCount();
        return r;
    }
    public override MetadataTag RemoveLast(string name)
    {
        var r = (MetadataTag)base.RemoveLast(name);
        r.ValidateCount();
        return r;
    }
    public override MetadataTag RemoveAll(string name)
    {
        var r = (MetadataTag)base.RemoveAll(name);
        r.ValidateCount();
        return r;
    }
    public override MetadataTag Remove(Predicate<string> predicate)
    {
        var r = (MetadataTag)base.Remove(predicate);
        r.ValidateCount();
        return r;
    }
    public override MetadataTag RemoveLast(Predicate<string> predicate)
    {
        var r = (MetadataTag)base.RemoveLast(predicate);
        r.ValidateCount();
        return r;
    }
    public override MetadataTag RemoveAll(Predicate<string> predicate)
    {
        var r = (MetadataTag)base.RemoveAll(predicate);
        r.ValidateCount();
        return r;
    }

    /// <summary>
    /// <inheritdoc/> This method keeps the primary name.
    /// </summary>
    /// <returns></returns>
    public override MetadataTag Clear()
    {
        if (Count == 0) ValidateCount();
        if (Count == 1) return this;

        var r = (MetadataTag)base.RemoveRange(1, Count - 1);
        ValidateCount();
        return r;
    }
}