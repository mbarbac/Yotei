namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IKnownTags"/>
/// </summary>
[InheritsWith(ReturnType = typeof(IKnownTags))]
[Cloneable(ReturnType = typeof(IKnownTags))]
public partial class KnownTags : IKnownTags
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="ignoreCase"></param>
    /// <param name="identifierTags"></param>
    /// <param name="primaryKeyTag"></param>
    /// <param name="uniqueValuedTag"></param>
    /// <param name="readonlyTag"></param>
    public KnownTags(
        bool ignoreCase = false,
        ImmutableArray<IMetadataTag>? identifierTags = null,
        IMetadataTag? primaryKeyTag = null,
        IMetadataTag? uniqueValuedTag = null,
        IMetadataTag? readonlyTag = null)
    {
        IgnoreCase = ignoreCase;
        IdentifierTags = identifierTags;
        PrimaryKeyTag = primaryKeyTag;
        UniqueValuedTag = uniqueValuedTag;
        ReadOnlyTag = readonlyTag;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected KnownTags(KnownTags other)
    {
        ArgumentNullException.ThrowIfNull(other);

        IgnoreCase = other.IgnoreCase;
        IdentifierTags = other.IdentifierTags;
        PrimaryKeyTag = other.PrimaryKeyTag;
        UniqueValuedTag = other.UniqueValuedTag;
        ReadOnlyTag = other.ReadOnlyTag;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => _ToStringValue ??= GenerateToString();
    string? _ToStringValue;

    // Invoked once to cache the string representation.
    protected virtual string GenerateToString()
    {
        var sb = new StringBuilder();

        sb.Append(IdentifierTags == null || IdentifierTags.Value.Length == 0
            ? "-"
            : string.Join('.', IdentifierTags.Value.Select(x => x.Default)));

        if (PrimaryKeyTag != null) sb.Append($", Primary:{PrimaryKeyTag.Default}");
        if (UniqueValuedTag != null) sb.Append($", Unique:{UniqueValuedTag.Default}");
        if (ReadOnlyTag != null) sb.Append($", ReadOnly:{ReadOnlyTag.Default}");

        return sb.ToString();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator<IMetadataTag> GetEnumerator()
    {
        if (IdentifierTags != null) foreach (var tag in IdentifierTags.Value) yield return tag;
        if (PrimaryKeyTag != null) yield return PrimaryKeyTag;
        if (UniqueValuedTag != null) yield return UniqueValuedTag;
        if (ReadOnlyTag != null) yield return ReadOnlyTag;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(IKnownTags? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (IgnoreCase != other.IgnoreCase) return false;

        if (IdentifierTags == null && other.IdentifierTags != null) return false;
        if (IdentifierTags != null && other.IdentifierTags == null) return false;
        if (IdentifierTags != null &&
            other.IdentifierTags != null &&
            !IdentifierTags.Value.SequenceEqual(other.IdentifierTags.Value, new TagComparer()))
            return false;

        if (!PrimaryKeyTag.EqualsEx(other.PrimaryKeyTag)) return false;
        if (!UniqueValuedTag.EqualsEx(other.UniqueValuedTag)) return false;
        if (!ReadOnlyTag.EqualsEx(other.ReadOnlyTag)) return false;

        return true;
    }

    readonly struct TagComparer : IEqualityComparer<IMetadataTag>
    {
        public bool Equals(IMetadataTag? x, IMetadataTag? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;

            return x.Equals(y);
        }
        public int GetHashCode(IMetadataTag obj) => throw new NotImplementedException();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IKnownTags);

    public static bool operator ==(KnownTags? host, IKnownTags? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(KnownTags? host, IKnownTags? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, IgnoreCase);

        if (IdentifierTags != null)
            foreach (var tag in IdentifierTags) code = HashCode.Combine(code, tag);

        code = HashCode.Combine(code, PrimaryKeyTag);
        code = HashCode.Combine(code, UniqueValuedTag);
        code = HashCode.Combine(code, ReadOnlyTag);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IgnoreCase { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ImmutableArray<IMetadataTag>? IdentifierTags
    {
        get;
        init
        {
            if (value is null) { field = null; return; }
            if (value.Value.Length == 0) { field = null; return; }

            field = null; foreach (var tag in value)
            {
                if (IgnoreCase != tag.IgnoreCase) throw new ArgumentException(
                    "IgnoreCase value of the given tag is not the same as this instance's one.")
                    .WithData(tag)
                    .WithData(this);

                if (Contains(tag)) throw new DuplicateException(
                    "This instance already carries a name from the given tag.")
                    .WithData(value)
                    .WithData(this);
            }
            field = value;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IMetadataTag? PrimaryKeyTag
    {
        get;
        init
        {
            if (value is null) { field = null; return; }
            if (field?.Equals(value) ?? false) return;

            if (IgnoreCase != value.IgnoreCase) throw new ArgumentException(
                "IgnoreCase value of the given tag is not the same as this instance's one.")
                .WithData(value)
                .WithData(this);

            field = null;
            if (Contains(value)) throw new DuplicateException(
                "This instance already carries a name from the given tag.")
                .WithData(value)
                .WithData(this);

            field = value;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IMetadataTag? UniqueValuedTag
    {
        get;
        init
        {
            if (value is null) { field = null; return; }
            if (field?.Equals(value) ?? false) return;

            if (IgnoreCase != value.IgnoreCase) throw new ArgumentException(
                "IgnoreCase value of the given tag is not the same as this instance's one.")
                .WithData(value)
                .WithData(this);

            field = null;
            if (Contains(value)) throw new DuplicateException(
                "This instance already carries a name from the given tag.")
                .WithData(value)
                .WithData(this);

            field = value;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IMetadataTag? ReadOnlyTag
    {
        get;
        init
        {
            if (value is null) { field = null; return; }
            if (field?.Equals(value) ?? false) return;

            if (IgnoreCase != value.IgnoreCase) throw new ArgumentException(
                "IgnoreCase value of the given tag is not the same as this instance's one.")
                .WithData(value)
                .WithData(this);

            field = null;
            if (Contains(value)) throw new DuplicateException(
                "This instance already carries a name from the given tag.")
                .WithData(value)
                .WithData(this);

            field = value;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name) => Find(name) != null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public bool Contains(IEnumerable<string> names)
    {
        ArgumentNullException.ThrowIfNull(names);

        foreach (var name in names) if (Contains(name)) return true;
        return false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IMetadataTag? Find(string name)
    {
        name = name.NotNullNotEmpty(trim: true);

        if (IdentifierTags != null)
        {
            var tag = IdentifierTags.Value.FirstOrDefault(x => x.Contains(name));
            if (tag != null) return tag;
        }

        if (PrimaryKeyTag?.Contains(name) ?? false) return PrimaryKeyTag;
        if (UniqueValuedTag?.Contains(name) ?? false) return UniqueValuedTag;
        if (ReadOnlyTag?.Contains(name) ?? false) return ReadOnlyTag;

        return null;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public List<IMetadataTag> Find(IEnumerable<string> names)
    {
        ArgumentNullException.ThrowIfNull(names);

        List<IMetadataTag> items = []; foreach (var name in names)
        {
            var item = Find(name);
            if (item != null && !items.Contains(item)) items.Add(item);
        }
        return items;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<string> Names
    {
        get
        {
            if (IdentifierTags != null)
                foreach (var tag in IdentifierTags)
                    foreach (var name in tag) yield return name;

            if (PrimaryKeyTag != null) foreach (var name in PrimaryKeyTag) yield return name;
            if (UniqueValuedTag != null) foreach (var name in UniqueValuedTag) yield return name;
            if (ReadOnlyTag != null) foreach (var name in ReadOnlyTag) yield return name;
        }
    }
}