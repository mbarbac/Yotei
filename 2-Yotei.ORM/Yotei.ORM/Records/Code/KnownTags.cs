using System.Runtime.InteropServices.Marshalling;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IKnownTags"/>
/// </summary>
[Cloneable(PreventVirtual = true)]
[WithGenerator(PreventVirtual = true)]
public partial class KnownTags : IKnownTags
{
    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    public KnownTags(bool caseSensitiveTags)
    {
        CaseSensitiveTags = caseSensitiveTags;
        IdentifierTags = new IdentifierTags(caseSensitiveTags);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="identifierTags"></param>
    /// <param name="primaryKeyTag"></param>
    /// <param name="uniqueValuedTag"></param>
    /// <param name="readOnlyTag"></param>
    public KnownTags(
        bool caseSensitiveTags,
        IIdentifierTags identifierTags,
        string? primaryKeyTag = null,
        string? uniqueValuedTag = null,
        string? readOnlyTag = null)
    {
        CaseSensitiveTags = caseSensitiveTags;
        IdentifierTags = identifierTags;
        if (primaryKeyTag != null) PrimaryKeyTag = primaryKeyTag;
        if (uniqueValuedTag != null) UniqueValuedTag = uniqueValuedTag;
        if (readOnlyTag != null) ReadOnlyTag = readOnlyTag;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected KnownTags(KnownTags source)
    {
        source.ThrowWhenNull();

        _CaseSensitiveTags = source.CaseSensitiveTags;
        _IdentifierTags = source.IdentifierTags;
        _PrimaryKeyTag = source.PrimaryKeyTag;
        _UniqueValuedTag = source._UniqueValuedTag;
        _ReadOnlyTag = source._ReadOnlyTag;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<string> GetEnumerator()
    {
        foreach (var tag in IdentifierTags) yield return tag;
        if (PrimaryKeyTag != null) yield return PrimaryKeyTag;
        if (UniqueValuedTag != null) yield return UniqueValuedTag;
        if (ReadOnlyTag != null) yield return ReadOnlyTag;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (obj is not IKnownTags other) return false;

        if (CaseSensitiveTags != other.CaseSensitiveTags) return false;
        if (!IdentifierTags.Equals(other.IdentifierTags)) return false;
        if (!Compare(PrimaryKeyTag!, other.PrimaryKeyTag!)) return false;
        if (!Compare(UniqueValuedTag!, other.UniqueValuedTag!)) return false;
        if (!Compare(PrimaryKeyTag!, other.PrimaryKeyTag!)) return false;

        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = HashCode.Combine(CaseSensitiveTags);
        code = HashCode.Combine(code, IdentifierTags.GetHashCode());
        if (PrimaryKeyTag != null) code = HashCode.Combine(code, PrimaryKeyTag);
        if (UniqueValuedTag != null) code = HashCode.Combine(code, UniqueValuedTag);
        if (ReadOnlyTag != null) code = HashCode.Combine(code, ReadOnlyTag);
        return code;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        var id = string.Join('.', IdentifierTags);
        if (id.Length == 0) id = "-";
        sb.Append(id);
        if (PrimaryKeyTag != null) sb.Append($", PrimaryKey:{PrimaryKeyTag}");
        if (UniqueValuedTag != null) sb.Append($", UniqueValued:{UniqueValuedTag}");
        if (ReadOnlyTag != null) sb.Append($", ReadOnly:{ReadOnlyTag}");

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool CaseSensitiveTags
    {
        get => _CaseSensitiveTags;
        init
        {
            if (_CaseSensitiveTags == value) return;
            _CaseSensitiveTags = value;

            var old = Clone();
            ClearInternal();
            _IdentifierTags = new IdentifierTags(value, old.IdentifierTags);
            _PrimaryKeyTag = old.PrimaryKeyTag;
            _UniqueValuedTag = old.UniqueValuedTag;
            _ReadOnlyTag = old.ReadOnlyTag;
        }
    }
    bool _CaseSensitiveTags = Engine.CASESENSITIVETAGS;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IIdentifierTags IdentifierTags
    {
        get => _IdentifierTags;
        init
        {
            if (ReferenceEquals(_IdentifierTags, value)) return;

            value.ThrowWhenNull();
            if (value.CaseSensitiveTags != CaseSensitiveTags) throw new ArgumentException(
                "Case Sensitive Tags value mismtach.")
                .WithData(value)
                .WithData(this);

            foreach (var tag in value)
            {
                if (Compare(tag, PrimaryKeyTag!) ||
                    Compare(tag, UniqueValuedTag!) ||
                    Compare(tag, ReadOnlyTag!))
                    throw new DuplicateException(
                        "This instance already carries a tag from the given range.")
                        .WithData(tag)
                        .WithData(this);
            }

            _IdentifierTags = value;
        }
    }
    IIdentifierTags _IdentifierTags = default!;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? PrimaryKeyTag
    {
        get => _PrimaryKeyTag;
        init
        {
            value = value?.NotNullNotEmpty();

            if (value == null) _PrimaryKeyTag = null;
            else
            {
                if (IdentifierTags.Contains(value) ||
                    Compare(value, UniqueValuedTag!) ||
                    Compare(value, ReadOnlyTag!))
                    throw new DuplicateException(
                        "This instance already carries the given tag.")
                        .WithData(value)
                        .WithData(this);

                _PrimaryKeyTag = value;
            }
        }
    }
    string? _PrimaryKeyTag = null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? UniqueValuedTag
    {
        get => _UniqueValuedTag;
        init
        {
            value = value?.NotNullNotEmpty();

            if (value == null) _UniqueValuedTag = null;
            else
            {
                if (IdentifierTags.Contains(value) ||
                    Compare(value, PrimaryKeyTag!) ||
                    Compare(value, ReadOnlyTag!))
                    throw new DuplicateException(
                        "This instance already carries the given tag.")
                        .WithData(value)
                        .WithData(this);

                _UniqueValuedTag = value;
            }
        }
    }
    string? _UniqueValuedTag = null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? ReadOnlyTag
    {
        get => _ReadOnlyTag;
        init
        {
            value = value?.NotNullNotEmpty();

            if (value == null) _ReadOnlyTag = null;
            else
            {
                if (IdentifierTags.Contains(value) ||
                    Compare(value, PrimaryKeyTag!) ||
                    Compare(value, UniqueValuedTag!))
                    throw new DuplicateException(
                        "This instance already carries the given tag.")
                        .WithData(value)
                        .WithData(this);

                _ReadOnlyTag = value;
            }
        }
    }
    string? _ReadOnlyTag = null;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the two given tags shall be considered the same, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    bool Compare(string source, string target)
    {
        return string.Compare(source, target, !CaseSensitiveTags) == 0;
    }

    /// <summary>
    /// Determines if this instance is an empty one, or not.
    /// </summary>
    bool IsEmpty =>
        IdentifierTags.Count == 0 &&
        PrimaryKeyTag == null &&
        UniqueValuedTag == null &&
        ReadOnlyTag == null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool Contains(string tag)
    {
        tag = tag.NotNullNotEmpty();

        if (IdentifierTags.Contains(tag)) return true;
        if (PrimaryKeyTag != null && Compare(PrimaryKeyTag, tag)) return true;
        if (UniqueValuedTag != null && Compare(UniqueValuedTag, tag)) return true;
        if (ReadOnlyTag != null && Compare(ReadOnlyTag, tag)) return true;

        return false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IKnownTags Clear()
    {
        if (IsEmpty) return this;

        var temp = Clone();
        temp.ClearInternal();
        return temp;
    }
    protected virtual void ClearInternal()
    {
        _IdentifierTags = new IdentifierTags(CaseSensitiveTags);
        _PrimaryKeyTag = null;
        _UniqueValuedTag = null;
        _ReadOnlyTag = null;
    }
}