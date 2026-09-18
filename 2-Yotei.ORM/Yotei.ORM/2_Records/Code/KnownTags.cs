namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IKnownTags"/>
/// </summary>
[Cloneable(ReturnType = typeof(IKnownTags))]
[InheritsWith(ReturnType = typeof(IKnownTags))]
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
        bool ignoreCase,
        ImmutableArray<IMetadataTag>? identifierTags = null,
        IMetadataTag? primaryKeyTag = null,
        IMetadataTag? uniqueValuedTag = null,
        IMetadataTag? readonlyTag = null)
    {
        IgnoreCase = ignoreCase;
        if (identifierTags != null) IdentifierTags = identifierTags;
        if (primaryKeyTag != null) PrimaryKeyTag = primaryKeyTag;
        if (uniqueValuedTag != null) UniqueValuedTag = uniqueValuedTag;
        if (readonlyTag != null) ReadOnlyTag = readonlyTag;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected KnownTags(KnownTags other)
    {
        ArgumentNullException.ThrowIfNull(other);

        IgnoreCase = other.IgnoreCase;
        if (other.IdentifierTags != null) IdentifierTags = other.IdentifierTags;
        if (other.PrimaryKeyTag != null) PrimaryKeyTag = other.PrimaryKeyTag;
        if (other.UniqueValuedTag != null) UniqueValuedTag = other.UniqueValuedTag;
        if (other.ReadOnlyTag != null) ReadOnlyTag = other.ReadOnlyTag;
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<string> Names
    {
        get
        {
            var iter = GetEnumerator();
            while (iter.MoveNext())
            {
                var tag = iter.Current;
                foreach (var name in tag) yield return name;
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        if (IdentifierTags == null) sb.Append('-');
        else sb.Append(string.Join(", ", IdentifierTags.Value.Select(x => x.Default)));

        if (PrimaryKeyTag != null) sb.Append($", Primary:{PrimaryKeyTag.Default}");
        if (UniqueValuedTag != null) sb.Append($", UniqueValued:{UniqueValuedTag.Default}");
        if (ReadOnlyTag != null) sb.Append($", ReadOnly:{ReadOnlyTag.Default}");

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Compares the two arrays of tags.
    /// </summary>
    static bool Compare(ImmutableArray<IMetadataTag> source, ImmutableArray<IMetadataTag> target)
    {
        if (source.Length != target.Length) return false;

        for (int i = 0; i < source.Length; i++)
        {
            var xsource = source[i];
            var xtarget = target[i];
            if (!xsource.Equals(xtarget)) return false;
        }

        return true;
    }

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
        if (IdentifierTags.HasValue && other.IdentifierTags.HasValue &&
            !Compare(IdentifierTags!.Value, other.IdentifierTags!.Value)) return false;

        if (PrimaryKeyTag == null && other.PrimaryKeyTag != null) return false;
        if (PrimaryKeyTag != null && other.PrimaryKeyTag == null) return false;
        if (PrimaryKeyTag != null && other.PrimaryKeyTag != null &&
            !PrimaryKeyTag!.Equals(other.PrimaryKeyTag)) return false;

        if (UniqueValuedTag == null && other.UniqueValuedTag != null) return false;
        if (UniqueValuedTag != null && other.UniqueValuedTag == null) return false;
        if (UniqueValuedTag != null && other.UniqueValuedTag != null &&
            !UniqueValuedTag!.Equals(other.UniqueValuedTag)) return false;

        if (ReadOnlyTag == null && other.ReadOnlyTag != null) return false;
        if (ReadOnlyTag != null && other.ReadOnlyTag == null) return false;
        if (ReadOnlyTag != null && other.ReadOnlyTag != null &&
            !ReadOnlyTag!.Equals(other.ReadOnlyTag)) return false;

        return true;
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
        var code = IgnoreCase.GetHashCode();

        if (IdentifierTags.HasValue)
            foreach (var tag in IdentifierTags.Value) code = HashCode.Combine(code, tag);

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
            if (field.HasValue && Compare(field.Value, value.Value)) return;

            field = null; foreach (var tag in value)
            {
                ArgumentNullException.ThrowIfNull(tag);

                if (IgnoreCase != tag.IgnoreCase) throw new ArgumentException(
                    "IgnoreCase value of the given tag is not the same as this instance's one")
                    .WithData(tag);

                if (this.ContainsAny(tag)) throw new DuplicateException(
                    "This instance already carries a name from the given tag.")
                    .WithData(tag)
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
            if (field != null && field.Equals(value)) return;

            if (IgnoreCase != value.IgnoreCase) throw new ArgumentException(
                "IgnoreCase value of the given tag is not the same as this instance's one")
                .WithData(value);

            field = null;
            if (this.ContainsAny(value)) throw new DuplicateException(
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
            if (field != null && field.Equals(value)) return;

            if (IgnoreCase != value.IgnoreCase) throw new ArgumentException(
                "IgnoreCase value of the given tag is not the same as this instance's one")
                .WithData(value);

            field = null;
            if (this.ContainsAny(value)) throw new DuplicateException(
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
            if (field != null && field.Equals(value)) return;

            if (IgnoreCase != value.IgnoreCase) throw new ArgumentException(
                "IgnoreCase value of the given tag is not the same as this instance's one")
                .WithData(value);

            field = null;
            if (this.ContainsAny(value)) throw new DuplicateException(
                "This instance already carries a name from the given tag.")
                .WithData(value)
                .WithData(this);

            field = value;
        }
    }
}