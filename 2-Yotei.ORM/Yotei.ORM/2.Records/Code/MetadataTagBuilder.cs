namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// Represents a builder of <see cref="MetadataTag"/> instances.
/// </summary>
[Cloneable]
public sealed partial class MetadataTagBuilder : CoreList<string>
{
    protected override string ValidateItem(string name, bool add) => name.NotNullNotEmpty();
    protected override bool CompareItems(string source, string name) => string.Compare(source, name, !CaseSensitiveNames) == 0;
    protected override bool SameItem(string source, string name) => source == name;
    protected override List<int> GetDuplicates(string name) => base.GetDuplicates(name);
    protected override bool AcceptDuplicate(string source, string name)
        => throw new DuplicateException("Duplicated tag name.").WithData(name);

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// <br/> Note that empty instances are just temporary working ones.
    /// </summary>
    /// <param name="caseSensitiveNames"></param>
    public MetadataTagBuilder(bool caseSensitiveNames) => CaseSensitiveNames = caseSensitiveNames;

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="caseSensitiveNames"></param>
    /// <param name="name"></param>
    public MetadataTagBuilder(bool caseSensitiveNames, string name)
        : this (caseSensitiveNames)
        => Add(name);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="caseSensitiveNames"></param>
    /// <param name="range"></param>
    public MetadataTagBuilder(bool caseSensitiveNames, IEnumerable<string> range)
        : this(caseSensitiveNames)
        => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    MetadataTagBuilder(MetadataTagBuilder source)
        : this(source.CaseSensitiveNames)
        => AddRange(source);

    /// <summary>
    /// Determines if the names carried by this tag are case sensitive, or not.
    /// </summary>
    public bool CaseSensitiveNames
    {
        get => _CaseSensitiveNames;
        set
        {
            if (_CaseSensitiveNames == value) return;

            _CaseSensitiveNames = value;
            var range = ToArray();
            Clear();
            AddRange(range);
        }
    }
    bool _CaseSensitiveNames;
}