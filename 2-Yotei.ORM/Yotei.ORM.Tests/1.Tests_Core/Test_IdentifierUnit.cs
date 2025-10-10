namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_IdentifierUnit
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Same_Terminators_Empty()
    {
        IdentifierUnit item;
        var ch = '\'';
        var engine = new FakeEngine() { LeftTerminator = ch, RightTerminator = ch };

        item = new IdentifierUnit(engine, "''");
        Assert.Null(item.RawValue);
        Assert.Null(item.Value);

        item = new IdentifierUnit(engine, " ' ' ");
        Assert.Null(item.RawValue);
        Assert.Null(item.Value);

        item = new IdentifierUnit(engine, " ' ' . ' ' ");
        Assert.Null(item.RawValue);
        Assert.Null(item.Value);

        try { _ = new IdentifierUnit(engine, " ' . ' "); Assert.Fail(); } // No only dots
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Same_Terminators_Populated()
    {
        IdentifierUnit item;
        var ch = '\'';
        var engine = new FakeEngine() { LeftTerminator = ch, RightTerminator = ch };

        item = new IdentifierUnit(engine, " ' aa ' ");
        Assert.Equal("aa", item.RawValue);
        Assert.Equal("'aa'", item.Value);

        item = new IdentifierUnit(engine, " ' ' aa ' ' ");
        Assert.Equal("aa", item.RawValue);
        Assert.Equal("'aa'", item.Value);

        item = new IdentifierUnit(engine, " ' aa bb ' ");
        Assert.Equal("aa bb", item.RawValue);
        Assert.Equal("'aa bb'", item.Value);

        item = new IdentifierUnit(engine, " ' aa.bb ' ");
        Assert.Equal("aa.bb", item.RawValue);
        Assert.Equal("'aa.bb'", item.Value);

        item = new IdentifierUnit(engine, "'aa.'bb''");
        Assert.Equal("aa.'bb'", item.RawValue);
        Assert.Equal("'aa.'bb''", item.Value);

        try { _ = new IdentifierUnit(engine, "aa.bb"); Assert.Fail(); } // Many parts
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Distinct_Terminators_Empty()
    {
        IdentifierUnit item;
        var engine = new FakeEngine();

        item = new IdentifierUnit(engine);
        Assert.Null(item.RawValue);
        Assert.Null(item.Value);

        item = new IdentifierUnit(engine, "");
        Assert.Null(item.RawValue);
        Assert.Null(item.Value);

        item = new IdentifierUnit(engine, " ");
        Assert.Null(item.RawValue);
        Assert.Null(item.Value);

        item = new IdentifierUnit(engine, " [ ] . [ ] ");
        Assert.Null(item.RawValue);
        Assert.Null(item.Value);

        try { _ = new IdentifierUnit(engine, " [ . ] "); Assert.Fail(); } // No only dots
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Distinct_Terminators_Populated()
    {
        IdentifierUnit item;
        var engine = new FakeEngine();

        item = new IdentifierUnit(engine, " [ aa ] ");
        Assert.Equal("aa", item.RawValue);
        Assert.Equal("[aa]", item.Value);

        item = new IdentifierUnit(engine, " [ [ aa ] ] ");
        Assert.Equal("aa", item.RawValue);
        Assert.Equal("[aa]", item.Value);

        item = new IdentifierUnit(engine, " [ aa bb ] ");
        Assert.Equal("aa bb", item.RawValue);
        Assert.Equal("[aa bb]", item.Value);

        item = new IdentifierUnit(engine, " [ aa.bb ] ");
        Assert.Equal("aa.bb", item.RawValue);
        Assert.Equal("[aa.bb]", item.Value);

        item = new IdentifierUnit(engine, "[aa.[bb]] ");
        Assert.Equal("aa.[bb]", item.RawValue);
        Assert.Equal("[aa.[bb]]", item.Value);

        try {  _ = new IdentifierUnit(engine, "aa.bb"); Assert.Fail(); } // Many parts
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Compare()
    {
        var xengine = new FakeEngine() { CaseSensitiveNames = false };
        var xitem = new IdentifierUnit(xengine, "one");
        var yitem = new IdentifierUnit(xengine, "ONE");
        Assert.NotSame(xitem, yitem);
        Assert.Equal(xitem, yitem);
        Assert.True(xitem == yitem);

        var yengine = new FakeEngine() { CaseSensitiveNames = true };
        yitem = new IdentifierUnit(yengine, "ONE");
        Assert.Equal(xitem, yitem);
        Assert.True(xitem == yitem);

        Assert.NotEqual(yitem, xitem);
        Assert.False(yitem == xitem);
    }
}