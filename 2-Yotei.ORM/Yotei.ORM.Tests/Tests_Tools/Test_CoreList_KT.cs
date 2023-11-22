#pragma warning disable IDE0028

namespace Yotei.ORM.Tests.Tools;

// ========================================================
//[Enforced]
public static class Test_CoreList_KT
{
    public class Element(string name)
    {
        public string Name { get; set; } = name;
        public override string ToString() => Name ?? string.Empty;
    }
    public class Chain : CoreList<string, Element>
    {
        public Chain(bool sensitive)
        {
            CaseSensitive = sensitive;
            ValidateItem = (item, add) => { item.ThrowWhenNull(); if (add) ValidateKey(GetKey(item)); return item; };
            GetKey = (item) => item.Name;
            ValidateKey = (key) => key.NotNullNotEmpty();
            Compare = (source, target) => string.Compare(source, target, !CaseSensitive) == 0;
            IsSame = (source, target) => ReferenceEquals(source, target);
            ValidDuplicate = (source, target) => IsSame(source, target)
                ? true
                : throw new DuplicateException("Duplicated element.").WithData(target);
        }
        public Chain(bool sensitive, Element item) : this(sensitive) => Add(item);
        public Chain(bool sensitive, IEnumerable<Element> range) : this(sensitive) => AddRange(range);
        public override string ItemToString(Element item) => item.Name ?? string.Empty;

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

    static readonly Element xone = new("one");
    static readonly Element xtwo = new("two");
    static readonly Element xthree = new("three");
    static readonly Element xfour = new("four");
    static readonly Element xfive = new("five");
    static readonly Element xsix = new("six");

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
        catch (EmptyException) { }
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

        try { _ = new Chain(false, [xone, new Element("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        items = new Chain(false, [xone, xtwo, xone]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xone, items[2]);

        items = new Chain(true, [xone, new Element("ONE")]);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Equal("ONE", items[1].Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Settings()
    {
        var items = new Chain(true, [xone, new Element("ONE")]);

        try { items.CaseSensitive = false; Assert.Fail(); }
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
    public static void Test_GetRange()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xone]);

        var range = items.GetRange(0, 0);
        Assert.Empty(range);

        range = items.GetRange(1, 2);
        Assert.Equal(2, range.Count);
        Assert.Same(xtwo, range[0]);
        Assert.Same(xthree, range[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Setter()
    {
        var items = new Chain(false, [xone, xtwo, xthree]);
        items[1] = xone;
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);
        Assert.Same(xthree, items[2]);

        items = new Chain(false, [xone, xtwo, xthree]);
        items[0] = new Element("ONE");
        Assert.Equal(3, items.Count);
        Assert.Equal("ONE", items[0].Name);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        items = new Chain(false, [xone, xtwo, xthree]);
        try { items[1] = new Element("ONE"); Assert.Fail(); }
        catch (DuplicateException) { }

        try { items[0] = null!; Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items[0] = new Element(""); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var items = new Chain(false, [xone, xtwo, xthree]);
        var done = items.Replace(0, xone);
        Assert.Equal(0, done);

        items = new Chain(false, [xone, xtwo, xthree]);
        done = items.Replace(1, xone);
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);
        Assert.Same(xthree, items[2]);

        items = new Chain(false, [xone, xtwo, xthree]);
        done = items.Replace(0, new Element("ONE"));
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Equal("ONE", items[0].Name);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        items = new Chain(false, [xone, xtwo, xthree]);
        try { items.Replace(1, new Element("ONE")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { items.Replace(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items.Replace(0, new Element("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var items = new Chain(false, [xone, xtwo, xthree]);
        var done = items.Add(xfour);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);

        items = new Chain(false, [xone, xtwo, xthree]);
        done = items.Add(xone);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xone, items[3]);

        try { items.Add(new("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { items.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items.Add(new("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var items = new Chain(false, [xone, xtwo, xthree]);
        var done = items.AddRange([]);
        Assert.Equal(0, done);
        Assert.Equal(3, items.Count);

        items = new Chain(false, [xone, xtwo, xthree]);
        done = items.AddRange([xfour, xfive]);
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xfive, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var items = new Chain(false, [xone, xtwo, xthree]);
        var done = items.Insert(3, xfour);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);

        items = new Chain(false, [xone, xtwo, xthree]);
        done = items.Insert(3, xone);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xone, items[3]);

        try { items.Insert(3, new("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { items.Insert(3, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items.Insert(3, new("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var items = new Chain(false, [xone, xtwo, xthree]);
        var done = items.InsertRange(3, []);
        Assert.Equal(0, done);
        Assert.Equal(3, items.Count);

        items = new Chain(false, [xone, xtwo, xthree]);
        done = items.InsertRange(3, [xfour, xfive]);
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xfive, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var items = new Chain(false, [xone, xtwo, xthree]);
        var done = items.RemoveAt(0);
        Assert.Equal(1, done);
        Assert.Equal(2, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var items = new Chain(false, [xone, xtwo, xthree]);
        var done = items.RemoveRange(0, 0);
        Assert.Equal(0, done);

        done = items.RemoveRange(1, 0);
        Assert.Equal(0, done);

        done = items.RemoveRange(1, 2);
        Assert.Equal(2, done);
        Assert.Single(items);
        Assert.Same(xone, items[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xone]);
        var done = items.Remove("any");
        Assert.Equal(0, done);
        Assert.Equal(4, items.Count);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        done = items.Remove("one");
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xone, items[2]);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        done = items.RemoveLast("one");
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        done = items.RemoveAll("one");
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xone]);
        var done = items.Remove(x => x.Name.Contains('z'));
        Assert.Equal(0, done);
        Assert.Equal(4, items.Count);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        done = items.Remove(x => x.Name.Contains('e'));
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xone, items[2]);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        done = items.RemoveLast(x => x.Name.Contains('e'));
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        done = items.RemoveAll(x => x.Name.Contains('e'));
        Assert.Equal(3, done);
        Assert.Single(items);
        Assert.Same(xtwo, items[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var items = new Chain(false);
        var done = items.Clear();
        Assert.Equal(0, done);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        done = items.Clear();
        Assert.Equal(4, done);
        Assert.Empty(items);
    }
}