#pragma warning disable CA1859

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Identifier_MultiPart
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine();

        item = new Identifier(engine, [null, null]);
        Assert.Equal(0, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(string.Empty, item.ToStringEx(reduce: false));
        Assert.Empty(item.Enumerate());

        item = new Identifier(engine, [null, null], reduce: false);
        Assert.Equal(2, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(".", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());

        item = new Identifier(engine, ["", ""]);
        Assert.Equal(0, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(string.Empty, item.ToStringEx(reduce: false));
        Assert.Empty(item.Enumerate());

        item = new Identifier(engine, ["", ""], reduce: false);
        Assert.Equal(2, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(".", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());

        item = new Identifier(engine, [" ", " "]);
        Assert.Equal(0, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(string.Empty, item.ToStringEx(reduce: false));
        Assert.Empty(item.Enumerate());

        item = new Identifier(engine, [" ", " "], reduce: false);
        Assert.Equal(2, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(".", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_Different_Terminators()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine();

        item = new Identifier(engine, [" [ ] ", " [ ] "]);
        Assert.Equal(0, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(string.Empty, item.ToStringEx(reduce: false));
        Assert.Empty(item.Enumerate());

        item = new Identifier(engine, [" [ ] ", " [ ] "], reduce: false);
        Assert.Equal(2, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(".", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_Different_Terminators_Split()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine();

        item = new Identifier(engine, [" [ ] . [ ] "], reduce: false);
        Assert.Equal(2, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(".", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_Same_Terminators()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine() { LeftTerminator = '-', RightTerminator = '-' };

        item = new Identifier(engine, [" - - ", " - - "]);
        Assert.Equal(0, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(string.Empty, item.ToStringEx(reduce: false));
        Assert.Empty(item.Enumerate());

        item = new Identifier(engine, [" - - ", " - - "], reduce: false);
        Assert.Equal(2, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(".", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_Same_Terminators_Split()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine() { LeftTerminator = '-', RightTerminator = '-' };

        item = new Identifier(engine, [" - - . - - "], reduce: false);
        Assert.Equal(2, item.Count);
        Assert.Null(item.Value);
        Assert.Equal(".", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_No_Terminators()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine();

        item = new Identifier(engine, [" aa ", " bb "]);
        Assert.Equal(2, item.Count);
        Assert.Equal("[aa].[bb]", item.Value);
        Assert.Equal("[aa].[bb]", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());

        item = new Identifier(engine, [" aa ", null, " bb ", null]);
        Assert.Equal(4, item.Count);
        Assert.Equal("[aa]..[bb].", item.Value);
        Assert.Equal("[aa]..[bb].", item.ToStringEx(reduce: false));
        Assert.Equal(4, item.Enumerate().Count());

        item = new Identifier(engine, [null, null, " aa ", " bb "]);
        Assert.Equal(2, item.Count);
        Assert.Equal("[aa].[bb]", item.Value);
        Assert.Equal("[aa].[bb]", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());

        item = new Identifier(engine, [null, null, " aa ", " bb "], reduce: false);
        Assert.Equal(4, item.Count);
        Assert.Equal("[aa].[bb]", item.Value);
        Assert.Equal("..[aa].[bb]", item.ToStringEx(reduce: false));
        Assert.Equal(4, item.Enumerate().Count());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_No_Terminators_Split()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine();

        item = new Identifier(engine, [" aa . bb "]);
        Assert.Equal(2, item.Count);
        Assert.Equal("[aa].[bb]", item.Value);
        Assert.Equal("[aa].[bb]", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());

        item = new Identifier(engine, [" aa . ", " bb . "]);
        Assert.Equal(4, item.Count);
        Assert.Equal("[aa]..[bb].", item.Value);
        Assert.Equal("[aa]..[bb].", item.ToStringEx(reduce: false));
        Assert.Equal(4, item.Enumerate().Count());

        item = new Identifier(engine, [" . . aa ", " bb "]);
        Assert.Equal(2, item.Count);
        Assert.Equal("[aa].[bb]", item.Value);
        Assert.Equal("[aa].[bb]", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());

        item = new Identifier(engine, [" . . aa ", " bb "], reduce: false);
        Assert.Equal(4, item.Count);
        Assert.Equal("[aa].[bb]", item.Value);
        Assert.Equal("..[aa].[bb]", item.ToStringEx(reduce: false));
        Assert.Equal(4, item.Enumerate().Count());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Different_Terminators()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine();

        item = new Identifier(engine, [" [ aa ] ", " [ bb ] "]);
        Assert.Equal(2, item.Count);
        Assert.Equal("[aa].[bb]", item.Value);
        Assert.Equal("[aa].[bb]", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());

        item = new Identifier(engine, [" [ aa ] ", null, " [ bb ] ", null]);
        Assert.Equal(4, item.Count);
        Assert.Equal("[aa]..[bb].", item.Value);
        Assert.Equal("[aa]..[bb].", item.ToStringEx(reduce: false));
        Assert.Equal(4, item.Enumerate().Count());

        item = new Identifier(engine, [null, null, " [ aa ] ", " [ bb ] "]);
        Assert.Equal(2, item.Count);
        Assert.Equal("[aa].[bb]", item.Value);
        Assert.Equal("[aa].[bb]", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());

        item = new Identifier(engine, [null, null, " [ aa ] ", " [ bb ] "], reduce: false);
        Assert.Equal(4, item.Count);
        Assert.Equal("[aa].[bb]", item.Value);
        Assert.Equal("..[aa].[bb]", item.ToStringEx(reduce: false));
        Assert.Equal(4, item.Enumerate().Count());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Different_Terminators_Split()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine();

        item = new Identifier(engine, [" [ aa ] . [ bb ] ", " [ cc ] "]);
        Assert.Equal(3, item.Count);
        Assert.Equal("[aa].[bb].[cc]", item.Value);
        Assert.Equal("[aa].[bb].[cc]", item.ToStringEx(reduce: false));
        Assert.Equal(3, item.Enumerate().Count());

        item = new Identifier(engine, [" [ aa ] . . [ bb ] . ", " [ cc ] . "]);
        Assert.Equal(6, item.Count);
        Assert.Equal("[aa]..[bb]..[cc].", item.Value);
        Assert.Equal("[aa]..[bb]..[cc].", item.ToStringEx(reduce: false));
        Assert.Equal(6, item.Enumerate().Count());

        item = new Identifier(engine, [" . [ aa ] . . [ bb ] . ", " [ cc ] . "]);
        Assert.Equal(6, item.Count);
        Assert.Equal("[aa]..[bb]..[cc].", item.Value);
        Assert.Equal("[aa]..[bb]..[cc].", item.ToStringEx(reduce: false));
        Assert.Equal(6, item.Enumerate().Count());

        item = new Identifier(engine, [" . [ aa ] . . [ bb ] . ", " [ cc ] . "], reduce: false);
        Assert.Equal(7, item.Count);
        Assert.Equal("[aa]..[bb]..[cc].", item.Value);
        Assert.Equal(".[aa]..[bb]..[cc].", item.ToStringEx(reduce: false));
        Assert.Equal(7, item.Enumerate().Count());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Same_Terminators()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine() { LeftTerminator = '-', RightTerminator = '-' };

        item = new Identifier(engine, [" - aa - ", " - bb - "]);
        Assert.Equal(2, item.Count);
        Assert.Equal("-aa-.-bb-", item.Value);
        Assert.Equal("-aa-.-bb-", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());

        item = new Identifier(engine, [" - aa - ", null, " - bb - ", null]);
        Assert.Equal(4, item.Count);
        Assert.Equal("-aa-..-bb-.", item.Value);
        Assert.Equal("-aa-..-bb-.", item.ToStringEx(reduce: false));
        Assert.Equal(4, item.Enumerate().Count());

        item = new Identifier(engine, [null, null, " - aa - ", " - bb - "]);
        Assert.Equal(2, item.Count);
        Assert.Equal("-aa-.-bb-", item.Value);
        Assert.Equal("-aa-.-bb-", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());

        item = new Identifier(engine, [null, null, " - aa - ", " - bb - "], reduce: false);
        Assert.Equal(4, item.Count);
        Assert.Equal("-aa-.-bb-", item.Value);
        Assert.Equal("..-aa-.-bb-", item.ToStringEx(reduce: false));
        Assert.Equal(4, item.Enumerate().Count());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Same_Terminators_Split()
    {
        IIdentifier item;
        IEngine engine = new FakeEngine() { LeftTerminator = '-', RightTerminator = '-' };

        item = new Identifier(engine, [" - aa - . - bb - "]);
        Assert.Equal(2, item.Count);
        Assert.Equal("-aa-.-bb-", item.Value);
        Assert.Equal("-aa-.-bb-", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());

        item = new Identifier(engine, [" - aa - . - bb - ", null]);
        Assert.Equal(3, item.Count);
        Assert.Equal("-aa-.-bb-.", item.Value);
        Assert.Equal("-aa-.-bb-.", item.ToStringEx(reduce: false));
        Assert.Equal(3, item.Enumerate().Count());

        item = new Identifier(engine, [" . - aa - ", " - bb - "]);
        Assert.Equal(2, item.Count);
        Assert.Equal("-aa-.-bb-", item.Value);
        Assert.Equal("-aa-.-bb-", item.ToStringEx(reduce: false));
        Assert.Equal(2, item.Enumerate().Count());

        item = new Identifier(engine, [" . . - aa - ", " - bb - "], reduce: false);
        Assert.Equal(4, item.Count);
        Assert.Equal("-aa-.-bb-", item.Value);
        Assert.Equal("..-aa-.-bb-", item.ToStringEx(reduce: false));
        Assert.Equal(4, item.Enumerate().Count());
    }
}