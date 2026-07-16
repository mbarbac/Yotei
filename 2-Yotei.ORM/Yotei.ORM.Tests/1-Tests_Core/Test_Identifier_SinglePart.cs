#pragma warning disable CA1859

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Identifier_SinglePart
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine();

        item = new Identifier(engine, [null]);
        Assert.Equal(0, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(string.Empty, item.ToStringEx(reduce: false));
        Assert.Empty(item.Enumerate());

        item = new Identifier(engine, [null], reduce: false);
        Assert.Equal(1, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(string.Empty, item.ToStringEx(reduce: false));
        Assert.Single(item.Enumerate());

        item = new Identifier(engine, "");
        Assert.Equal(0, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(string.Empty, item.ToStringEx(reduce: false));
        Assert.Empty(item.Enumerate());

        item = new Identifier(engine, "", reduce: false);
        Assert.Equal(1, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(string.Empty, item.ToStringEx(reduce: false));
        Assert.Single(item.Enumerate());

        item = new Identifier(engine, " ");
        Assert.Equal(0, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(string.Empty, item.ToStringEx(reduce: false));
        Assert.Empty(item.Enumerate());

        item = new Identifier(engine, " ", reduce: false);
        Assert.Equal(1, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(string.Empty, item.ToStringEx(reduce: false));
        Assert.Single(item.Enumerate());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_Different_Terminators()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine();

        item = new Identifier(engine, " [ ] ");
        Assert.Equal(0, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(string.Empty, item.ToStringEx(reduce: false));
        Assert.Empty(item.Enumerate());

        item = new Identifier(engine, " [ ]", reduce: false);
        Assert.Equal(1, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(string.Empty, item.ToStringEx(reduce: false));
        Assert.Single(item.Enumerate());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_Same_Terminators()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine() { LeftTerminator = '-', RightTerminator = '-' };

        item = new Identifier(engine, " - - ");
        Assert.Equal(0, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(string.Empty, item.ToStringEx(reduce: false));
        Assert.Empty(item.Enumerate());

        item = new Identifier(engine, " - - ", reduce: false);
        Assert.Equal(1, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(string.Empty, item.ToStringEx(reduce: false));
        Assert.Single(item.Enumerate());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_No_Terminators()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine();

        item = new Identifier(engine, " aa ");
        Assert.Equal(1, item.Count);
        Assert.Equal("[aa]", item.Value);
        Assert.Equal("[aa]", item.ToStringEx(reduce: false));
        Assert.Single(item.Enumerate());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Different_Terminators()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine();

        item = new Identifier(engine, " [ aa ]");
        Assert.Equal(1, item.Count);
        Assert.Equal("[aa]", item.Value);
        Assert.Equal("[aa]", item.ToStringEx(reduce: false));
        Assert.Single(item.Enumerate());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Same_Terminators()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine() { LeftTerminator = '-', RightTerminator = '-' };

        item = new Identifier(engine, " - aa - ");
        Assert.Equal(1, item.Count);
        Assert.Equal("-aa-", item.Value);
        Assert.Equal("-aa-", item.ToStringEx(reduce: false));
        Assert.Single(item.Enumerate());
    }
}