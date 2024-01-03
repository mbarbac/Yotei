using System.ComponentModel.Design;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// An immutable object that contains the metadata tags that are well-know to a given engine.
/// <br/> Duplicates are not allowed.
/// </summary>
[Cloneable]
public partial class KnownTags : IEnumerable<MetadataTag>
{
    /// <summary>
    /// Initializes a new empty instace.
    /// </summary>
    /// <param name="engine"></param>
    public KnownTags(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected KnownTags(KnownTags source)
    {
        Engine = source.Engine;
        _IdentifierTags = source.IdentifierTags;
        _PrimaryKeyTag = source._PrimaryKeyTag;
        _UniqueValuedTag = source._UniqueValuedTag;
        _ReadOnlyTag = source._ReadOnlyTag;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator<MetadataTag> GetEnumerator()
    {
        if (_IdentifierTags != null)
            foreach (var item in _IdentifierTags) yield return item;

        if (_PrimaryKeyTag != null) yield return _PrimaryKeyTag;
        if (_UniqueValuedTag != null) yield return _UniqueValuedTag;
        if (_ReadOnlyTag != null) yield return _ReadOnlyTag;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        var id = _IdentifierTags == null || _IdentifierTags.Count == 0
            ? "-"
            : string.Join('.', _IdentifierTags.Select(x => x[0]));

        sb.Append(id);
        if (_PrimaryKeyTag != null) sb.Append($", Primary:{_PrimaryKeyTag[0]}");
        if (_UniqueValuedTag != null) sb.Append($", Unique:{_UniqueValuedTag[0]}");
        if (_ReadOnlyTag != null) sb.Append($", ReadOnly:{_ReadOnlyTag[0]}");

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// The ordered collection of metadata tags that describes the maximal structure of the
    /// identifiers of the underlying engine, or null if this information is not available.
    /// </summary>
    [WithGenerator]
    public IdentifierTags? IdentifierTags
    {
        get => _IdentifierTags;
        init
        {
            if (value == null) _IdentifierTags = null;
            else
            {
                if (Engine != value.Engine) throw new ArgumentException(
                    "Engine of the given identifier tags is not the one of this instance.")
                    .WithData(value);

                var clone = Clone();
                clone._IdentifierTags = null;
                if (clone.ContainsAny(value.Names)) throw new DuplicateException(
                    "This instance already carries a name from the given range.")
                    .WithData(value);

                _IdentifierTags = value;
            }
        }
    }
    IdentifierTags? _IdentifierTags;

    /// <summary>
    /// The metadata tag that identifies if an associated entry is a primary key one, or not,
    /// or null if this information is not available.
    /// </summary>
    [WithGenerator]
    public MetadataTag? PrimaryKeyTag
    {
        get => _PrimaryKeyTag;
        init
        {
            if (value == null) _PrimaryKeyTag = null;
            else
            {
                if (Engine != value.Engine) throw new ArgumentException(
                    "Engine of the given identifier tags is not the one of this instance.")
                    .WithData(value);

                var clone = Clone();
                clone._PrimaryKeyTag = null;
                if (clone.ContainsAny(value)) throw new DuplicateException(
                    "This instance already contains a name from the given tag.")
                    .WithData(value);

                _PrimaryKeyTag = value;
            }
        }
    }
    MetadataTag? _PrimaryKeyTag;

    /// <summary>
    /// The metadata tag that identifies if an associated entry is a unique valued one, or not,
    /// or null if this information is not available.
    /// </summary>
    [WithGenerator]
    public MetadataTag? UniqueValuedTag
    {
        get => _UniqueValuedTag;
        init
        {
            if (value == null) _UniqueValuedTag = null;
            else
            {
                if (Engine != value.Engine) throw new ArgumentException(
                    "Engine of the given identifier tags is not the one of this instance.")
                    .WithData(value);

                var clone = Clone();
                clone._UniqueValuedTag = null;
                if (clone.ContainsAny(value)) throw new DuplicateException(
                    "This instance already contains a name from the given tag.")
                    .WithData(value);

                _UniqueValuedTag = value;
            }
        }
    }
    MetadataTag? _UniqueValuedTag;

    /// <summary>
    /// The metadata tag that identifies if an associated entry is a read only one, or not,
    /// or null if this information is not available.
    /// </summary>
    [WithGenerator]
    public MetadataTag? ReadOnlyTag
    {
        get => _ReadOnlyTag;
        init
        {
            if (value == null) _ReadOnlyTag = null;
            else
            {
                if (Engine != value.Engine) throw new ArgumentException(
                    "Engine of the given identifier tags is not the one of this instance.")
                    .WithData(value);

                var clone = Clone();
                clone._ReadOnlyTag = null;
                if (clone.ContainsAny(value)) throw new DuplicateException(
                    "This instance already contains a name from the given tag.")
                    .WithData(value);

                _ReadOnlyTag = value;
            }
        }
    }
    MetadataTag? _ReadOnlyTag;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance contains a metadata tag with the given name, or not.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual bool Contains(string name)
    {
        name = name.NotNullNotEmpty();

        if (IdentifierTags != null && IdentifierTags.Contains(name)) return true;
        if (PrimaryKeyTag != null && PrimaryKeyTag.Contains(name)) return true;
        if (UniqueValuedTag != null && UniqueValuedTag.Contains(name)) return true;
        if (ReadOnlyTag != null && ReadOnlyTag.Contains(name)) return true;

        return false;
    }

    /// <summary>
    /// Determines if this instance contains a metadata tag with any of the given names, or not.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual bool ContainsAny(IEnumerable<string> names)
    {
        names.ThrowWhenNull();

        if (IdentifierTags != null && IdentifierTags.ContainsAny(names)) return true;
        if (PrimaryKeyTag != null && PrimaryKeyTag.ContainsAny(names)) return true;
        if (UniqueValuedTag != null && UniqueValuedTag.ContainsAny(names)) return true;
        if (ReadOnlyTag != null && ReadOnlyTag.ContainsAny(names)) return true;

        return false;
    }

    /// <summary>
    /// Returns a new instance where all the metadata tags have been removed.
    /// </summary>
    /// <returns></returns>
    public virtual KnownTags Clear()
    {
        var clone = Clone();
        var num = clone.ClearInternal();
        return num ? clone : this;
    }
    protected virtual bool ClearInternal()
    {
        if (IdentifierTags == null &&
            PrimaryKeyTag == null &&
            UniqueValuedTag == null &&
            ReadOnlyTag == null)
            return false;

        _IdentifierTags = null;
        _PrimaryKeyTag = null;
        _UniqueValuedTag = null;
        _ReadOnlyTag = null;
        return true;
    }

}