namespace Yotei.ORM.Tests.Internals.DbTokens;

// ========================================================
//[Enforced]
public static class Test_DbTokenVisitor_Dynamics
{
    //[Enforced]
    [Fact]
    public static void Test_Argument()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
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
        var visitor = new DbTokenVisitor(connection);
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
        var visitor = new DbTokenVisitor(connection);
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
        var visitor = new DbTokenVisitor(connection);
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
        var visitor = new DbTokenVisitor(connection);
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
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        var token = new DbTokenChain([
            new DbTokenValue("Hello"),
            new DbTokenLiteral(" world!")]);

        builder = visitor.VisitRange(token);
        Assert.Equal("#0 world!", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("Hello", builder.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        builder = visitor.VisitRange(token);
        Assert.Equal("'Hello' world!", builder.Text);
        Assert.Empty(builder.Parameters);

        visitor = visitor with { UseQuotes = false };
        builder = visitor.VisitRange(token);
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
        var visitor = new DbTokenVisitor(connection);
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
        var visitor = new DbTokenVisitor(connection);
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
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        var command = new FakeCommand(connection,
            "SELECT * FROM Emps WHERE LastName = {0} AND Id = {1}",
            null, "007");

        builder = visitor.Visit(x => command);
        Assert.Equal("(SELECT * FROM Emps WHERE LastName = NULL AND Id = #1)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Command_Invoke()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        var command = new FakeCommand(connection,
            "SELECT * FROM Emps WHERE LastName = {0} AND Id = {1}",
            null, "007");

        builder = visitor.Visit(x => x(command).As(x.Any));
        Assert.Equal("(SELECT * FROM Emps WHERE LastName = NULL AND Id = #1) AS [Any]", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_CommandInfo_Null()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        var info = new CommandInfo(engine, "[First] = {0}", null);
        var token = new DbTokenCommandInfo(info);
        builder = visitor.Visit(token);

        Assert.Equal("[First] = NULL", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_CommandInfo_Valued()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        var info = new CommandInfo(engine, "[First] = {0}", "007");
        var token = new DbTokenCommandInfo(info);
        builder = visitor.Visit(token);

        Assert.Equal("[First] = #0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Identifier_Standard()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Alpha);
        Assert.Equal("[Alpha]", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Alpha.Beta);
        Assert.Equal("[Alpha].[Beta]", builder.Text);
        Assert.Empty(builder.Parameters);

        visitor = visitor with { UseTerminators = false };
        builder = visitor.Visit(x => x.Alpha.Beta);
        Assert.Equal("Alpha.Beta", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Identifier_Dynamic()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.x.x.x);
        Assert.Empty(builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.x.x.Alpha);
        Assert.Equal("[Alpha]", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Alpha.x);
        Assert.Equal("[Alpha].", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Alpha.x.x);
        Assert.Equal("[Alpha]..", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Alpha.x.Beta);
        Assert.Equal("[Alpha]..[Beta]", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Alpha.x.x.Beta);
        Assert.Equal("[Alpha]...[Beta]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Identifier_UnPure()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => (x.Alpha + x.Beta).Delta);
        Assert.Equal("([Alpha] + [Beta]).[Delta]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Indexed_Argument()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x[27]);
        Assert.Equal("[#0]", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal(27, builder.Parameters[0].Value);

        builder = visitor.Visit(x => x[x.Alpha, x.Beta]);
        Assert.Equal("[[Alpha], [Beta]]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Indexed_Member()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Alpha[x.Beta, null, "any"]);
        Assert.Equal("[Alpha][[Beta], NULL, #0]", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("any", builder.Parameters[0].Value);

        builder = visitor.Visit(x => x.Alpha.x[x.Beta, null, "any"]);
        Assert.Equal("[Alpha].[[Beta], NULL, #0]", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("any", builder.Parameters[0].Value);

        builder = visitor.Visit(x => x.Alpha.x.x[x.Beta, null, "any"]);
        Assert.Equal("[Alpha]..[[Beta], NULL, #0]", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("any", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Indexed_Nested()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Alpha[null, x[x.Alpha], x[x.Beta[x.Alpha]]]);
        Assert.Equal("[Alpha][NULL, [[Alpha]], [[Beta][[Alpha]]]]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Invoke_OnArgument()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x());
        Assert.Equal("", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x(50));
        Assert.Equal("#0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal(50, builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Invoke_OnArgument_For_Chaining()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x(x.Alpha, 3, " = ", null));
        Assert.Equal("[Alpha]#0#1NULL", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal(3, builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Equal(" = ", builder.Parameters[1].Value);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Invoke_ToLiteral()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x("any")); // 1st-level single-string argument invoke...
        Assert.Equal("any", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Any("other")); // String is captured...
        Assert.Equal("Any(#0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("other", builder.Parameters[0].Value);

        builder = visitor.Visit(x => x.Any(x("other"))); // String is escaped by 1st-level invoke
        Assert.Equal("Any(other)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Invoke_ToLiteral_Chaining()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x("50")); // 1st-level single-string argument invoke...
        Assert.Equal("50", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x(x(x.Alpha).x(" = ").x(null))); // Not 1st-level escape!
        Assert.Equal("[Alpha]#0NULL", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal(" = ", builder.Parameters[0].Value);

        builder = visitor.Visit(x => x(x(x.Alpha).x(x(" = ")).x(null))); // Enforced...
        Assert.Equal("[Alpha] = NULL", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x(x.Alpha)(x.Beta)(x(" = ").x(null))); // 1st level at the beginning...
        Assert.Equal("[Alpha][Beta] = NULL", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Invoke_MethodAlike()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Alpha.x());
        Assert.Equal("[Alpha]", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Alpha.x(x(" >= "), "any"));
        Assert.Equal("[Alpha] >= #0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("any", builder.Parameters[0].Value);

        builder = visitor.Visit(x => x.Alpha.x(x(" >= ")).x("any")); // '>=' needs enforced escape
        Assert.Equal("[Alpha] >= #0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("any", builder.Parameters[0].Value);

        // Interception of dynamic argument's name as method name is case sensitive...
        builder = visitor.Visit(x => x.X("."));
        Assert.Equal("X(#0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal(".", builder.Parameters[0].Value);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_From_Expression()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => null!);
        Assert.Equal("NULL", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x());
        Assert.Empty(builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x("any"));
        Assert.Equal("any", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Method_Regular_OnArgument()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Alpha());
        Assert.Equal("Alpha()", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Alpha(x.Beta, null, 99));
        Assert.Equal("Alpha([Beta], NULL, #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal(99, builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Regular_OnMember()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Alpha.Beta());
        Assert.Equal("[Alpha].Beta()", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Alpha.Beta(null, x.Delta, 45));
        Assert.Equal("[Alpha].Beta(NULL, [Delta], #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal(45, builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Regular_WithGenerics()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Alpha.Beta<string>(x.Delta, 50));
        Assert.Equal("[Alpha].Beta<String>([Delta], #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal(50, builder.Parameters[0].Value);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Method_Not()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Not(x.Id));
        Assert.Equal("(NOT [Id])", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Alpha.Not(x.Id));
        Assert.Equal("[Alpha].Not([Id])", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Count()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Count());
        Assert.Equal("COUNT(*)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Count('*'));
        Assert.Equal("COUNT(*)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Count("*"));
        Assert.Equal("COUNT(*)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Count(x.Alpha, x.Beta));
        Assert.Equal("COUNT([Alpha], [Beta])", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Id.Count(x.Delta));
        Assert.Equal("[Id].Count([Delta])", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Cast()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Cast(x.Alpha, "DECIMAL(2,5)"));
        Assert.Equal("CAST([Alpha] AS DECIMAL(2,5))", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Cast<string>(x.Alpha));
        Assert.Equal("CAST([Alpha] AS String)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Convert()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Convert("DECIMAL(2,5)", x.Alpha));
        Assert.Equal("CAST([Alpha] AS DECIMAL(2,5))", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Convert<string>(x.Alpha));
        Assert.Equal("CAST([Alpha] AS String)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_As()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Id.As(x.Any));
        Assert.Equal("[Id] AS [Any]", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Id.As(x.Any, 33));
        Assert.Equal("[Id] AS [Any33]", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Id.As(x.Any, null, 50));
        Assert.Equal("[Id] AS [Any50]", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => x.Id.As(x.Any, " ", 50));
        Assert.Equal("[Id] AS [Any 50]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_In()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Id.In("007", null, x.Alpha));
        Assert.Equal("[Id] IN (#0, NULL, [Alpha])", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("007", builder.Parameters[0].Value);

        builder = visitor.Visit(x => x.Id.In(new object?[] { "007", null, x.Alpha }));
        Assert.Equal("[Id] IN (#0, NULL, [Alpha])", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_NotIn()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Id.NotIn("007", null, x.Alpha));
        Assert.Equal("[Id] NOT IN (#0, NULL, [Alpha])", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("007", builder.Parameters[0].Value);

        builder = visitor.Visit(x => x.Id.NotIn(new object?[] { "007", null, x.Alpha }));
        Assert.Equal("[Id] NOT IN (#0, NULL, [Alpha])", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Between()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Id.Between(x.Alpha, 50));
        Assert.Equal("[Id] BETWEEN ([Alpha] AND #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal(50, builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Like()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Id.Like("00*"));
        Assert.Equal("[Id] LIKE #0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("00*", builder.Parameters[0].Value);

        builder = visitor.Visit(x => x.Id.Like(x("'00*'")));
        Assert.Equal("[Id] LIKE '00*'", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_NotLike()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Id.NotLike("00*"));
        Assert.Equal("[Id] NOT LIKE #0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("00*", builder.Parameters[0].Value);

        builder = visitor.Visit(x => x.Id.NotLike(x("'00*'")));
        Assert.Equal("[Id] NOT LIKE '00*'", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Setter_OnArgument()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x = "007");
        Assert.Equal("#0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("007", builder.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };

        builder = visitor.Visit(x => x = "007");
        Assert.Equal("'007'", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Setter_OnMember()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Alpha = "007");
        Assert.Equal("([Alpha] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("007", builder.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };

        builder = visitor.Visit(x => x.Alpha = "007");
        Assert.Equal("([Alpha] = '007')", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Setter_Dynamic()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Alpha = x.Beta);
        Assert.Equal("([Alpha] = [Beta])", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Ternary()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x.Ternary(x.Alpha, x.Beta, x.Delta));
        Assert.Equal("IF (([Alpha]) THEN ([Beta]) ELSE ([Delta]))", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Unary_Dynamic()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => !x);
        Assert.Equal("(NOT )", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => -x);
        Assert.Equal("-()", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Unary_Standard()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => !x.Alpha);
        Assert.Equal("(NOT [Alpha])", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = visitor.Visit(x => -x.Alpha);
        Assert.Equal("-([Alpha])", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Value_Nulls()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => null!);
        Assert.Equal("NULL", builder.Text);
        Assert.Empty(builder.Parameters);

        visitor = visitor with { UseNullString = false };
        builder = visitor.Visit(x => null!);
        Assert.Equal("#0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Null(builder.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        builder = visitor.Visit(x => null!);
        Assert.Equal("''", builder.Text);
        Assert.Empty(builder.Parameters);

        visitor = visitor with { UseQuotes = false };
        builder = visitor.Visit(x => null!);
        Assert.Equal("", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Value_Strings()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => "any");
        Assert.Equal("#0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("any", builder.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        builder = visitor.Visit(x => "any");
        Assert.Equal("'any'", builder.Text);
        Assert.Empty(builder.Parameters);

        visitor = visitor with { UseQuotes = false };
        builder = visitor.Visit(x => "any");
        Assert.Equal("any", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Value_Boolean()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => true);
        Assert.Equal("#0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal(true, builder.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        builder = visitor.Visit(x => true);
        Assert.Equal("TRUE", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Value_Decimal_Invariant_Culture()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.InvariantCulture);
        var visitor = new DbTokenVisitor(connection) { Locale = locale };
        ICommandInfo.IBuilder builder;
        var value = new decimal(1234.56);

        builder = visitor.Visit(x => value);
        Assert.Equal("#0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal(value, builder.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        builder = visitor.Visit(x => value);
        Assert.Equal("1234.56", builder.Text);
        Assert.Empty(builder.Parameters);

        visitor = visitor with { UseQuotes = false };
        builder = visitor.Visit(x => value);
        Assert.Equal("1234.56", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Value_Decimal_Custom_Culture()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.GetCultureInfo("es-ES"));
        var visitor = new DbTokenVisitor(connection) { Locale = locale };
        ICommandInfo.IBuilder builder;
        var value = new decimal(1234.56);

        builder = visitor.Visit(x => value);
        Assert.Equal("#0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal(value, builder.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        builder = visitor.Visit(x => value);
        Assert.Equal("1234,56", builder.Text);
        Assert.Empty(builder.Parameters);

        visitor = visitor with { UseQuotes = false };
        builder = visitor.Visit(x => value);
        Assert.Equal("1234,56", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Value_DateTime_Invariant_Culture()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.InvariantCulture);
        var visitor = new DbTokenVisitor(connection) { Locale = locale, ConvertValues = false };
        ICommandInfo.IBuilder builder;
        var value = new DateTime(2001, 12, 31);

        builder = visitor.Visit(x => value);
        Assert.Equal("#0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal(value, builder.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        builder = visitor.Visit(x => value);
        Assert.Equal("'12/31/2001 00:00:00'", builder.Text);
        Assert.Empty(builder.Parameters);

        visitor = visitor with { UseQuotes = false };
        builder = visitor.Visit(x => value);
        Assert.Equal("12/31/2001 00:00:00", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Value_DateTime_Custom_Culture()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var locale = new Locale(CultureInfo.GetCultureInfo("es-ES"));
        var visitor = new DbTokenVisitor(connection) { Locale = locale, ConvertValues = false };
        ICommandInfo.IBuilder builder;
        var value = new DateTime(2001, 12, 31);

        builder = visitor.Visit(x => value);
        Assert.Equal("#0", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal(value, builder.Parameters[0].Value);

        visitor = visitor with { CaptureValues = false };
        builder = visitor.Visit(x => value);
        Assert.Equal("'31/12/2001 0:00:00'", builder.Text);
        Assert.Empty(builder.Parameters);

        visitor = visitor with { UseQuotes = false };
        builder = visitor.Visit(x => value);
        Assert.Equal("31/12/2001 0:00:00", builder.Text);
        Assert.Empty(builder.Parameters);
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
        ICommandInfo.IBuilder builder;

        builder = visitor.Visit(x => x("SELECT *")(x(" FROM "), x.Temp.As("My Alias")));
        Assert.Equal("SELECT * FROM [Temp] AS [My Alias]", builder.Text);
        Assert.Empty(builder.Parameters);
    }
}