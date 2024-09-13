using System.Xml;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static partial class Test_CoreList_T
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
    partial class Chain : CoreList<IElement>, IElement
    {
        public Chain(bool sensitive) => Sensitive = sensitive;
        public Chain(bool sensitive, int capacity) : this(sensitive) => Capacity = capacity;
        public Chain(bool sensitive, IEnumerable<IElement> range) : this(sensitive) => AddRange(range);
        protected Chain(Chain source) : this(source.Sensitive) => AddRange(source);

        public bool Sensitive { get; }
        public override IElement ValidateItem(IElement item)
        {
            item.ThrowWhenNull();
            if (item is Element named) named.Name.NotNullNotEmpty();
            return item;
        }
        public override bool CompareItems(IElement x, IElement y)
            => x is Element xnamed && y is Element ynamed
            ? string.Compare(xnamed.Name, ynamed.Name, !Sensitive) == 0
            : ReferenceEquals(x, y);
        public override bool CanInclude(IElement item, IElement items) => ReferenceEquals(item, items)
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

        Assert.Equal(-1, items.IndexOf(xfive));

        Assert.Equal(0, items.IndexOf(xone));
        Assert.Equal(0, items.IndexOf(new Element("ONE")));

        Assert.Equal(3, items.LastIndexOf(xone));
        Assert.Equal(3, items.LastIndexOf(new Element("ONE")));

        var list = items.IndexesOf(xone);
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        list = items.IndexesOf(new Element("ONE"));
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
        items.Sort(new MyComparer());

        Assert.Same(xfour, items[0]);
        Assert.Same(xone, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xtwo, items[3]);
    }
    struct MyComparer : IComparer<IElement>
    {
        public int Compare(IElement? x, IElement? y)
        {
            if (x is null && y is null) return 0;
            if (x is null) return -1;
            if (y is null) return +1;

            if (x is Chain xchain && y is Chain ychain)
            {
                if (xchain.Count != ychain.Count) return xchain.Count > ychain.Count ? -1 : +1;
                for (int i = 0; i < xchain.Count; i++)
                {
                    var xitem = xchain[i];
                    var yitem = ychain[i];
                    var r = Compare(xitem, yitem);
                    if (r != 0) return r;
                }
                return 0;
            }
            if (x is Chain) return -1;
            if (y is Chain) return +1;

            if (x is Element xnamed && y is Element ynamed)
            {
                return string.Compare(xnamed.Name, ynamed.Name);
            }
            if (x is Element) return -1;
            if (y is Element) return +1;

            throw new NotSupportedException("Unknown element.");
        }
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

        try { items.GetRange(-1, 0); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { items.GetRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { items.GetRange(5, 0); Assert.Fail(); }
        catch (ArgumentException) { }

        try { items.GetRange(0, 5); Assert.Fail(); }
        catch (ArgumentException) { }
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

    //[Enforced]
    [Fact]
    public static void Test_Replace_Extended_Empty()
    {
        var other = new Chain(false);
        var items = new Chain(false, [xone, xtwo, xthree]);

        var num = items.Replace(1, other);
        Assert.Equal(1, num);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Equal(xthree, items[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var items = new Chain(false, [xone, xtwo]);
        var num = items.Add(xthree);
        Assert.Equal(1, num);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        try { _ = items.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = items.Add(new Element("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Duplicates()
    {
        var items = new Chain(false, [xone, xtwo]);
        var num = items.Add(xone);
        Assert.Equal(1, num);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xone, items[2]);

        items = new Chain(false, [xone, xtwo]);
        try { _ = items.Add(new Element("one")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Extended()
    {
        var other = new Chain(false, []);
        var items = new Chain(false, [xone, xtwo, xthree]);
        var num = items.Add(other);
        Assert.Equal(0, num);
        Assert.Equal(3, items.Count);

        other = new Chain(false, [xfour, xfive]);
        num = items.Add(other);
        Assert.Equal(2, num);
        Assert.Equal(5, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Equal(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xfive, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var items = new Chain(false, [xone, xtwo]);
        var num = items.AddRange([]);
        Assert.Equal(0, num);

        num = items.AddRange([xthree, xfour]);
        Assert.Equal(2, num);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);

        try { _ = items.AddRange([new Element("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = items.AddRange([xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = items.AddRange([xfive, new Element("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Extended()
    {
        var other = new Chain(false, []);
        var items = new Chain(false, [xone, xtwo]);
        var num = items.AddRange([other]);
        Assert.Equal(0, num);
        Assert.Equal(2, items.Count);

        other = new Chain(false, [xfour, xfive]);
        num = items.AddRange([xthree, other]);
        Assert.Equal(3, num);
        Assert.Equal(5, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Equal(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xfive, items[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var items = new Chain(false, [xone, xtwo]);
        var num = items.Insert(2, xthree);
        Assert.Equal(1, num);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        try { _ = items.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = items.Insert(0, new Element("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Duplicates()
    {
        var items = new Chain(false, [xone, xtwo]);
        var num = items.Insert(2, xone);
        Assert.Equal(1, num);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xone, items[2]);

        items = new Chain(false, [xone, xtwo]);
        try { _ = items.Insert(0, new Element("one")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Extended()
    {
        var other = new Chain(false, []);
        var items = new Chain(false, [xone, xtwo, xthree]);
        var num = items.Insert(3, other);
        Assert.Equal(0, num);
        Assert.Equal(3, items.Count);

        other = new Chain(false, [xfour, xfive]);
        num = items.Insert(3, other);
        Assert.Equal(2, num);
        Assert.Equal(5, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Equal(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xfive, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var items = new Chain(false, [xone, xtwo]);
        var num = items.InsertRange(2, []);
        Assert.Equal(0, num);

        num = items.InsertRange(2, [xthree, xfour]);
        Assert.Equal(2, num);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);

        try { _ = items.InsertRange(0, [new Element("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = items.InsertRange(0, [xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = items.InsertRange(0, [xfive, new Element("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Extended()
    {
        var other = new Chain(false, []);
        var items = new Chain(false, [xone, xtwo]);
        var num = items.InsertRange(2, [other]);
        Assert.Equal(0, num);
        Assert.Equal(2, items.Count);

        other = new Chain(false, [xfour, xfive]);
        num = items.InsertRange(2, [xthree, other]);
        Assert.Equal(3, num);
        Assert.Equal(5, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Equal(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xfive, items[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xone]);

        var num = items.RemoveAt(0);
        Assert.Equal(1, num);
        Assert.Equal(3, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xone, items[2]);

        try { items.RemoveAt(999); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xone]);
        var num = items.RemoveRange(0, 0);
        Assert.Equal(0, num);
        Assert.Equal(4, items.Count);

        num = items.RemoveRange(0, 4);
        Assert.Equal(4, num);
        Assert.Empty(items);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        num = items.RemoveRange(0, 1);
        Assert.Equal(1, num);
        Assert.Equal(3, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xone, items[2]);

        try { _ = items.RemoveRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { _ = items.RemoveRange(-1, 0); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { _ = items.RemoveRange(4, 0); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = items.RemoveRange(0, 4); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xone]);
        var num = items.Remove(xfour);
        Assert.Equal(0, num);
        Assert.Equal(4, items.Count);

        num = items.Remove(xone);
        Assert.Equal(1, num);
        Assert.Equal(3, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xone, items[2]);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        num = items.Remove(new Element("ONE"));
        Assert.Equal(1, num);
        Assert.Equal(3, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xone, items[2]);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        num = items.RemoveLast(xone);
        Assert.Equal(1, num);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        num = items.RemoveLast(new Element("ONE"));
        Assert.Equal(1, num);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        num = items.RemoveAll(xone);
        Assert.Equal(2, num);
        Assert.Equal(2, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        num = items.RemoveAll(new Element("ONE"));
        Assert.Equal(2, num);
        Assert.Equal(2, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xone]);
        var num = items.Remove(x => ((Element)x).Name.Contains('z'));
        Assert.Equal(0, num);

        num = items.Remove(x => ((Element)x).Name.Contains('n'));
        Assert.Equal(1, num);
        Assert.Equal(3, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xone, items[2]);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        num = items.RemoveLast(x => ((Element)x).Name.Contains('n'));
        Assert.Equal(1, num);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        num = items.RemoveAll(x => ((Element)x).Name.Contains('n'));
        Assert.Equal(2, num);
        Assert.Equal(2, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var items = new Chain(false);

        var num = items.Clear();
        Assert.Equal(0, num);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        num = items.Clear();
        Assert.Equal(4, num);
        Assert.Empty(items);
    }
}