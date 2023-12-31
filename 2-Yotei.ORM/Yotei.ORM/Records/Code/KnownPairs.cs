using TPair = System.Collections.Generic.KeyValuePair<string, object?>;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// An immutable object that contains the metadata pairs that are well-known to a given engine,
/// each containing its tag name and its default value.
/// <br/> Pairs with empty tag names are considered invalid and shall not be used.
/// <br/> Pairs with duplicated tag names are not allowed.
/// </summary>
[Cloneable]
public partial class KnownPairs : IEnumerable<TPair>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    [SuppressMessage("", "IDE0290")]
    public KnownPairs(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected KnownPairs(KnownPairs source) : this(source.Engine)
    {
        _IdentifierPairs = source._IdentifierPairs;
        _PrimaryKeyPair = source._PrimaryKeyPair;
        _ReadOnlyPair = source._ReadOnlyPair;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TPair> GetEnumerator()
    {
        if (IdentifierPairs != null) foreach (var id in IdentifierPairs) yield return new(id.Key, id.Value);
        if (PrimaryKeyPair != null) yield return new(PrimaryKeyPair.Value.Key, PrimaryKeyPair.Value.Value);
        if (ReadOnlyPair != null) yield return new(ReadOnlyPair.Value.Key, ReadOnlyPair.Value.Value);
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var str = IdentifierPairs != null && IdentifierPairs.Count > 0 ? IdentifierPairs.ToString() : "-";
        if (PrimaryKeyPair != null) str += $", Primary({PrimaryKeyPair.Value.Key}:{PrimaryKeyPair.Value.Value})";
        if (ReadOnlyPair != null) str += $", ReadOnly({ReadOnlyPair.Value.Key}:{ReadOnlyPair.Value.Value})";

        return str;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// The ordered collection of metadata that describes the maximal structure of the identifiers
    /// of the underlying database.If null, then it is assumed the underlying engine does not
    /// support his property,
    /// </summary>
    [WithGenerator]
    public IdentifierPairs? IdentifierPairs
    {
        get => _IdentifierPairs;
        init
        {
            if (value == null) _IdentifierPairs = null;
            else
            {
                if (!ReferenceEquals(Engine, value.Engine)) throw new ArgumentException(
                    "Engine of the identifier pairs is not the engine of this instance.")
                    .WithData(value);

                foreach (var (tag, _) in value)
                {
                    if ((PrimaryKeyPair != null && Compare(tag, PrimaryKeyPair.Value.Key) ||
                        (ReadOnlyPair != null && Compare(tag, ReadOnlyPair.Value.Key))))
                        throw new DuplicateException(
                            "Identifier tag is already used in this instance.")
                            .WithData(value);
                }

                _IdentifierPairs = value;
            }
        }
    }
    IdentifierPairs? _IdentifierPairs = null;

    /// <summary>
    /// The pair that identifies if a given entry is a primary key one or not. If null, then it
    /// is assumed the underlying engine does not support this property.
    /// </summary>
    [WithGenerator]
    public KeyValuePair<string, bool>? PrimaryKeyPair
    {
        get => _PrimaryKeyPair;
        init
        {
            if (value == null) _PrimaryKeyPair = null;
            else
            {
                var tag = value.Value.Key.NotNullNotEmpty();

                if ((IdentifierPairs != null && IdentifierPairs.Contains(tag))||
                    (ReadOnlyPair != null && Compare(tag, ReadOnlyPair.Value.Key)))
                    throw new DuplicateException(
                        "Primary Key tag is already used in this instance.")
                        .WithData(value);

                _PrimaryKeyPair = value;
            }
        }
    }
    KeyValuePair<string, bool>? _PrimaryKeyPair = null;

    /// <summary>
    /// The pair that identifies if a given entry is a read only one or not. If null, then it
    /// is assumed the underlying engine does not support this property.
    /// </summary>
    [WithGenerator]
    public KeyValuePair<string, bool>? ReadOnlyPair
    {
        get => _ReadOnlyPair;
        init
        {
            if (value == null) _ReadOnlyPair = null;
            else
            {
                var tag = value.Value.Key.NotNullNotEmpty();

                if ((IdentifierPairs != null && IdentifierPairs.Contains(tag)) ||
                    (PrimaryKeyPair != null && Compare(tag, PrimaryKeyPair.Value.Key)))
                    throw new DuplicateException(
                        "read Only tag is already used in this instance.")
                        .WithData(value);

                _ReadOnlyPair = value;
            }
        }
    }
    KeyValuePair<string, bool>? _ReadOnlyPair = null;

    // ----------------------------------------------------

    /// <summary>
    /// Detemines if the two given tags are equal or not.
    /// </summary>
    protected bool Compare(
        string? x, string? y) => string.Compare(x, y, !Engine.CaseSensitiveTags) == 0;

    /// <summary>
    /// Determines if this instance contains a pair with the given tag name, or not.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public virtual bool Contains(string tag)
    {
        tag = tag.NotNullNotEmpty();

        if (IdentifierPairs != null && IdentifierPairs.Contains(tag)) return true;
        if (PrimaryKeyPair != null && Compare(tag, PrimaryKeyPair.Value.Key)) return true;
        if (ReadOnlyPair != null && Compare(tag, ReadOnlyPair.Value.Key)) return true;

        return false;
    }

    /// <summary>
    /// Returns a new instance where all the pairs have been removed.
    /// </summary>
    /// <returns></returns>
    public virtual KnownPairs Clear()
    {
        var clone = Clone();
        var done = clone.ClearInternal();
        return done ? clone : this;
    }
    protected virtual bool ClearInternal()
    {
        if (_IdentifierPairs == null &&
            _PrimaryKeyPair == null &&
            _ReadOnlyPair == null)
            return false;

        _IdentifierPairs = null;
        _PrimaryKeyPair = null;
        _ReadOnlyPair = null;
        return true;
    }
}