namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static partial class Test_CoreList_KT
{
    interface IElement { }

    // ----------------------------------------------------

    class Element(string name) : IElement
    {
        public string Name { get; set; } = name;
        public override string ToString() => Name ?? string.Empty;
    }
    static readonly Element xone = new("one");
    static readonly Element xtwo = new("two");
    static readonly Element xthree = new("three");
    static readonly Element xfour = new("four");
    static readonly Element xfive = new("five");

    // ----------------------------------------------------

    [Cloneable]
    partial class Chain : CoreList<string, IElement>, IElement
    {
        public Chain(bool sensitive) : base() => Sensitive = sensitive;
        public Chain(bool sensitive, int capacity) : this(sensitive) => Capacity = capacity;
        public Chain(bool sensitive, IEnumerable<IElement> range) : this(sensitive) => AddRange(range);
        protected Chain(Chain source) : this(source.Sensitive) => AddRange(source);

        public bool Sensitive { get; }
        public override IElement ValidateItem(IElement item) => item.ThrowWhenNull();
        public override string GetKey(IElement item) => item is Element named
            ? named.Name
            : throw new ArgumentException("Element is not a named instance.").WithData(item);
        public override string ValidateKey(string key) => key.NotNullNotEmpty();
        public override bool CompareKeys(string x, string y) => string.Compare(x, y, !Sensitive) == 0;
        public override bool CanInclude(IElement item, IElement source) => ReferenceEquals(item, source)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new Chain(false);
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        var items = new Chain(false, []);
        Assert.Empty(items);

        items = new Chain(false, [xone, xtwo, xthree]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        try { _ = new Chain(false, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain(false, [null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Chain(false, [new Element("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Duplicates()
    {
        var items = new Chain(false, [xone, xone]);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);

        items = new Chain(false, [xone, xtwo, xone]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xone, items[2]);

        try { _ = new Chain(false, [xone, new Element("ONE")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new Chain(true, [xone, new Element("one")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Extended()
    {
        var other = new Chain(false, [xtwo, xthree]);
        var items = new Chain(false, [xone, other]);

        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
    }

    // ----------------------------------------------------

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
    public static void Test_Find()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf("xfive"));

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
    public static void Test_Reverse()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xfour]);
        items.Reverse();

        Assert.Same(xfour, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xtwo, items[2]);
        Assert.Same(xone, items[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Sort()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xfour]);
        items.Sort(StringComparer.Ordinal);

        Assert.Same(xfour, items[0]);
        Assert.Same(xone, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xtwo, items[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xfour]);
        var list = items.GetRange(0, 0);
        Assert.Empty(list);

        list = items.GetRange(1, 2);
        Assert.Equal(2, list.Count);
        Assert.Same(xtwo, list[0]);
        Assert.Same(xthree, list[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var items = new Chain(false, [xone, xtwo, xthree]);
        var num = items.Replace(1, xtwo);
        Assert.Equal(0, num);

        num = items.Replace(1, xone);
        Assert.Equal(1, num);
        Assert.Same(xone, items[0]);
        Assert.Equal(xone, items[1]);
        Assert.Same(xthree, items[2]);

        try { items.Replace(1, new Element("one")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Extended()
    {
        var other = new Chain(false, [xfour, xfive]);
        var items = new Chain(false, [xone, xtwo, xthree]);
        
        var num = items.Replace(1, other);
        Assert.Equal(2, num);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Equal(xfour, items[1]);
        Assert.Same(xfive, items[2]);
        Assert.Same(xthree, items[3]);
    }

    /*
    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new Chain(false, [xone, xtwo]);

        var done = source.Add(xthree);
        Assert.Equal(1, done);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain(false, [xone, xtwo]);
        done = source.Add(xone);
        Assert.Equal(1, done);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(false, [xone, xtwo]);
        try { _ = source.Add(new Element("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        source = new Chain(false, [xone, xtwo]);
        try { _ = source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        source = new Chain(false, [xone, xtwo]);
        try { _ = source.Add(new Element("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new Chain(false, [xone, xtwo]);

        var done = source.AddRange([]);
        Assert.Equal(0, done);

        done = source.AddRange([xthree, xfour]);
        Assert.Equal(2, done);
        Assert.Equal(4, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xfour, source[3]);

        source = new Chain(false, [xone, xtwo]);
        done = source.AddRange([xone]);
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(false, [xone, xtwo]);
        try { _ = source.AddRange([new Element("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        source = new Chain(false, [xone, xtwo]);
        try { _ = source.AddRange([xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        source = new Chain(false, [xone, xtwo]);
        try { _ = source.AddRange([xfive, new Element("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new Chain(false, [xone, xtwo]);

        var done = source.Insert(2, xthree);
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain(false, [xone, xtwo]);
        done = source.Insert(2, xone);
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(false, [xone, xtwo]);
        try { _ = source.Insert(2, new Element("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        source = new Chain(false, [xone, xtwo]);
        try { _ = source.Insert(2, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        source = new Chain(false, [xone, xtwo]);
        try { _ = source.Insert(2, new Element("")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Insert(-1, xfive); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { source.Insert(3, xfive); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new Chain(false, [xone, xtwo]);

        var done = source.InsertRange(2, []);
        Assert.Equal(0, done);

        done = source.InsertRange(2, [xthree, xfour]);
        Assert.Equal(2, done);
        Assert.Equal(4, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xfour, source[3]);

        source = new Chain(false, [xone, xtwo]);
        done = source.InsertRange(2, [xone]);
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(false, [xone, xtwo]);
        try { _ = source.InsertRange(2, [new Element("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        source = new Chain(false, [xone, xtwo]);
        try { _ = source.InsertRange(2, [xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        source = new Chain(false, [xone, xtwo]);
        try { _ = source.InsertRange(2, [xfive, new Element("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

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
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var done = source.RemoveRange(0, 0);
        Assert.Equal(0, done);

        done = source.RemoveRange(0, 1);
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        done = source.RemoveRange(0, 4);
        Assert.Equal(4, done);
        Assert.Empty(source);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        try { _ = source.RemoveRange(0, -1); Assert.Fail(); }
        catch (ArgumentException) { }

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        try { _ = source.RemoveRange(-1, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        try { _ = source.RemoveRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        try { _ = source.RemoveRange(4, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        try { _ = source.RemoveRange(0, 5); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var done = source.Remove("four");
        Assert.Equal(0, done);

        done = source.Remove("one");
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        done = source.Remove("ONE");
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        done = source.RemoveLast("one");
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        done = source.RemoveLast("ONE");
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        done = source.RemoveAll("one");
        Assert.Equal(2, done);
        Assert.Equal(2, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        done = source.RemoveAll("ONE");
        Assert.Equal(2, done);
        Assert.Equal(2, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var done = source.Remove(x => ((Element)x).Name.Contains('z'));
        Assert.Equal(0, done);

        done = source.Remove(x => ((Element)x).Name.Contains('n'));
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        done = source.RemoveLast(x => ((Element)x).Name.Contains('n'));
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        done = source.RemoveAll(x => ((Element)x).Name.Contains('n'));
        Assert.Equal(2, done);
        Assert.Equal(2, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new Chain(false);

        var done = source.Clear();
        Assert.Equal(0, done);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        done = source.Clear();
        Assert.Equal(4, done);
        Assert.Empty(source);
    }*/
}