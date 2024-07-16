namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_CoreList_KT
{
    internal class Element(string name)
    {
        public string Name { get; set; } = name;
        public override string ToString() => Name ?? string.Empty;
    }
    static readonly Element xone = new("one");
    static readonly Element xtwo = new("two");
    static readonly Element xthree = new("three");
    static readonly Element xfour = new("four");
    static readonly Element xfive = new("five");

    // ====================================================

    [Cloneable]
    internal partial class Chain : CoreList<string, Element>
    {
        public Chain(bool sensitive)
        {
            Sensitive = sensitive;
            ValidateItem = (x) => x.ThrowWhenNull();
            GetKey = (x) => x.ThrowWhenNull().Name;
            ValidateKey = (x) => x.NotNullNotEmpty();
            CompareKeys = (x, y) => string.Compare(x.NotNullNotEmpty(), y.NotNullNotEmpty(), !Sensitive) == 0;
            GetDuplicates = IndexesOf;
            CanInclude = (x, y) => ReferenceEquals(x, y)
                ? true
                : throw new DuplicateException("Duplicated element.").WithData(x);
        }
        public Chain(bool sensitive, Element item) : this(sensitive) => Add(item);
        public Chain(bool sensitive, IEnumerable<Element> range) : this(sensitive) => AddRange(range);
        protected Chain(Chain source) : this(source.Sensitive) => AddRange(source);

        public bool Sensitive
        {
            get => _Sensitive;
            set
            {
                if (_Sensitive == value) return;
                _Sensitive = value;
                Reload();
            }
        }
        bool _Sensitive;
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

        try { _ = new Chain(false, (Element)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain(false, new Element("")); Assert.Fail(); }
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

        try { _ = new Chain(false, (IEnumerable<Element>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain(false, [xone, null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Chain(false, [xone, new Element("")]); Assert.Fail(); }
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

        try { _ = new Chain(false, [xone, new Element("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new Chain(false, [xone, new Element("ONE")]); Assert.Fail(); }
        catch (DuplicateException) { }

        items = new Chain(true, [xone, new Element("ONE")]);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Equal("ONE", items[1].Name);
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
    public static void Test_Reduce()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var done = source.Reduce(0, 4);
        Assert.Equal(0, done);

        done = source.Reduce(0, 0);
        Assert.Equal(4, done);
        Assert.Empty(source);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        done = source.Reduce(1, 2);
        Assert.Equal(2, done);
        Assert.Equal(2, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);

        try { source.Reduce(-1, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { source.Reduce(5, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { source.Reduce(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { source.Reduce(0, 5); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var done = source.Replace(0, xone);
        Assert.Equal(0, done);

        done = source.Replace(1, xone);
        Assert.Equal(1, done);
        Assert.Equal(4, source.Count);
        Assert.Equal(xone, source[0]);
        Assert.Same(xone, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xone, source[3]);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        try { _ = source.Replace(1, new Element("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        try { _ = source.Replace(1, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        try { _ = source.Replace(1, new Element("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

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

        var done = source.Remove(x => x.Name.Contains('z'));
        Assert.Equal(0, done);

        done = source.Remove(x => x.Name.Contains('n'));
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        done = source.RemoveLast(x => x.Name.Contains('n'));
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
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
        var source = new Chain(false);

        var done = source.Clear();
        Assert.Equal(0, done);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        done = source.Clear();
        Assert.Equal(4, done);
        Assert.Empty(source);
    }
}