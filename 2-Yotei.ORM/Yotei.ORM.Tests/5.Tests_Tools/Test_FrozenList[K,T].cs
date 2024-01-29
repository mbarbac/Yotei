using T = Yotei.ORM.Tests.Test_FrozenList_KT.Element;
using K = string;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_FrozenList_KT
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
    internal partial class Chain : FrozenList<K, T>
    {
        [Cloneable]
        protected partial class InnerItems : CoreList<K, T>
        {
            readonly Chain Master;
            public InnerItems(Chain master)
            {
                Master = master.ThrowWhenNull();
                ValidateItem = (item) =>
                {
                    item.ThrowWhenNull(); ValidateKey(item.Name);
                    return item;
                };
                GetKey = (item) => item?.Name ?? throw new ArgumentNullException(nameof(item));
                ValidateKey = (key) => key.NotNullNotEmpty();
                CompareKeys = (x, y) => string.Compare(x, y, !Master.Engine.CaseSensitiveNames) == 0;
                Duplicates = (@this, key) => @this.IndexesOf(key);
                CanInclude = (x, item) => ReferenceEquals(x, item)
                    ? true
                    : throw new DuplicateException("Duplicated element.").WithData(item);
            }
            public InnerItems(InnerItems source) : this(source.Master) => AddRange(source);
        }
        protected override InnerItems Items => _Items ??= new(this);
        InnerItems? _Items = null;

        public Chain(IEngine engine) => Engine = engine.ThrowWhenNull();
        public Chain(IEngine engine, T item) : this(engine) => Items.Add(item);
        public Chain(IEngine engine, IEnumerable<T> range) : this(engine) => Items.AddRange(range);
        public Chain(Chain source) : this(source.Engine) => Items.AddRange(source);

        public IEngine Engine { get; }

        public override Chain GetRange(int index, int count) => (Chain)base.GetRange(index, count);
        public override Chain Replace(int index, T item) => (Chain)base.Replace(index, item);
        public override Chain Add(T item) => (Chain)base.Add(item);
        public override Chain AddRange(IEnumerable<T> range) => (Chain)base.AddRange(range);
        public override Chain Insert(int index, T item) => (Chain)base.Insert(index, item);
        public override Chain InsertRange(int index, IEnumerable<T> range) => (Chain)base.InsertRange(index, range);
        public override Chain RemoveAt(int index) => (Chain)base.RemoveAt(index);
        public override Chain RemoveRange(int index, int count) => (Chain)base.RemoveRange(index, count);
        public override Chain Remove(K key) => (Chain)base.Remove(key);
        public override Chain RemoveLast(K key) => (Chain)base.RemoveLast(key);
        public override Chain RemoveAll(K key) => (Chain)base.RemoveAll(key);
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
        var engine = new FakeEngine();
        var items = new Chain(engine);
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var engine = new FakeEngine();
        var items = new Chain(engine, xone);
        Assert.Single(items);
        Assert.Same(xone, items[0]);

        try { _ = new Chain(engine, (T)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain(engine, new T("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var engine = new FakeEngine();
        var items = new Chain(engine, []);
        Assert.Empty(items);

        items = new Chain(engine, [xone, xtwo, xthree]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        try { _ = new Chain(engine, (IEnumerable<T>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain(engine, [xone, null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Chain(engine, [xone, new T("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }
    //[Enforced]
    [Fact]
    public static void Test_Create_Many_With_Duplicates()
    {
        var engine = new FakeEngine();
        var items = new Chain(engine, [xone, xone]);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);

        try { _ = new Chain(engine, [xone, new T("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new Chain(engine, [xone, new T("ONE")]); Assert.Fail(); }
        catch (DuplicateException) { }

        engine = new FakeEngine() { CaseSensitiveNames = true };
        items = new Chain(engine, [xone, new T("ONE")]);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Equal("ONE", items[1].Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var items = new Chain(engine, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf("five"));

        Assert.Equal(0, items.IndexOf("one"));
        Assert.Equal(0, items.IndexOf("ONE"));

        Assert.Equal(3, items.LastIndexOf("one"));
        Assert.Equal(3, items.LastIndexOf("ONE"));

        var list = items.IndexesOf("one");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        list = items.IndexesOf("ONE");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var engine = new FakeEngine();
        var items = new Chain(engine, [xone, xtwo, xthree, xone]);

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
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

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
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

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
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);

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
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);

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
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);

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
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);

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
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

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
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

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
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

        var target = source.Remove("four");
        Assert.Same(source, target);

        target = source.Remove("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveLast("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

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
        var engine = new FakeEngine();
        var source = new Chain(engine);

        var target = source.Clear();
        Assert.Same(source, target);

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}