namespace Yotei.ORM.Tests.Internals.DbTokens;

// ========================================================
//[Enforced]
public static class Test_DbTokenVisitor_Values
{
    //[Enforced]
    [Fact]
    public static void Test_Null()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection, new());
        string str;

        str = visitor.ToValueString(null);
        Assert.Equal(engine.NullValueLiteral, str);

        visitor = visitor with { UseNullString = false };
        str = visitor.ToValueString(null);
        Assert.Equal("''", str);

        visitor = visitor with { UseQuotes = false };
        str = visitor.ToValueString(null);
        Assert.Empty(str);

    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Boolean()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection, new());
        string str;

        str = visitor.ToValueString(true);
        Assert.Equal("TRUE", str);

        str = visitor.ToValueString(false);
        Assert.Equal("FALSE", str);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Decimal()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection, new());
        string str;
        var value = new decimal(1234.56);

        visitor = visitor with { Locale = new Locale(CultureInfo.InvariantCulture) };
        str = visitor.ToValueString(value);
        Assert.Equal("1234.56", str);

        visitor = visitor with { Locale = new Locale(CultureInfo.GetCultureInfo("es-ES")) };
        str = visitor.ToValueString(value);
        Assert.Equal("1234,56", str);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_DateTime()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection, new()) { ConvertValues = false };
        string str;
        var value = new DateTime(2001, 12, 31);

        visitor = visitor with { Locale = new Locale(CultureInfo.InvariantCulture) };
        str = visitor.ToValueString(value);
        Assert.Equal("'12/31/2001 00:00:00'", str);

        visitor = visitor with { Locale = new Locale(CultureInfo.GetCultureInfo("es-ES")) };
        str = visitor.ToValueString(value);
        Assert.Equal("'31/12/2001 0:00:00'", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_DateOnly()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection, new()) { ConvertValues = false };
        string str;
        var value = new DateOnly(2001, 12, 31);

        visitor = visitor with { Locale = new Locale(CultureInfo.InvariantCulture) };
        str = visitor.ToValueString(value);
        Assert.Equal("'12/31/2001'", str);

        visitor = visitor with { Locale = new Locale(CultureInfo.GetCultureInfo("es-ES")) };
        str = visitor.ToValueString(value);
        Assert.Equal("'31/12/2001'", str);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_TimeOnly()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection, new());
        string str;
        var value = new TimeOnly(23, 55, 59, 800, 900);

        visitor = visitor with { Locale = new Locale(CultureInfo.InvariantCulture) };
        str = visitor.ToValueString(value);
        Assert.Equal("'23:55'", str);

        visitor = visitor with { Locale = new Locale(CultureInfo.GetCultureInfo("es-ES")) };
        str = visitor.ToValueString(value);
        Assert.Equal("'23:55'", str);
    }
}