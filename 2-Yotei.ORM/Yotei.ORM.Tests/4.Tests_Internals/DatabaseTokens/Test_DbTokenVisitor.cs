using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_DbTokenVisitor
{
    //[Enforced]
    [Fact]
    public static void Test_Argument()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x);
        Assert.Empty(item.Text);
        Assert.Empty(item.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Binary_Nulls_Standard()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Alpha & null);
        Assert.Equal("([Alpha] AND NULL)", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Binary_Nulls_Intercepted()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Alpha == null);
        Assert.Equal("([Alpha] IS NULL)", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Alpha != null);
        Assert.Equal("([Alpha] IS NOT NULL)", item.Text);
        Assert.Empty(item.Parameters);

        visitor = visitor with { UseNullString = false };
        item = visitor.Visit(x => x.Alpha != null);
        Assert.Equal("([Alpha] <> #0)", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Null(item.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        item = visitor.Visit(x => x.Alpha != null);
        Assert.Equal("([Alpha] <> '')", item.Text);
        Assert.Empty(item.Parameters);

        visitor = visitor with { UseQuotes = false };
        item = visitor.Visit(x => x.Alpha != null);
        Assert.Equal("([Alpha] <> )", item.Text);
        Assert.Empty(item.Parameters);
    }


    //[Enforced]
    [Fact]
    public static void Test_Binary_Values()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Alpha >= "any");
        Assert.Equal("([Alpha] >= #0)", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal("any", item.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        item = visitor.Visit(x => x.Alpha >= "any");
        Assert.Equal("([Alpha] >= 'any')", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Binary_Dynamic()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Alpha && x.Beta);
        Assert.Equal("([Alpha] AND [Beta])", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Alpha || x);
        Assert.Equal("([Alpha] OR )", item.Text);
        Assert.Empty(item.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Chain()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        var token = new DbTokenChain([
            new DbTokenValue("Hello"),
            new DbTokenLiteral(" world!")]);

        item = visitor.VisitRange(token);
        Assert.Equal("#0 world!", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal("Hello", item.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        item = visitor.VisitRange(token);
        Assert.Equal("'Hello' world!", item.Text);
        Assert.Empty(item.Parameters);

        visitor = visitor with { UseQuotes = false };
        item = visitor.VisitRange(token);
        Assert.Equal("Hello world!", item.Text);
        Assert.Empty(item.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Coalesce()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Coalesce(x.Alpha, x.Beta));
        Assert.Equal("COALESCE([Alpha], [Beta])", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Coalesce_SpecialCase()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Coalesce(null, x.Alpha));
        Assert.Equal("[Alpha]", item.Text);
        Assert.Empty(item.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Command_Direct()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        var command = new FakeCommand(connection,
            "SELECT * FROM [Emp] WHERE [LastName] = {0}",
            null!);

        item = visitor.Visit(x => command);
        Assert.Equal("(SELECT * FROM [Emp] WHERE [LastName] = #0)", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Null(item.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Command_Invoke()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        var command = new FakeCommand(connection,
            "SELECT * FROM [Emp] WHERE [LastName] = {0}",
            null!);

        item = visitor.Visit(x => x(command));
        Assert.Equal("(SELECT * FROM [Emp] WHERE [LastName] = #0)", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Null(item.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Command_Invoke_WithAlias()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        var command = new FakeCommand(connection,
            "SELECT * FROM [Emp] WHERE [LastName] = {0}",
            null!);

        item = visitor.Visit(x => x(command).As(x.Any));
        Assert.Equal("(SELECT * FROM [Emp] WHERE [LastName] = #0) AS [Any]", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Null(item.Parameters[0].Value);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Identifier_Standard()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Alpha);
        Assert.Equal("[Alpha]", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Alpha.Beta);
        Assert.Equal("[Alpha].[Beta]", item.Text);
        Assert.Empty(item.Parameters);

        visitor = visitor with { UseTerminators = false };
        item = visitor.Visit(x => x.Alpha.Beta);
        Assert.Equal("Alpha.Beta", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Identifier_Dynamic()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.x.x.x);
        Assert.Empty(item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.x.x.Alpha);
        Assert.Equal("[Alpha]", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Alpha.x);
        Assert.Equal("[Alpha].", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Alpha.x.x);
        Assert.Equal("[Alpha]..", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Alpha.x.Beta);
        Assert.Equal("[Alpha]..[Beta]", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Alpha.x.x.Beta);
        Assert.Equal("[Alpha]...[Beta]", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Identifier_UnPure()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => (x.Alpha + x.Beta).Delta);
        Assert.Equal("([Alpha] + [Beta]).[Delta]", item.Text);
        Assert.Empty(item.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Indexed_Argument()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x[27]);
        Assert.Equal("[#0]", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal(27, item.Parameters[0].Value);

        item = visitor.Visit(x => x[x.Alpha]);
        Assert.Equal("[[Alpha]]", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Indexed_Member()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Alpha[x.Beta, null, "any"]);
        Assert.Equal("[Alpha][[Beta], NULL, #0]", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal("any", item.Parameters[0].Value);

        item = visitor.Visit(x => x.Alpha.x[x.Beta, null, "any"]);
        Assert.Equal("[Alpha].[[Beta], NULL, #0]", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal("any", item.Parameters[0].Value);

        item = visitor.Visit(x => x.Alpha.x.x[x.Beta, null, "any"]);
        Assert.Equal("[Alpha]..[[Beta], NULL, #0]", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal("any", item.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Indexed_Nested()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Alpha[null, x[x.Alpha], x[x.Beta[x.Alpha]]]);
        Assert.Equal("[Alpha][NULL, [[Alpha]], [[Beta][[Alpha]]]]", item.Text);
        Assert.Empty(item.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Invoke_OnArgument()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x());
        Assert.Equal("", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x(50));
        Assert.Equal("#0", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal(50, item.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Invoke_ToLiteral()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x("50"));
        Assert.Equal("50", item.Text);
        Assert.Empty(item.Parameters); // Literal values are not captured...
    }

    //[Enforced]
    [Fact]
    public static void Test_Invoke_OnArgument_For_Chaining()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x(x.Alpha, 3, " = ", null));
        Assert.Equal("[Alpha]#0#1NULL", item.Text);
        Assert.Equal(2, item.Parameters.Count);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal(3, item.Parameters[0].Value);
        Assert.Equal("#1", item.Parameters[1].Name); Assert.Equal(" = ", item.Parameters[1].Value);

        item = visitor.Visit(x => x(x.Alpha, 3, x(" = "), null));
        Assert.Equal("[Alpha]#0 = NULL", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal(3, item.Parameters[0].Value);

        item = visitor.Visit(x => x(x.Alpha, x("3"), x(" = "), null));
        Assert.Equal("[Alpha]3 = NULL", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x(x.Alpha)(x.Beta)(x(" = "), null));
        Assert.Equal("[Alpha][Beta] = NULL", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Invoke_MethodAlike()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Alpha.x());
        Assert.Equal("[Alpha]", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Alpha.x(x(" >= "), 50));
        Assert.Equal("[Alpha] >= #0", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal(50, item.Parameters[0].Value);

        item = visitor.Visit(x => x.Alpha.x(x(), "any"));
        Assert.Equal("[Alpha]#0", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal("any", item.Parameters[0].Value);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x("any"));
        Assert.Equal("any", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.First.x("Name"));
        Assert.Equal("[First]Name", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x(x.First, x("Name")));
        Assert.Equal("[First]Name", item.Text);
        Assert.Empty(item.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Method_Regular_OnArgument()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Alpha());
        Assert.Equal("Alpha()", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Alpha(x.Beta, null, 99));
        Assert.Equal("Alpha([Beta], NULL, #0)", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal(99, item.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Regular_OnMember()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Alpha.Beta());
        Assert.Equal("[Alpha].Beta()", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Alpha.Beta(null, x.Delta, 45));
        Assert.Equal("[Alpha].Beta(NULL, [Delta], #0)", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal(45, item.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Regular_WithGenerics()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Alpha.Beta<string>(x.Delta, 50));
        Assert.Equal("[Alpha].Beta<String>([Delta], #0)", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal(50, item.Parameters[0].Value);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Method_Not()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Not(x.Id));
        Assert.Equal("(NOT [Id])", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Alpha.Not(x.Id));
        Assert.Equal("[Alpha].Not([Id])", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Count()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Count());
        Assert.Equal("COUNT(*)", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Count('*'));
        Assert.Equal("COUNT(*)", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Count("*"));
        Assert.Equal("COUNT(*)", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Count(x.Alpha, x.Beta));
        Assert.Equal("COUNT([Alpha], [Beta])", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Id.Count(x.Delta));
        Assert.Equal("[Id].Count([Delta])", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Cast()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Cast(x.Alpha, "DECIMAL(2,5)"));
        Assert.Equal("CAST([Alpha] AS DECIMAL(2,5))", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Cast<string>(x.Alpha));
        Assert.Equal("CAST([Alpha] AS String)", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Convert()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Convert("DECIMAL(2,5)", x.Alpha));
        Assert.Equal("CAST([Alpha] AS DECIMAL(2,5))", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Convert<string>(x.Alpha));
        Assert.Equal("CAST([Alpha] AS String)", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_As()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Id.As(x.Any));
        Assert.Equal("[Id] AS [Any]", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Id.As(x.Any, 33));
        Assert.Equal("[Id] AS [Any33]", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Id.As(x.Any, null, 50));
        Assert.Equal("[Id] AS [Any50]", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => x.Id.As(x.Any, " ", 50));
        Assert.Equal("[Id] AS [Any 50]", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_In()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Id.In("007", null, x.Alpha));
        Assert.Equal("[Id] IN (#0, NULL, [Alpha])", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal("007", item.Parameters[0].Value);

        item = visitor.Visit(x => x.Id.In(new object?[] { "007", null, x.Alpha }));
        Assert.Equal("[Id] IN (#0, NULL, [Alpha])", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal("007", item.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_NotIn()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Id.NotIn("007", null, x.Alpha));
        Assert.Equal("[Id] NOT IN (#0, NULL, [Alpha])", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal("007", item.Parameters[0].Value);

        item = visitor.Visit(x => x.Id.NotIn(new object?[] { "007", null, x.Alpha }));
        Assert.Equal("[Id] NOT IN (#0, NULL, [Alpha])", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal("007", item.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Between()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Id.Between(x.Alpha, 50));
        Assert.Equal("[Id] BETWEEN ([Alpha] AND #0)", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal(50, item.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Like()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Id.Like("00*"));
        Assert.Equal("[Id] LIKE #0", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal("00*", item.Parameters[0].Value);

        item = visitor.Visit(x => x.Id.Like(x("'00*'")));
        Assert.Equal("[Id] LIKE '00*'", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_NotLike()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Id.NotLike("00*"));
        Assert.Equal("[Id] NOT LIKE #0", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal("00*", item.Parameters[0].Value);

        item = visitor.Visit(x => x.Id.NotLike(x("'00*'")));
        Assert.Equal("[Id] NOT LIKE '00*'", item.Text);
        Assert.Empty(item.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Setter_OnArgument()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x = "007");
        Assert.Equal("#0", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal("007", item.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };

        item = visitor.Visit(x => x = "007");
        Assert.Equal("'007'", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Setter_OnMember()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Alpha = "007");
        Assert.Equal("([Alpha] = #0)", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal("007", item.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };

        item = visitor.Visit(x => x.Alpha = "007");
        Assert.Equal("([Alpha] = '007')", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Setter_Dynamic()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Alpha = x.Beta);
        Assert.Equal("([Alpha] = [Beta])", item.Text);
        Assert.Empty(item.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Ternary()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x.Ternary(x.Alpha, x.Beta, x.Delta));
        Assert.Equal("IF (([Alpha]) THEN ([Beta]) ELSE ([Delta]))", item.Text);
        Assert.Empty(item.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Unary_Dynamic()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => !x);
        Assert.Equal("(NOT )", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => -x);
        Assert.Equal("-()", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Unary_Standard()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => !x.Alpha);
        Assert.Equal("(NOT [Alpha])", item.Text);
        Assert.Empty(item.Parameters);

        item = visitor.Visit(x => -x.Alpha);
        Assert.Equal("-([Alpha])", item.Text);
        Assert.Empty(item.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Value_Nulls()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => null!);
        Assert.Equal("NULL", item.Text);
        Assert.Empty(item.Parameters);

        visitor = visitor with { UseNullString = false };
        item = visitor.Visit(x => null!);
        Assert.Equal("#0", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Null(item.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        item = visitor.Visit(x => null!);
        Assert.Equal("''", item.Text);
        Assert.Empty(item.Parameters);

        visitor = visitor with { UseQuotes = false };
        item = visitor.Visit(x => null!);
        Assert.Equal("", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Value_Strings()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => "any");
        Assert.Equal("#0", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal("any", item.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        item = visitor.Visit(x => "any");
        Assert.Equal("'any'", item.Text);
        Assert.Empty(item.Parameters);

        visitor = visitor with { UseQuotes = false };
        item = visitor.Visit(x => "any");
        Assert.Equal("any", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Value_Boolean()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => true);
        Assert.Equal("#0", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal(true, item.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        item = visitor.Visit(x => true);
        Assert.Equal("TRUE", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Value_Decimal_Invariant_Culture()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.InvariantCulture);
        var visitor = new DbTokenVisitor(connection) { Locale = locale };
        ICommandInfo.IBuilder item;
        var value = new decimal(1234.56);

        item = visitor.Visit(x => value);
        Assert.Equal("#0", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal(value, item.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        item = visitor.Visit(x => value);
        Assert.Equal("1234.56", item.Text);
        Assert.Empty(item.Parameters);

        visitor = visitor with { UseQuotes = false };
        item = visitor.Visit(x => value);
        Assert.Equal("1234.56", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Value_Decimal_Custom_Culture()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.GetCultureInfo("es-ES"));
        var visitor = new DbTokenVisitor(connection) { Locale = locale };
        ICommandInfo.IBuilder item;
        var value = new decimal(1234.56);

        item = visitor.Visit(x => value);
        Assert.Equal("#0", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal(value, item.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        item = visitor.Visit(x => value);
        Assert.Equal("1234,56", item.Text);
        Assert.Empty(item.Parameters);

        visitor = visitor with { UseQuotes = false };
        item = visitor.Visit(x => value);
        Assert.Equal("1234,56", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Value_DateTime_Invariant_Culture()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.InvariantCulture);
        var visitor = new DbTokenVisitor(connection) { Locale = locale, ConvertValues = false };
        ICommandInfo.IBuilder item;
        var value = new DateTime(2001, 12, 31);

        item = visitor.Visit(x => value);
        Assert.Equal("#0", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal(value, item.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        item = visitor.Visit(x => value);
        Assert.Equal("'12/31/2001 00:00:00'", item.Text);
        Assert.Empty(item.Parameters);

        visitor = visitor with { UseQuotes = false };
        item = visitor.Visit(x => value);
        Assert.Equal("12/31/2001 00:00:00", item.Text);
        Assert.Empty(item.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Value_DateTime_Custom_Culture()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.GetCultureInfo("es-ES"));
        var visitor = new DbTokenVisitor(connection) { Locale = locale, ConvertValues = false };
        ICommandInfo.IBuilder item;
        var value = new DateTime(2001, 12, 31);

        item = visitor.Visit(x => value);
        Assert.Equal("#0", item.Text);
        Assert.Single(item.Parameters);
        Assert.Equal("#0", item.Parameters[0].Name); Assert.Equal(value, item.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        item = visitor.Visit(x => value);
        Assert.Equal("'31/12/2001 0:00:00'", item.Text);
        Assert.Empty(item.Parameters);

        visitor = visitor with { UseQuotes = false };
        item = visitor.Visit(x => value);
        Assert.Equal("31/12/2001 0:00:00", item.Text);
        Assert.Empty(item.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Others()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.GetCultureInfo("es-ES"));
        var visitor = new DbTokenVisitor(connection) { Locale = locale };
        ICommandInfo.IBuilder item;

        item = visitor.Visit(x => x("SELECT *")(x(" FROM "), x.Temp.As("My Alias")));
        Assert.Equal("SELECT * FROM [Temp] AS [My Alias]", item.Text);
        Assert.Empty(item.Parameters);
    }
}