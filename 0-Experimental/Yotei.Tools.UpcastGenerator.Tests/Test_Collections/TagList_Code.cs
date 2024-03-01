#define GENERATE_

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
public partial interface ITagList
{
    bool CaseSensitive { get; }
}
#if !GENERATE
public partial interface ITagList : IFrozenList<K, T>
{
    // Interface imheriting an interface: upcast elements.
    // Note: the upcasted type may not have been emitted yet, if its contents are generated. So
    // we need to keep track of what types are generated, what contents generated for each, and
    // manage the order dependencies that may arise.

    new ITagList GetRange(int index, int count);
    new ITagList Replace(int index, T item);
    new ITagList Add(T item);
    new ITagList AddRange(IEnumerable<T> range);
    new ITagList Insert(int index, T item);
    new ITagList InsertRange(int index, IEnumerable<T> range);
    new ITagList RemoveAt(int index);
    new ITagList RemoveRange(int index, int count);
    new ITagList Remove(K key);
    new ITagList RemoveLast(K key);
    new ITagList RemoveAll(K key);
    new ITagList Remove(Predicate<T> predicate);
    new ITagList RemoveLast(Predicate<T> predicate);
    new ITagList RemoveAll(Predicate<T> predicate);
    new ITagList Clear();
}
#else
public partial interface ITagList : IUpcast<IFrozenList<K, T>> { }
#endif

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
public partial class TagList : FrozenList<K, T>, ITagList
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
#if !GENERATE
public partial class TagList : FrozenList<K, T>, ITagList
{
    // Core from core: upcast elements, returns base implementation.
    // Note: previous consideration for base types not been emitted yet.

    public override TagList GetRange(int index, int count) => (TagList)base.GetRange(index, count);
    public override TagList Replace(int index, T item) => (TagList)base.Replace(index, item);
    public override TagList Add(T item) => (TagList)base.Add(item);
    public override TagList AddRange(IEnumerable<T> range) => (TagList)base.AddRange(range);
    public override TagList Insert(int index, T item) => (TagList)base.Insert(index, item);
    public override TagList InsertRange(int index, IEnumerable<T> range) => (TagList)base.InsertRange(index, range);
    public override TagList RemoveAt(int index) => (TagList)base.RemoveAt(index);
    public override TagList RemoveRange(int index, int count) => (TagList)base.RemoveRange(index, count);
    public override TagList Remove(K key) => (TagList)base.Remove(key);
    public override TagList RemoveLast(K key) => (TagList)base.RemoveLast(key);
    public override TagList RemoveAll(K key) => (TagList)base.RemoveAll(key);
    public override TagList Remove(Predicate<T> predicate) => (TagList)base.Remove(predicate);
    public override TagList RemoveLast(Predicate<T> predicate) => (TagList)base.RemoveLast(predicate);
    public override TagList RemoveAll(Predicate<T> predicate) => (TagList)base.RemoveAll(predicate);
    public override TagList Clear() => (TagList)base.Clear();

    // Core from interface, but previous return type not matching (it was IFrozenList): use
    // explicit interface implementation, returning this one.
    // Note: previous consideration for base types not been emitted yet. In this case, ITagList
    // may not have been emitted, so the explicit implementation shall be based in the contents
    // recorded in its dedicated node.

    ITagList ITagList.GetRange(int index, int count) => GetRange(index, count);
    ITagList ITagList.Replace(int index, T item) => Replace(index, item);
    ITagList ITagList.Add(T item) => Add(item);
    ITagList ITagList.AddRange(IEnumerable<T> range) => AddRange(range);
    ITagList ITagList.Insert(int index, T item) => Insert(index, item);
    ITagList ITagList.InsertRange(int index, IEnumerable<T> range) => InsertRange(index, range);
    ITagList ITagList.RemoveAt(int index) => RemoveAt(index);
    ITagList ITagList.RemoveRange(int index, int count) => RemoveRange(index, count);
    ITagList ITagList.Remove(K key) => Remove(key);
    ITagList ITagList.RemoveLast(K key) => RemoveLast(key);
    ITagList ITagList.RemoveAll(K key) => RemoveAll(key);
    ITagList ITagList.Remove(Predicate<T> predicate) => Remove(predicate);
    ITagList ITagList.RemoveLast(Predicate<T> predicate) => RemoveLast(predicate);
    ITagList ITagList.RemoveAll(Predicate<T> predicate) => RemoveAll(predicate);
    ITagList ITagList.Clear() => Clear();
}
#else
public partial class TagList : IUpcast<FrozenList<K, T>>, IUpcast<ITagList> { }
#endif
