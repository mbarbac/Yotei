using THost = Yotei.ORM.Records.Code.ParameterList;
using TItem = Yotei.ORM.Records.Code.Parameter;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_ParameterList
{
    readonly static TItem x007 = new("Id", "007");
    readonly static TItem x008 = new("Id", "008");
    readonly static TItem xJames = new("FirstName", "James");
    readonly static TItem xBond = new("LastName", "Bond");
    readonly static TItem xMi6 = new("Organization", "Mi6");

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var items = new THost(engine);
        Assert.Empty(items);
    }
    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var engine = new FakeEngine();
        var items = new THost(engine, x007);
        Assert.Single(items);
        Assert.Equal("007", items[0].Value);

        try { _ = new THost(engine, (TItem)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var engine = new FakeEngine();
        var items = new THost(engine, [x007, xJames, xBond]);
        Assert.Equal(3, items.Count);
        Assert.Equal("007", items[0].Value);
        Assert.Equal("James", items[1].Value);
        Assert.Equal("Bond", items[2].Value);

        items = new THost(engine, [x007, x007]);
        Assert.Equal(2, items.Count);
        Assert.Same(x007, items[0]);
        Assert.Same(x007, items[1]);

        try { _ = new THost(engine, (IEnumerable<TItem>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new THost(engine, [x007, new TItem("ID", "008")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Key()
    {
        var engine = new FakeEngine();
        var items = new THost(engine, [x007, xJames, xBond, x007]);
        Assert.Equal(0, items.IndexOf("ID"));
        Assert.Equal(1, items.IndexOf("FIRSTname"));
        Assert.Equal(-1, items.IndexOf("x"));

        Assert.Equal(3, items.LastIndexOf("ID"));

        var list = items.IndexesOf("ID");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var engine = new FakeEngine();
        var items = new THost(engine, [x007, xJames, xBond, x007]);

        var list = items.IndexesOf(x => x.Name.Contains('d'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Next_Name()
    {
        var engine = new FakeEngine();
        var items = new THost(engine, [x007, xJames, xBond, x007]);

        var name = items.NextName();
        Assert.Equal("#4", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, [x007, xJames, xBond]);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);

        source = new THost(engine, [x007, xJames, xBond, x007]);
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
        Assert.Same(source[3], target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, [x007, xJames, xBond]);

        var target = source.GetRange(1, 0);
        Assert.Empty(target);

        target = source.GetRange(0, source.Count);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(source[1], target[0]);
        Assert.Same(source[2], target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, [x007, xJames, xBond]);

        var target = source.Replace(0, x007);
        Assert.Same(source, target);
        Assert.Same(source[0], target[0]);

        target = source.Replace(0, new TItem("Id", "008"));
        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Equal("008", target[0].Value);

        target = source.Replace(1, x007);
        Assert.NotSame(source, target);
        Assert.Same(x007, target[0]);
        Assert.Same(x007, target[1]);
        Assert.Same(xBond, target[2]);

        try { source.Replace(0, new TItem("FirstName", "...")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, [x007, xJames, xBond]);

        var target = source.Add(xMi6);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(xMi6, target[3]);

        try { source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add(x008); Assert.Fail(); }
        catch (DuplicateException) { }

        target = source.Add(x007);
        Assert.NotSame(source, target);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(x007, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, [x007, xJames, xBond]);

        var target = source.AddRange([xMi6, new TItem("any", "value")]);
        Assert.NotSame(source, target);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(xMi6, target[3]);
        Assert.Same("value", ((TItem)target[4]).Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, [x007, xJames, xBond]);

        var target = source.Insert(0, xMi6);
        Assert.NotSame(source, target);
        Assert.Same(xMi6, target[0]);
        Assert.Same(x007, target[1]);
        Assert.Same(xJames, target[2]);
        Assert.Same(xBond, target[3]);

        try { source.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Insert(0, x008); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, [x007, xJames, xBond]);

        var target = source.InsertRange(0, [xMi6, new TItem("any", "value")]);
        Assert.NotSame(source, target);
        Assert.Same(xMi6, target[0]);
        Assert.Same("value", ((TItem)target[1]).Value);
        Assert.Same(x007, target[2]);
        Assert.Same(xJames, target[3]);
        Assert.Same(xBond, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, [x007, xJames, xBond]);

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, [x007, xJames, xBond]);

        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 2);
        Assert.NotSame(source, target);
        Assert.Same(xBond, target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, [x007, xJames, xBond, x007]);

        var target = source.Remove("ID");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.NotSame(source, target);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);

        target = source.RemoveLast("ID");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.NotSame(source, target);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);

        target = source.RemoveAll("ID");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.NotSame(source, target);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, [x007, xJames, xBond, x007]);

        var target = source.Remove(x => x.Name.Contains('d'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.NotSame(source, target);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);

        target = source.RemoveLast(x => x.Name.Contains('d'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.NotSame(source, target);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);

        target = source.RemoveAll(x => x.Name.Contains('d'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.NotSame(source, target);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, [x007, xJames, xBond, x007]);

        var target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}