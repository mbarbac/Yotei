namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public class Test_Identifier
{
    //[Enforced]
    [Fact]
    public static void Test_Parts_NoTerminator_Empty()
    {
        List<string?> items;
        var engine = new FakeEngine();

        items = Identifier.GetParts(engine, null); Assert.Empty(items);
        items = Identifier.GetParts(engine, ""); Assert.Empty(items);
        items = Identifier.GetParts(engine, "  "); Assert.Empty(items);

        items = Identifier.GetParts(engine, ".."); Assert.Empty(items);

        items = Identifier.GetParts(engine, "..", reduce: false);
        Assert.Equal(3, items.Count);
        Assert.Null(items[0]);
        Assert.Null(items[1]);
        Assert.Null(items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Parts_NoTerminator_Populated()
    {
        List<string?> items;
        var engine = new FakeEngine();

        items = Identifier.GetParts(engine, "one.two.three");
        Assert.Equal(3, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Equal("two", items[1]);
        Assert.Equal("three", items[2]);

        items = Identifier.GetParts(engine, " . . one . . three . . ");
        Assert.Equal(5, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Null(items[1]);
        Assert.Equal("three", items[2]);
        Assert.Null(items[3]);
        Assert.Null(items[4]);

        items = Identifier.GetParts(engine, " . . one . . three . . ", reduce: false);
        Assert.Equal(7, items.Count);
        Assert.Null(items[0]);
        Assert.Null(items[1]);
        Assert.Equal("one", items[2]);
        Assert.Null(items[3]);
        Assert.Equal("three", items[4]);
        Assert.Null(items[5]);
        Assert.Null(items[6]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Parts_WithTerminator_Empty()
    {
        List<string?> items;
        var engine = new FakeEngine();

        items = Identifier.GetParts(engine, "[]"); Assert.Empty(items);
        items = Identifier.GetParts(engine, "[[]]"); Assert.Empty(items);

        items = Identifier.GetParts(engine, " . [] . [ ] "); Assert.Empty(items);

        items = Identifier.GetParts(engine, " . [] . [ ] ", reduce: false);
        Assert.Equal(3, items.Count);
        Assert.Null(items[0]);
        Assert.Null(items[1]);
        Assert.Null(items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Parts_WithTerminator_Populated()
    {
        List<string?> items;
        var engine = new FakeEngine();

        items = Identifier.GetParts(engine, " [ [ one ] ] ");
        Assert.Single(items);
        Assert.Equal("one", items[0]);

        items = Identifier.GetParts(engine, " [ one two ] ");
        Assert.Single(items);
        Assert.Equal("one two", items[0]);

        items = Identifier.GetParts(engine, " [ one . two ] ");
        Assert.Single(items);
        Assert.Equal("one . two", items[0]);

        items = Identifier.GetParts(engine, " [ one ] . [ two ] . [ three ] ");
        Assert.Equal(3, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Equal("two", items[1]);
        Assert.Equal("three", items[2]);

        items = Identifier.GetParts(engine, " . . [ one ] . . [ three ] . . ");
        Assert.Equal(5, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Null(items[1]);
        Assert.Equal("three", items[2]);
        Assert.Null(items[3]);
        Assert.Null(items[4]);

        items = Identifier.GetParts(engine, " . . [ one ] . . [ three ] . . ", reduce: false);
        Assert.Equal(7, items.Count);
        Assert.Null(items[0]);
        Assert.Null(items[1]);
        Assert.Equal("one", items[2]);
        Assert.Null(items[3]);
        Assert.Equal("three", items[4]);
        Assert.Null(items[5]);
        Assert.Null(items[6]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Parts_SameTerminator_Empty()
    {
        List<string?> items;
        var ch = '-';
        var engine = new FakeEngine() { LeftTerminator = ch, RightTerminator = ch };

        items = Identifier.GetParts(engine, "--"); Assert.Empty(items);
        items = Identifier.GetParts(engine, "----"); Assert.Empty(items);

        items = Identifier.GetParts(engine, " . -- . - - "); Assert.Empty(items);

        items = Identifier.GetParts(engine, " . -- . - - ", reduce: false);
        Assert.Equal(3, items.Count);
        Assert.Null(items[0]);
        Assert.Null(items[1]);
        Assert.Null(items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Parts_SameTerminator_Populated()
    {
        List<string?> items;
        var ch = '-';
        var engine = new FakeEngine() { LeftTerminator = ch, RightTerminator = ch };

        items = Identifier.GetParts(engine, " - - one - - ");
        Assert.Single(items);
        Assert.Equal("one", items[0]);

        items = Identifier.GetParts(engine, " - one two - ");
        Assert.Single(items);
        Assert.Equal("one two", items[0]);

        items = Identifier.GetParts(engine, " - one . two - ");
        Assert.Single(items);
        Assert.Equal("one . two", items[0]);

        items = Identifier.GetParts(engine, " - one - . - two - . - three - ");
        Assert.Equal(3, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Equal("two", items[1]);
        Assert.Equal("three", items[2]);

        items = Identifier.GetParts(engine, " . . - one - . . - three - . . ");
        Assert.Equal(5, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Null(items[1]);
        Assert.Equal("three", items[2]);
        Assert.Null(items[3]);
        Assert.Null(items[4]);

        items = Identifier.GetParts(engine, " . . - one - . . - three - . . ", reduce: false);
        Assert.Equal(7, items.Count);
        Assert.Null(items[0]);
        Assert.Null(items[1]);
        Assert.Equal("one", items[2]);
        Assert.Null(items[3]);
        Assert.Equal("three", items[4]);
        Assert.Null(items[5]);
        Assert.Null(items[6]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Parts_WithTerminator_Complex()
    {
        List<string?> items;
        var engine = new FakeEngine();

        items = Identifier.GetParts(engine, "[one.[two]].[three]");
        Assert.Equal(2, items.Count);
        Assert.Equal("one.[two]", items[0]);
        Assert.Equal("three", items[1]);
    }
}