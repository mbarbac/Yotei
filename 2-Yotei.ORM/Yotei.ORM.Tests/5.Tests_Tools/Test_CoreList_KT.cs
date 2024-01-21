namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
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

    // ----------------------------------------------------

    [Cloneable]
    internal partial class Chain : CoreList<string, Element>
    {
        public Chain(bool sensitive) => CaseSensitive = sensitive;
        public Chain(bool sensitive, Element item) : this(sensitive) => Add(item);
        public Chain(bool sensitive, IEnumerable<Element> range) : this(sensitive) => AddRange(range);
        protected Chain(Chain source) : this(source.CaseSensitive) => AddRange(source);

        protected override Element ValidateItem(Element item) => item.ThrowWhenNull();
        protected override string GetKey(Element item) => item.ThrowWhenNull().Name;
        protected override string ValidateKey(string key) => key.NotNullNotEmpty();
        protected override bool CompareKeys(string source, string item) => string.Compare(source, item, !CaseSensitive) == 0;
        protected override bool SameItem(Element source, Element item) => ReferenceEquals(source, item);
        protected override bool AcceptDuplicate(Element source, Element item)
            => SameItem(source, item)
            ? true
            : throw new DuplicateException("Duplicated item.").WithData(item);

        [WithGenerator]
        public bool CaseSensitive
        {
            get => _CaseSensitive;
            set
            {
                if (_CaseSensitive == value) return;
                _CaseSensitive = value;

                if (Count == 0) return;
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
        bool _CaseSensitive;
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
        catch (ArgumentNullException) { }
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
        Assert.Same(xone, items[0]);
        Assert.Equal("ONE", items[1].Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Settings()
    {
        var source = new Chain(true, [xone, new Element("ONE")]);

        try { _ = source.WithCaseSensitive(false); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Key()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf("any"));

        Assert.Equal(0, items.IndexOf("ONE"));
        Assert.Equal(3, items.LastIndexOf("ONE"));

        var list = items.IndexesOf("ONE");
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

        Assert.Equal(0, items.IndexOf(x => x.Name.Contains('e')));
        Assert.Equal(3, items.LastIndexOf(x => x.Name.Contains('e')));

        var list = items.IndexesOf(x => x.Name.Contains('e'));
        Assert.Equal(3, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var num = source.Replace(0, xone);
        Assert.Equal(0, num);

        source = new Chain(false, [xone, xtwo, xthree]);
        num = source.Replace(0, new Element("one"));
        Assert.Equal(1, num);
        Assert.NotSame(xone, source[0]);
        Assert.Equal("one", source[0].Name);

        source = new Chain(false, [xone, xtwo, xthree]);
        try { source.Replace(1, new Element("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Replace(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Replace(0, new Element("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var num = source.Add(xfour);
        Assert.Equal(1, num);
        Assert.Equal(4, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xfour, source[3]);

        source = new Chain(false, [xone, xtwo, xthree]);
        num = source.Add(xone);
        Assert.Equal(1, num);
        Assert.Equal(4, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xone, source[3]);

        try { source.Add(new Element("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add(new Element("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var num = source.AddRange([]);
        Assert.Equal(0, num);

        source = new Chain(false, [xone, xtwo, xthree]);
        num = source.AddRange([xfour, xfive]);
        Assert.Equal(2, num);
        Assert.Equal(5, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xfour, source[3]);
        Assert.Same(xfive, source[4]);

        try { source.AddRange(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.AddRange([null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.AddRange([new Element("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var num = source.Insert(3, xfour);
        Assert.Equal(1, num);
        Assert.Equal(4, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xfour, source[3]);

        source = new Chain(false, [xone, xtwo, xthree]);
        num = source.Insert(3, xone);
        Assert.Equal(1, num);
        Assert.Equal(4, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xone, source[3]);

        try { source.Insert(3, new Element("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Insert(3, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Insert(3, new Element("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var num = source.InsertRange(3, []);
        Assert.Equal(0, num);

        source = new Chain(false, [xone, xtwo, xthree]);
        num = source.InsertRange(3, [xfour, xfive]);
        Assert.Equal(2, num);
        Assert.Equal(5, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xfour, source[3]);
        Assert.Same(xfive, source[4]);

        try { source.InsertRange(3, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.InsertRange(3, [null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.InsertRange(3, [new Element("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var num = source.RemoveAt(0);
        Assert.Equal(1, num);
        Assert.Equal(2, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var num = source.RemoveRange(0, 0);
        Assert.Equal(0, num);

        num = source.RemoveRange(1, 0);
        Assert.Equal(0, num);

        num = source.RemoveRange(1, 2);
        Assert.Equal(2, num);
        Assert.Single(source);
        Assert.Same(xone, source[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);
        var num = source.Remove("any");
        Assert.Equal(0, num);

        num = source.Remove("ONE");
        Assert.Equal(1, num);
        Assert.Equal(3, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        num = source.RemoveLast("ONE");
        Assert.Equal(1, num);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        num = source.RemoveAll("ONE");
        Assert.Equal(2, num);
        Assert.Equal(2, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);
        var num = source.Remove(x => x.Name.Contains('z'));
        Assert.Equal(0, num);

        num = source.Remove(x => x.Name.Contains('e'));
        Assert.Equal(1, num);
        Assert.Equal(3, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
        Assert.Same(xone, source[2]);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        num = source.RemoveLast(x => x.Name.Contains('e'));
        Assert.Equal(1, num);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        num = source.RemoveAll(x => x.Name.Contains('e'));
        Assert.Equal(3, num);
        Assert.Single(source);
        Assert.Same(xtwo, source[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new Chain(false);
        var num = source.Clear();
        Assert.Equal(0, num);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        num = source.Clear();
        Assert.Equal(4, num);
        Assert.Empty(source);
    }
}