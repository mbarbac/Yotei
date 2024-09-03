using Xunit.Sdk;

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
    partial class Chain : CoreList<string, IElement>
    {
        public Chain(bool sensitive) : base() => Sensitive = sensitive;
        public Chain(bool sensitive, int capacity) : this(sensitive) => Capacity = capacity;
        public Chain(bool sensitive, IEnumerable<Element> range) : this(sensitive) => AddRange(range);
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
}