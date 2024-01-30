using T = Yotei.ORM.Tests.Test_CoreList_T.Element;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_CoreList_T
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
    internal partial class Chain : CoreList<T>
    {
        public Chain(IEngine engine)
        {
            Engine = engine.ThrowWhenNull();
            Validate = (item) =>
            {
                item.ThrowWhenNull(); item.Name.NotNullNotEmpty();
                return item;
            };
            Compare = (x, y) => string.Compare(x.Name, y.Name, !Engine.CaseSensitiveNames) == 0;
            Duplicates = (@this, item) => @this.IndexesOf(item);
            CanInclude = (x, item) => ReferenceEquals(x, item)
                ? true
                : throw new DuplicateException("Duplicated element.").WithData(item);
        }
        public Chain(IEngine engine, T item) : this(engine) => Add(item);
        public Chain(IEngine engine, IEnumerable<T> range) : this(engine) => AddRange(range);
        public Chain(Chain source) : this(source.Engine) => AddRange(source);

        public IEngine Engine { get; }
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

        var done = source.Replace(0, xone);
        Assert.Equal(0, done);

        done = source.Replace(1, xone);
        Assert.Equal(1, done);
        Assert.Equal(4, source.Count);
        Assert.Equal(xone, source[0]);
        Assert.Same(xone, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xone, source[3]);

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        try { _ = source.Replace(1, new T("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        try { _ = source.Replace(1, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        try { _ = source.Replace(1, new T("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);

        var done = source.Add(xthree);
        Assert.Equal(1, done);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain(engine, [xone, xtwo]);
        done = source.Add(xone);
        Assert.Equal(1, done);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(engine, [xone, xtwo]);
        try { _ = source.Add(new T("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        source = new Chain(engine, [xone, xtwo]);
        try { _ = source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        source = new Chain(engine, [xone, xtwo]);
        try { _ = source.Add(new T("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);

        var done = source.AddRange([]);
        Assert.Equal(0, done);

        done = source.AddRange([xthree, xfour]);
        Assert.Equal(2, done);
        Assert.Equal(4, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xfour, source[3]);

        source = new Chain(engine, [xone, xtwo]);
        done = source.AddRange([xone]);
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(engine, [xone, xtwo]);
        try { _ = source.AddRange([new T("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        source = new Chain(engine, [xone, xtwo]);
        try { _ = source.AddRange([xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        source = new Chain(engine, [xone, xtwo]);
        try { _ = source.AddRange([xfive, new T("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);

        var done = source.Insert(2, xthree);
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain(engine, [xone, xtwo]);
        done = source.Insert(2, xone);
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(engine, [xone, xtwo]);
        try { _ = source.Insert(2, new T("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        source = new Chain(engine, [xone, xtwo]);
        try { _ = source.Insert(2, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        source = new Chain(engine, [xone, xtwo]);
        try { _ = source.Insert(2, new T("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);

        var done = source.InsertRange(2, []);
        Assert.Equal(0, done);

        done = source.InsertRange(2, [xthree, xfour]);
        Assert.Equal(2, done);
        Assert.Equal(4, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xfour, source[3]);

        source = new Chain(engine, [xone, xtwo]);
        done = source.InsertRange(2, [xone]);
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(engine, [xone, xtwo]);
        try { _ = source.InsertRange(2, [new T("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        source = new Chain(engine, [xone, xtwo]);
        try { _ = source.InsertRange(2, [xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        source = new Chain(engine, [xone, xtwo]);
        try { _ = source.InsertRange(2, [xfive, new T("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

        var done = source.RemoveAt(0);
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
        Assert.Same(xone, source[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

        var done = source.RemoveRange(0, 0);
        Assert.Equal(0, done);

        done = source.RemoveRange(0, 1);
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        done = source.RemoveRange(0, 4);
        Assert.Equal(4, done);
        Assert.Empty(source);

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        try { _ = source.RemoveRange(0, -1); Assert.Fail(); }
        catch (ArgumentException) { }

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        try { _ = source.RemoveRange(-1, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        try { _ = source.RemoveRange(4, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

        var done = source.Remove(xfour);
        Assert.Equal(0, done);

        done = source.Remove(xone);
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        done = source.Remove(new T("ONE"));
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        done = source.RemoveLast(xone);
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        done = source.RemoveLast(new T("ONE"));
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        done = source.RemoveAll(xone);
        Assert.Equal(2, done);
        Assert.Equal(2, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        done = source.RemoveAll(new T("ONE"));
        Assert.Equal(2, done);
        Assert.Equal(2, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

        var done = source.Remove(x => x.Name.Contains('z'));
        Assert.Equal(0, done);

        done = source.Remove(x => x.Name.Contains('n'));
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        done = source.RemoveLast(x => x.Name.Contains('n'));
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        done = source.RemoveAll(x => x.Name.Contains('n'));
        Assert.Equal(2, done);
        Assert.Equal(2, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine);

        var done = source.Clear();
        Assert.Equal(0, done);

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        done = source.Clear();
        Assert.Equal(4, done);
        Assert.Empty(source);
    }
}