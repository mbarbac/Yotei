using T = Yotei.Tools.UpcastGenerator.Tests_Collections.Element;
using K = string;

namespace Yotei.Tools.UpcastGenerator.Tests_Collections;

// ========================================================
public class Element(string name)
{
    public override string ToString() => Name;
    public string Name { get; } = name.NotNullNotEmpty();
}

// ========================================================
[Cloneable]
public partial interface ITagList : IUpcast<IFrozenList<K, T>>
{
    bool CaseSensitive { get; }
}

// ========================================================
[Cloneable]
public partial class TagListBuilder : CoreList<K, T>
{
    public TagListBuilder(bool sensitive)
    {
        CaseSensitive = sensitive;
        ValidateItem = (item) => item.ThrowWhenNull();
        GetKey = (item) => item.Name;
        ValidateKey = (key) => key.NotNullNotEmpty();
        CompareKeys = (x, y) => string.Compare(x, y, !CaseSensitive) == 0;
        GetDuplicates = (item) => IndexesOf(item);
        CanInclude = (item, x) => ReferenceEquals(item, x)
            ? true
            : throw new DuplicateException("Duplicated item").WithData(item); ;
    }
    public TagListBuilder(bool sensitive, T item) : this(sensitive) => Add(item);
    public TagListBuilder(bool sensitive, IEnumerable<T> range) : this(sensitive) => AddRange(range);
    TagListBuilder(TagListBuilder source) : this(source.CaseSensitive) => AddRange(source);

    public bool CaseSensitive { get; }
}

// ========================================================
[Cloneable]
public partial class TagList : IUpcast<FrozenList<K, T>>, IUpcast<ITagList>
{
    protected override TagListBuilder Items => _Items ??= new(CaseSensitive);
    TagListBuilder? _Items;

    [SuppressMessage("", "IDE0290")]
    public TagList(bool sensitive) => CaseSensitive = sensitive;
    public TagList(bool sensitive, T item) : this(sensitive) => Items.Add(item);
    public TagList(bool sensitive, IEnumerable<T> range) : this(sensitive) => Items.AddRange(range);
    TagList(TagList source) : this(source.CaseSensitive) => Items.AddRange(source);

    public bool CaseSensitive { get; }
}