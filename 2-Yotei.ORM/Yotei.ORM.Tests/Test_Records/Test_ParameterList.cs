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
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var items = new THost(engine, [x007, xJames, xBond, x007]);
        int index;
        List<int> list;

        Assert.True(items.Contains("ID"));
        Assert.True(items.Contains(new TItem("ID", null)));
        Assert.True(items.Contains(x007));

        index = items.IndexOf(x007); Assert.Equal(0, index);
        index = items.LastIndexOf(x007); Assert.Equal(3, index);
        list = items.IndexesOf(x007);
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        index = items.IndexOf(x008); Assert.Equal(0, index);
        index = items.IndexOf(x008, true); Assert.Equal(-1, index);
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

        var target = source.GetRange(0, 0);
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

        var target = source.Replace(1, xBond);
        Assert.NotSame(source, target);
        Assert.Same(x007, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(xBond, target[2]);

        target = source.Replace(0, new TItem("Id", "value"));
        Assert.NotSame(source, target);
        Assert.Equal("value", target[0].Value);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);

        try { _ = source.Replace(1, new TItem("Id", "value")); Assert.Fail(); }
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
        var source = new THost(engine, [x007, xJames, xBond]);

        var target = source.Remove(xJames);
        Assert.NotSame(source, target);
        Assert.Same(x007, target[0]);
        Assert.Same(xBond, target[1]);

        target = source.Remove(new TItem("FirstName", "..."));
        Assert.NotSame(source, target);
        Assert.Same(x007, target[0]);
        Assert.Same(xBond, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Duplicated()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, [x007, xJames, xBond, x007]);

        var target = source.Remove(new TItem("Id", "..."));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);

        target = source.RemoveLast(new TItem("Id", "..."));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);

        target = source.RemoveAll(new TItem("Id", "..."));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
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