using THost = Yotei.Tools.Tests.Test_CoreList.Chain;
using TItem = Yotei.Tools.Tests.Test_CoreList.Persona;
using TKey = string;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_CoreList
{
    public class Persona(string name)
    {
        public string Name { get; set; } = name;
        public override string ToString() => Name ?? string.Empty;
    }

    public class Chain : CoreList<string, TItem>
    {
        public Chain(bool sensitive) => CaseSensitive = sensitive;
        public Chain(bool sensitive, TItem item) : this(sensitive) => Add(item);
        public Chain(bool sensitive, IEnumerable<TItem> range) : this(sensitive) => AddRange(range);

        public override TItem ValidateItem(TItem item) => item.ThrowWhenNull();
        public override bool AcceptDuplicate(int index, TItem item)
        {
            return this[index].Equals(item)
                ? true
                : throw new DuplicateException("Element is duplicated.").WithData(item);
        }
        public override TKey GetKey(TItem item) => item.Name;
        public override string ValidateKey(TKey key) => key.NotNullNotEmpty();
        public override bool CompareKeys(
            TKey source, TKey target) => string.Compare(source, target, !CaseSensitive) == 0;

        public List<TItem> SourceItems() => Items;

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

    static readonly TItem xone = new("one");
    static readonly TItem xtwo = new("two");
    static readonly TItem xthree = new("three");
    static readonly TItem xfour = new("four");
    static readonly TItem xfive = new("five");

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new THost(false);
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var items = new THost(false, xone);
        Assert.Single(items);
        Assert.Equal("one", items[0].Name);

        try { _ = new THost(false, (TItem)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new THost(false, new TItem(null!)); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new THost(false, new TItem("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var items = new THost(false, [xone, xtwo, xthree]);
        Assert.Equal(3, items.Count);
        Assert.Equal("one", items[0].Name);
        Assert.Equal("two", items[1].Name);
        Assert.Equal("three", items[2].Name);

        items = new THost(false, [xone, xtwo, xone]);
        Assert.Equal(3, items.Count);
        Assert.Equal("one", items[0].Name);
        Assert.Equal("two", items[1].Name);
        Assert.Same(items[0], items[2]);

        try { _ = new THost(false, [xone, xtwo, new TItem("ONE")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new THost(false, [xone, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Setting()
    {
        var items = new THost(true, [xone, xtwo, new TItem("ONE")]);

        try { items.CaseSensitive = false; Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Key()
    {
        var items = new THost(false, [xone, xtwo, xone]);

        Assert.Equal(-1, items.IndexOf("x"));

        Assert.Equal(0, items.IndexOf("ONE"));
        Assert.Equal(2, items.LastIndexOf("ONE"));

        var list = items.IndexesOf("ONE");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(2, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var items = new THost(false, [xone, xtwo, xthree]);

        Assert.Equal(-1, items.IndexOf(x => x.Name.Contains('p')));

        Assert.Equal(0, items.IndexOf(x => x.Name.Contains('e')));
        Assert.Equal(2, items.LastIndexOf(x => x.Name.Contains('e')));

        var list = items.IndexesOf(x => x.Name.Contains('e'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(2, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_ToList()
    {
        var items = new THost(false, [xone, xtwo, xthree]);
        var source = items.SourceItems();

        var list = items.ToList();
        Assert.NotSame(source, list);
        Assert.Equal(3, list.Count);
        Assert.Same(source[0], list[0]);
        Assert.Same(source[1], list[1]);
        Assert.Same(source[2], list[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var items = new THost(false, [xone, xtwo, xthree]);
        var source = items.SourceItems();

        var list = items.GetRange(0, 0);
        Assert.Empty(list);

        list = items.GetRange(0, source.Count);
        Assert.NotSame(source, list);
        Assert.Equal(3, list.Count);
        Assert.Same(source[0], list[0]);
        Assert.Same(source[1], list[1]);
        Assert.Same(source[2], list[2]);

        list = items.GetRange(1, 2);
        Assert.Equal(2, list.Count);
        Assert.Same(source[1], list[0]);
        Assert.Same(source[2], list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Setter()
    {
        var items = new THost(false, [xone, xtwo, xthree]);
        items[0] = xone;
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        items = new THost(false, [xone, xtwo, xthree]);
        items[2] = xone;
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xone, items[2]);

        try
        {
            items = new THost(false, [xone, xtwo, xthree]);
            items[2] = new TItem("ONE");
            Assert.Fail();
        }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Method()
    {
        var items = new THost(false, [xone, xtwo, xthree]);
        var done = items.Replace(0, xone);
        Assert.Equal(0, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        items = new THost(false, [xone, xtwo, xthree]);
        done = items.Replace(2, xone);
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xone, items[2]);

        try
        {
            items = new THost(false, [xone, xtwo, xthree]);
            done = items.Replace(2, new TItem("ONE"));
            Assert.Fail();
        }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var items = new THost(false, [xone, xtwo, xthree]);
        var done = items.Add(xfour);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);

        items = new THost(false, [xone, xtwo, xthree]);
        done = items.Add(xone);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xone, items[3]);

        try { items.Add(new TItem("ONE")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { items.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items.Add(new TItem(null!)); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items.Add(new TItem("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var items = new THost(false, [xone, xtwo, xthree]);
        var done = items.AddRange([xfour, xfive]);
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xfive, items[4]);

        items = new THost(false, [xone, xtwo, xthree]);
        done = items.AddRange([xfour, xone]);
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xone, items[4]);

        try
        {
            items = new THost(false, [xone, xtwo, xthree]);
            done = items.AddRange([xfour, new TItem("ONE")]);
            Assert.Fail();
        }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var items = new THost(false, [xone, xtwo, xthree]);
        var done = items.Insert(3, xfour);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);

        items = new THost(false, [xone, xtwo, xthree]);
        done = items.Insert(3, xone);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xone, items[3]);

        try { items.Insert(3, new TItem("ONE")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { items.Insert(3, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items.Insert(3, new TItem(null!)); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items.Insert(3, new TItem("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var items = new THost(false, [xone, xtwo, xthree]);
        var done = items.InsertRange(3, [xfour, xfive]);
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xfive, items[4]);

        items = new THost(false, [xone, xtwo, xthree]);
        done = items.InsertRange(3, [xfour, xone]);
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xone, items[4]);

        try
        {
            items = new THost(false, [xone, xtwo, xthree]);
            done = items.InsertRange(3, [xfour, new TItem("ONE")]);
            Assert.Fail();
        }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var items = new THost(false, [xone, xtwo, xthree]);
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
        var items = new THost(false, [xone, xtwo, xthree]);
        var done = items.RemoveRange(0, 0);
        Assert.Equal(0, done);

        done = items.RemoveRange(1, 2);
        Assert.Equal(2, done);
        Assert.Single(items);
        Assert.Same(xone, items[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Key()
    {
        var items = new THost(false, [xone, xtwo, xthree, xone]);
        var done = items.Remove("...");
        Assert.Equal(0, done);
        Assert.Equal(4, items.Count);

        items = new THost(false, [xone, xtwo, xthree, xone]);
        done = items.Remove("ONE");
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xone, items[2]);

        items = new THost(false, [xone, xtwo, xthree, xone]);
        done = items.RemoveLast("ONE");
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        items = new THost(false, [xone, xtwo, xthree, xone]);
        done = items.RemoveAll("ONE");
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var items = new THost(false, [xone, xtwo, xthree]);
        var done = items.Remove(x => x.Name.Contains('x'));
        Assert.Equal(0, done);
        Assert.Equal(3, items.Count);

        items = new THost(false, [xone, xtwo, xthree]);
        done = items.Remove(x => x.Name.Contains('e'));
        Assert.Equal(1, done);
        Assert.Equal(2, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);

        items = new THost(false, [xone, xtwo, xthree]);
        done = items.RemoveLast(x => x.Name.Contains('e'));
        Assert.Equal(1, done);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);

        items = new THost(false, [xone, xtwo, xthree]);
        done = items.RemoveAll(x => x.Name.Contains('e'));
        Assert.Equal(2, done);
        Assert.Single(items);
        Assert.Same(xtwo, items[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var items = new THost(false);
        var done = items.Clear();
        Assert.Equal(0, done);
        Assert.Empty(items);

        items = new THost(false, [xone, xtwo, xthree]);
        done = items.Clear();
        Assert.Equal(3, done);
        Assert.Empty(items);
    }
}