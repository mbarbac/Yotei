namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_DbTokenVisitor_Tokens
{
    //[Enforced]
    [Fact]
    public static void Test_Argument()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.InvariantCulture);
        var visitor = new DbTokenVisitor(connection, locale);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x);
        Assert.Empty(builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Binary_Nulls_Standard()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.InvariantCulture);
        var visitor = new DbTokenVisitor(connection, locale);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Alpha & null);
        Assert.Equal("([Alpha] AND NULL)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Binary_Nulls_Intercepted()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.InvariantCulture);
        var visitor = new DbTokenVisitor(connection, locale);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Alpha == null);
        Assert.Equal("([Alpha] IS NULL)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Alpha != null);
        Assert.Equal("([Alpha] IS NOT NULL)", builder.Text);
        Assert.Empty(builder.Parameters);

        visitor = visitor with { UseNullString = false };
        builder = visitor.Visit(x => x.Alpha != null);
        Assert.Equal("([Alpha] <> #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Null(builder.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        builder = visitor.Visit(x => x.Alpha != null);
        Assert.Equal("([Alpha] <> '')", builder.Text);
        Assert.Empty(builder.Parameters);

        visitor = visitor with { UseQuotes = false };
        builder = visitor.Visit(x => x.Alpha != null);
        Assert.Equal("([Alpha] <> )", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Binary_Values()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.InvariantCulture);
        var visitor = new DbTokenVisitor(connection, locale);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Alpha >= "any");
        Assert.Equal("([Alpha] >= #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("any", builder.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        builder = visitor.Visit(x => x.Alpha >= "any");
        Assert.Equal("([Alpha] >= 'any')", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Binary_Dynamic()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.InvariantCulture);
        var visitor = new DbTokenVisitor(connection, locale);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Alpha && x.Beta);
        Assert.Equal("([Alpha] AND [Beta])", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Alpha || x);
        Assert.Equal("([Alpha] OR )", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Chain()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.InvariantCulture);
        var visitor = new DbTokenVisitor(connection, locale);

        ICommandInfo.IBuilder builder; var token = new DbTokenChain([
            new DbTokenValue("Hello"),
            new DbTokenLiteral(" world!")]);

        builder = visitor.Visit(token);
        Assert.Equal("#0 world!", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name);
        Assert.Equal("Hello", builder.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        builder = visitor.Visit(token);
        Assert.Equal("'Hello' world!", builder.Text);
        Assert.Empty(builder.Parameters);

        visitor = visitor with { UseQuotes = false };
        builder = visitor.Visit(token);
        Assert.Equal("Hello world!", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Coalesce()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.InvariantCulture);
        var visitor = new DbTokenVisitor(connection, locale);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Coalesce(x.Alpha, x.Beta));
        Assert.Equal("COALESCE([Alpha], [Beta])", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Coalesce_SpecialCase()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.InvariantCulture);
        var visitor = new DbTokenVisitor(connection, locale);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Coalesce(null, x.Alpha));
        Assert.Equal("[Alpha]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Command_Direct()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.InvariantCulture);
        var visitor = new DbTokenVisitor(connection, locale);
        ICommandInfo.IBuilder builder;

        var command = new FakeCommand(connection,
            "SELECT * FROM Emps WHERE LastName = {0} AND Id = {1}",
            null, "007");

        builder = visitor.Visit(x => command);
        Assert.Equal("(SELECT * FROM Emps WHERE LastName = NULL AND Id = #1)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    ////[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var connection = new FakeConnection(engine);
    //    var locale = new Locale(CultureInfo.InvariantCulture);
    //    var visitor = new DbTokenVisitor(connection, locale);
    //    ICommandInfo.IBuilder builder;

    //    builder = visitor.Visit(x => 
    //    Assert.Equal("", builder.Text);
    //    Assert.Empty(builder.Parameters);
    //}

    // ----------------------------------------------------

    ////[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var connection = new FakeConnection(engine);
    //    var locale = new Locale(CultureInfo.InvariantCulture);
    //    var visitor = new DbTokenVisitor(connection, locale);
    //    ICommandInfo.IBuilder builder;

    //    builder = visitor.Visit(x => 
    //    Assert.Equal("", builder.Text);
    //    Assert.Empty(builder.Parameters);
    //}

    // ----------------------------------------------------

    ////[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var connection = new FakeConnection(engine);
    //    var locale = new Locale(CultureInfo.InvariantCulture);
    //    var visitor = new DbTokenVisitor(connection, locale);
    //    ICommandInfo.IBuilder builder;

    //    builder = visitor.Visit(x => 
    //    Assert.Equal("", builder.Text);
    //    Assert.Empty(builder.Parameters);
    //}

    // ----------------------------------------------------

    ////[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var connection = new FakeConnection(engine);
    //    var locale = new Locale(CultureInfo.InvariantCulture);
    //    var visitor = new DbTokenVisitor(connection, locale);
    //    ICommandInfo.IBuilder builder;

    //    builder = visitor.Visit(x => 
    //    Assert.Equal("", builder.Text);
    //    Assert.Empty(builder.Parameters);
    //}

    // ----------------------------------------------------

    ////[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var connection = new FakeConnection(engine);
    //    var locale = new Locale(CultureInfo.InvariantCulture);
    //    var visitor = new DbTokenVisitor(connection, locale);
    //    ICommandInfo.IBuilder builder;

    //    builder = visitor.Visit(x => 
    //    Assert.Equal("", builder.Text);
    //    Assert.Empty(builder.Parameters);
    //}

    // ----------------------------------------------------

    ////[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var connection = new FakeConnection(engine);
    //    var locale = new Locale(CultureInfo.InvariantCulture);
    //    var visitor = new DbTokenVisitor(connection, locale);
    //    ICommandInfo.IBuilder builder;

    //    builder = visitor.Visit(x => 
    //    Assert.Equal("", builder.Text);
    //    Assert.Empty(builder.Parameters);
    //}
}