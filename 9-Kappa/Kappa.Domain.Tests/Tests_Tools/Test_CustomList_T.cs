namespace Kappa.Domain.Tests;

// ========================================================
public static class Test_CustomList_T
{
    internal class Element(string name)
    {
        public string Name { get; set; } = name;
        public override string ToString() => Name ?? string.Empty;
    }
    internal class Chain : CustomList<Element>
    {
        public Chain(bool sensitive)
        {
            Validate = (item, add) =>
            {
                item.ThrowWhenNull(nameof(item));
                if (add) item.Name.NotNullNotEmpty();
                return item;
            };
            Compare = (source, target) =>
            {
                return ReferenceEquals(source, target)
                || string.Compare(source.Name, target.Name, !CaseSensitive) == 0;
            };
            AcceptDuplicate = (source, target) =>
            {
                return ReferenceEquals(source, target)
                ? true
                : throw new InvalidOperationException("Duplicated element.").WithData(target, nameof(target));
            };
            CaseSensitive = sensitive;
        }
        public Chain(bool sensitive, Element item) : this(sensitive) => Add(item);
        public Chain(bool sensitive, IEnumerable<Element> range) : this(sensitive) => AddRange(range);
        public bool CaseSensitive
        {
            get => _CaseSensitive;
            set
            {
                if (_CaseSensitive == value) return;
                _CaseSensitive = value;

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
        catch (InvalidOperationException) { }

        try { _ = new Chain(false, [xone, new Element("ONE")]); Assert.Fail(); }
        catch (InvalidOperationException) { }

        items = new Chain(true, [xone, new Element("ONE")]); // True sensitive...
        Assert.Same(xone, items[0]);
        Assert.Equal("ONE", items[1].Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Settings()
    {
        var items = new Chain(true, [xone, new Element("ONE")]);

        try { items.CaseSensitive = false; Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf(new Element("any")));

        Assert.Equal(0, items.IndexOf(new Element("ONE")));
        Assert.Equal(3, items.LastIndexOf(new Element("ONE")));

        var list = items.IndexesOf(new Element("ONE"));
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
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var target = source.GetRange(0, 0);
        Assert.Empty(target);

        target = source.GetRange(1, 2);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
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
        catch (InvalidOperationException) { }

        items = new Chain(false, [xone, xtwo, xthree]);
        try { items.Replace(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        items = new Chain(false, [xone, xtwo, xthree]);
        try { items.Replace(0, new Element("")); Assert.Fail(); }
        catch (ArgumentException) { }
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

        items = new Chain(false, [xone, xtwo, xthree]);
        try { items.Add(new Element("one")); Assert.Fail(); }
        catch (InvalidOperationException) { }

        try { items.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items.Add(new Element("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var items = new Chain(false, [xone, xtwo, xthree]);
        var done = items.AddRange([]);
        Assert.Equal(0, done);

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

        items = new Chain(false, [xone, xtwo, xthree]);
        try { items.Insert(3, new Element("one")); Assert.Fail(); }
        catch (InvalidOperationException) { }

        try { items.Insert(3, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items.Insert(3, new Element("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var items = new Chain(false, [xone, xtwo, xthree]);
        var done = items.InsertRange(3, []);
        Assert.Equal(0, done);

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

        items = new Chain(false, [xone, xtwo, xthree]);
        done = items.RemoveRange(1, 0);
        Assert.Equal(0, done);

        items = new Chain(false, [xone, xtwo, xthree]);
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
        var done = items.Remove(new Element("any"));
        Assert.Equal(0, done);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        done = items.Remove(new Element("ONE"));
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xone, items[2]);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        done = items.RemoveLast(new Element("ONE"));
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        items = new Chain(false, [xone, xtwo, xthree, xone]);
        done = items.RemoveAll(new Element("ONE"));
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