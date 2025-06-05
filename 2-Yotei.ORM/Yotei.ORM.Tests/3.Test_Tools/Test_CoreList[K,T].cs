using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_CoreList_KT
{
    public interface IElement { }

    public class Element(string name) : IElement
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
    public partial class Chain : CoreList<string, IElement>, IElement
    {
        public Chain(bool sensitive) => Sensitive = sensitive;
        public Chain(bool sensitive, int capacity) : this(sensitive) => Capacity = capacity;
        public Chain(bool sensitive, IEnumerable<IElement> range) : this(sensitive) => AddRange(range);
        public Chain(Chain source) : this(source.Sensitive) => AddRange(source);

        public override IElement ValidateItem(IElement item) => item.ThrowWhenNull();
        public override string GetKey(IElement item)
            => item is Element named
            ? named.Name
            : throw new ArgumentException("Element is not a named instance.").WithData(item);
        public override string ValidateKey(string key) => key.NotNullNotEmpty();
        public override IEqualityComparer<string> Comparer
            => Sensitive
            ? StringComparer.Ordinal
            : StringComparer.OrdinalIgnoreCase;
        public override bool ExpandItems => true;
        public override bool IsValidDuplicate(IElement source, IElement item)
            => ReferenceEquals(source, item)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);

        public bool Sensitive { get; }
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
        catch (EmptyException) { }
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
        var items = new Chain(false, [xone, xtwo, xthree, xone]);
        var target = items.Clone();

        Assert.NotSame(items, target);
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
    public static void Test_Find_Predicate()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf(x => ((Element)x).Name.Contains('z')));

        Assert.Equal(0, items.IndexOf(x => ((Element)x).Name.Contains('n')));
        Assert.Equal(3, items.LastIndexOf(x => ((Element)x).Name.Contains('n')));

        var list = items.IndexesOf(x => ((Element)x).Name.Contains('n'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var items = new Chain(false, [xone, xtwo, xthree]);
        var num = items.Replace(1, xtwo);
        Assert.Equal(0, num);
        Assert.Equal(3, items.Count);

        num = items.Replace(1, xone);
        Assert.Equal(1, num);
        Assert.Equal(3, items.Count);
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
        var items = new Chain(false, [xone, xtwo, xthree]);
        var other = new Chain(false, [xfour, xfive]);

        var num = items.Replace(1, other);
        Assert.Equal(2, num);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Equal(xfour, items[1]);
        Assert.Same(xfive, items[2]);
        Assert.Same(xthree, items[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Extended_Empty()
    {
        var items = new Chain(false, [xone, xtwo, xthree]);
        var other = new Chain(false);

        var num = items.Replace(1, other);
        Assert.Equal(0, num);
    }
}