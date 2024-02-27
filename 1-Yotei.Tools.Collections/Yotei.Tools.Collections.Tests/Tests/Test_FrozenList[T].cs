/*using T = Yotei.Tools.Tests.FrozenList_T.Test_FrozenList_T.Element;

namespace Yotei.Tools.Tests.FrozenList_T;

// ========================================================
//[Enforced]
public static partial class Test_FrozenList_T
{
    internal class Element(string name)
    {
        public string Name { get; set; } = name;
        public override string ToString() => Name ?? string.Empty;
    }
    static readonly T xone = new("one");
    static readonly T xtwo = new("two");
    static readonly T xthree = new("three");
    static readonly T xfour = new("four");
    static readonly T xfive = new("five");

    // ====================================================

    [Cloneable]
    internal partial class ChainBuilder : CoreList<T>
    {
        readonly Chain Master;
        public ChainBuilder(Chain master)
        {
            Master = master.ThrowWhenNull();
            Validate = (item) =>
            {
                item.ThrowWhenNull(); item.Name.NotNullNotEmpty();
                return item;
            };
            Compare = (x, y) => string.Compare(x.Name, y.Name, !Master.CaseSensitive) == 0;
            GetDuplicates = IndexesOf;
            CanInclude = (item, x) => ReferenceEquals(item, x)
                ? true
                : throw new DuplicateException("Duplicated element.").WithData(item);
        }
        protected ChainBuilder(ChainBuilder source) : this(source.Master) => AddRange(source);
    }

    // ----------------------------------------------------

    [Cloneable]
    internal partial class Chain : FrozenList<T>
    {
        protected override ChainBuilder Items => _Items ??= new(this);
        ChainBuilder? _Items = null;

        public Chain(bool caseSensitive) => CaseSensitive = caseSensitive;
        public Chain(bool caseSensitive, T item) : this(caseSensitive) => Items.Add(item);
        public Chain(bool caseSensitive, IEnumerable<T> range) : this(caseSensitive) => Items.AddRange(range);
        public Chain(Chain source) : this(source.CaseSensitive) => Items.AddRange(source);

        public bool CaseSensitive
        {
            get => _CaseSensitive;
            init
            {
                if (_CaseSensitive = value) return;
                _CaseSensitive = value;

                var range = Items.ToArray();
                Items.Clear();
                Items.AddRange(range);
            }
        }
        bool _CaseSensitive;

        public override Chain GetRange(int index, int count) => (Chain)base.GetRange(index, count);
        public override Chain Replace(int index, T item) => (Chain)base.Replace(index, item);
        public override Chain Add(T item) => (Chain)base.Add(item);
        public override Chain AddRange(IEnumerable<T> range) => (Chain)base.AddRange(range);
        public override Chain Insert(int index, T item) => (Chain)base.Insert(index, item);
        public override Chain InsertRange(int index, IEnumerable<T> range) => (Chain)base.InsertRange(index, range);
        public override Chain RemoveAt(int index) => (Chain)base.RemoveAt(index);
        public override Chain RemoveRange(int index, int count) => (Chain)base.RemoveRange(index, count);
        public override Chain Remove(T item) => (Chain)base.Remove(item);
        public override Chain RemoveLast(T item) => (Chain)base.RemoveLast(item);
        public override Chain RemoveAll(T item) => (Chain)base.RemoveAll(item);
        public override Chain Remove(Predicate<T> predicate) => (Chain)base.Remove(predicate);
        public override Chain RemoveLast(Predicate<T> predicate) => (Chain)base.RemoveLast(predicate);
        public override Chain RemoveAll(Predicate<T> predicate) => (Chain)base.RemoveAll(predicate);
        public override Chain Clear() => (Chain)base.Clear();
    }

    // ====================================================

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new Chain(false);
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var items = new Chain(false, xone);
        Assert.Single(items);
        Assert.Same(xone, items[0]);

        try { _ = new Chain(false, (T)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain(false, new T("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var items = new Chain(false, []);
        Assert.Empty(items);

        items = new Chain(false, [xone, xtwo, xthree]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        try { _ = new Chain(false, (IEnumerable<T>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain(false, [xone, null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Chain(false, [xone, new T("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many_With_Duplicates()
    {
        var items = new Chain(false, [xone, xone]);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);

        try { _ = new Chain(false, [xone, new T("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new Chain(false, [xone, new T("ONE")]); Assert.Fail(); }
        catch (DuplicateException) { }

        items = new Chain(true, [xone, new T("ONE")]);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Equal("ONE", items[1].Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf(xfive));

        Assert.Equal(0, items.IndexOf(xone));
        Assert.Equal(0, items.IndexOf(new T("ONE")));

        Assert.Equal(3, items.LastIndexOf(xone));
        Assert.Equal(3, items.LastIndexOf(new T("ONE")));

        var list = items.IndexesOf(xone);
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        list = items.IndexesOf(new T("ONE"));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf(x => x.Name.Contains('z')));

        Assert.Equal(0, items.IndexOf(x => x.Name.Contains('n')));
        Assert.Equal(3, items.LastIndexOf(x => x.Name.Contains('n')));

        var list = items.IndexesOf(x => x.Name.Contains('n'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        target = source.Replace(1, xone);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal(xone, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);

        try { _ = source.Replace(1, new T("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.Replace(1, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.Replace(1, new T("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new Chain(false, [xone, xtwo]);

        var target = source.Add(xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.Add(xone);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);

        try { _ = source.Add(new T("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.Add(new T("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new Chain(false, [xone, xtwo]);

        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange([xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        target = source.AddRange([xone]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);

        try { _ = source.AddRange([new T("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.AddRange([xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.AddRange([xfive, new T("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new Chain(false, [xone, xtwo]);

        var target = source.Insert(2, xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.Insert(2, xone);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);

        try { _ = source.Insert(2, new T("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.Insert(2, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.Insert(2, new T("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new Chain(false, [xone, xtwo]);

        var target = source.InsertRange(2, []);
        Assert.Same(source, target);

        target = source.InsertRange(2, [xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        target = source.InsertRange(2, [xone]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);

        try { _ = source.InsertRange(2, [new T("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.InsertRange(2, [xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.InsertRange(2, [xfive, new T("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 1);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveRange(0, 4);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        try { _ = source.RemoveRange(0, -1); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = source.RemoveRange(-1, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { _ = source.RemoveRange(4, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var target = source.Remove(xfour);
        Assert.Same(source, target);

        target = source.Remove(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.Remove(new T("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveLast(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveLast(new T("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveAll(new T("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var target = source.Remove(x => x.Name.Contains('z'));
        Assert.Same(source, target);

        target = source.Remove(x => x.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveLast(x => x.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(x => x.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new Chain(false);

        var target = source.Clear();
        Assert.Same(source, target);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}*/