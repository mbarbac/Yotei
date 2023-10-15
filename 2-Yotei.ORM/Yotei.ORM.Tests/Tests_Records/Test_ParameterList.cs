using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

using THost = Yotei.ORM.Records.ParameterList;
using TItem = Yotei.ORM.Records.Parameter;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_ParameterList
{
    readonly static TItem x007 = new("Id", "007");
    readonly static TItem xJames = new("FirstName", "James");
    readonly static TItem xBond = new("LastName", "Bond");
    readonly static TItem xMi6 = new("Company", "MI6");

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
        var items = new THost(engine, new[] { x007, xJames, xBond });
        Assert.Equal(3, items.Count);
        Assert.Equal("007", items[0].Value);
        Assert.Equal("James", items[1].Value);
        Assert.Equal("Bond", items[2].Value);

        try { _ = new THost(engine, (IEnumerable<TItem>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new THost(engine, new[] { x007, new("ID", "008") }); Assert.Fail(); }
        catch (DuplicateException) { }
    }

}