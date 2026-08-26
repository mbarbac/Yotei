namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_DbLambdaParser
{
    //[Enforced]
    [Fact]
    public static void Parse_Argument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenArgument arg;

        token = parser.Parse(x => x);
        Assert.Equal("x", token.ToString());
        arg = Assert.IsType<DbTokenArgument>(token);
        Assert.Equal("x", arg.Name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Binary_And()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenBinary binay;
        DbTokenValue value;

        token = parser.Parse(x => x.Alpha && x.Beta);
        Assert.Equal("(x.[Alpha] And x.[Beta])", token.ToString());
        binay = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, binay.Operation);
        Assert.IsType<DbTokenIdentifier>(binay.Left);
        Assert.IsType<DbTokenIdentifier>(binay.Right);

        token = parser.Parse(x => x.x.Alpha && null);
        Assert.Equal("(x..[Alpha] And NULL)", token.ToString());
        binay = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, binay.Operation);
        Assert.IsType<DbTokenIdentifier>(binay.Left);
        value = Assert.IsType<DbTokenValue>(binay.Right);
        Assert.Null(value.Value);

        token = parser.Parse(x => x.x.Alpha && "any");
        Assert.Equal("(x..[Alpha] And 'any')", token.ToString());
        binay = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, binay.Operation);
        Assert.IsType<DbTokenIdentifier>(binay.Left);
        value = Assert.IsType<DbTokenValue>(binay.Right);
        Assert.Equal("any", value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_SingleAnd()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenBinary binary;
        DbTokenValue value;

        token = parser.Parse(x => x.Alpha & x.Beta);
        Assert.Equal("(x.[Alpha] And x.[Beta])", token.ToString());
        binary = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, binary.Operation);
        Assert.IsType<DbTokenIdentifier>(binary.Left);
        Assert.IsType<DbTokenIdentifier>(binary.Right);

        token = parser.Parse(x => x.x.Alpha & null);
        Assert.Equal("(x..[Alpha] And NULL)", token.ToString());
        binary = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, binary.Operation);
        Assert.IsType<DbTokenIdentifier>(binary.Left);
        value = Assert.IsType<DbTokenValue>(binary.Right);
        Assert.Null(value.Value);

        token = parser.Parse(x => x.x.Alpha & "any");
        Assert.Equal("(x..[Alpha] And 'any')", token.ToString());
        binary = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, binary.Operation);
        Assert.IsType<DbTokenIdentifier>(binary.Left);
        value = Assert.IsType<DbTokenValue>(binary.Right);
        Assert.Equal("any", value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Others()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenBinary binary;
        DbTokenValue value;

        token = parser.Parse(x => x.Alpha >= x.Beta);
        Assert.Equal("(x.[Alpha] GreaterThanOrEqual x.[Beta])", token.ToString());
        binary = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.GreaterThanOrEqual, binary.Operation);
        Assert.IsType<DbTokenIdentifier>(binary.Left);
        Assert.IsType<DbTokenIdentifier>(binary.Right);

        token = parser.Parse(x => x.x.Alpha != "any");
        Assert.Equal("(x..[Alpha] NotEqual 'any')", token.ToString());
        binary = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.NotEqual, binary.Operation);
        Assert.IsType<DbTokenIdentifier>(binary.Left);
        value = Assert.IsType<DbTokenValue>(binary.Right);
        Assert.Equal("any", value.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Coalesce()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;

        // Standard case...
        token = parser.Parse(x => x.Coalesce(x.Alpha, x.Beta));
        Assert.Equal("(x.[Alpha] ?? x.[Beta])", token.ToString());
        Assert.IsType<DbTokenCoalesce>(token);

        // The dynamic lambda parser cannot parse coalesce expressions: it just returns the left
        // element. This is why we need to use a 'x.Coalesce()' virtual method.
        try
        {
            token = parser.Parse(x => x.Alpha ?? x.Beta);
            Assert.IsType<DbTokenCoalesce>(token);
            Assert.Fail();
        }
        catch (Xunit.Sdk.IsTypeException) { }

        // Special case when the left operand is already a null-alike one...
        token = parser.Parse(x => x.Coalesce(null, x.Beta));
        Assert.Equal("x.[Beta]", token.ToString());
        Assert.IsType<DbTokenIdentifier>(token);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Command_Straight()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenCommandInfo info;

        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection, "SELECT * FROM Employees WHERE Id = {0}", "007");

        token = parser.Parse(x => command);
        Assert.Equal("(SELECT * FROM Employees WHERE Id = #0)", token.ToString());
        info = Assert.IsType<DbTokenCommandInfo>(token);
        Assert.Equal("SELECT * FROM Employees WHERE Id = #0", info.CommandInfo.Text);
        Assert.Single(info.CommandInfo.Parameters);
        Assert.Equal("#0", info.CommandInfo.Parameters[0].Name);
        Assert.Equal("007", info.CommandInfo.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Command_Wrapped()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenCommandInfo info;

        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection, "SELECT * FROM Employees WHERE Id = {0}", "007");

        token = parser.Parse(x => x(command));
        Assert.Equal("(SELECT * FROM Employees WHERE Id = #0)", token.ToString());
        info = Assert.IsType<DbTokenCommandInfo>(token);
        Assert.Equal("SELECT * FROM Employees WHERE Id = #0", info.CommandInfo.Text);
        Assert.Single(info.CommandInfo.Parameters);
        Assert.Equal("#0", info.CommandInfo.Parameters[0].Name);
        Assert.Equal("007", info.CommandInfo.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Command_Wrapped_With_As()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenMethod method;
        DbTokenCommandInfo info;

        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection, "SELECT * FROM Employees WHERE Id = {0}", "007");

        token = parser.Parse(x => x(command).As(x.Any));
        Assert.Equal("(SELECT * FROM Employees WHERE Id = #0).As(x.[Any])", token.ToString());
        method = Assert.IsType<DbTokenMethod>(token);
        Assert.Equal("As", method.Name);
        Assert.Single(method.Arguments);
        Assert.IsType<DbTokenIdentifier>(method.Arguments[0]);

        info = Assert.IsType<DbTokenCommandInfo>(method.Host);
        Assert.Equal("SELECT * FROM Employees WHERE Id = #0", info.CommandInfo.Text);
        Assert.Single(info.CommandInfo.Parameters);
        Assert.Equal("#0", info.CommandInfo.Parameters[0].Name);
        Assert.Equal("007", info.CommandInfo.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Command_Chained()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenInvoke invoke;
        DbTokenMethod method;
        DbTokenCommandInfo info;
        DbTokenLiteral literal;

        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection, "WHERE Id = {0}", "007");

        token = parser.Parse(x => x(x("SELECT * "), x(command).As(x.Any)));
        Assert.Equal("x(SELECT * , (WHERE Id = #0).As(x.[Any]))", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Equal(2, invoke.Arguments.Length);
        literal = Assert.IsType<DbTokenLiteral>(invoke.Arguments[0]); Assert.Equal("SELECT * ", literal.Value);
        method = Assert.IsType<DbTokenMethod>(invoke.Arguments[1]);
        Assert.Equal("As", method.Name);
        Assert.Single(method.Arguments);
        Assert.IsType<DbTokenIdentifier>(method.Arguments[0]);
        info = Assert.IsType<DbTokenCommandInfo>(method.Host);
        Assert.Equal("WHERE Id = #0", info.CommandInfo.Text);
        Assert.Single(info.CommandInfo.Parameters);
        Assert.Equal("#0", info.CommandInfo.Parameters[0].Name);
        Assert.Equal("007", info.CommandInfo.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Cast_Argument_ToType()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenConvert.ToType cast;

        token = parser.Parse(x => (string)x);
        Assert.Equal("((string) x)", token.ToString());
        cast = Assert.IsType<DbTokenConvert.ToType>(token);
        Assert.Equal(typeof(string), cast.Type);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Cast_Member_ToType()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenConvert.ToType cast;

        token = parser.Parse(x => (DateTime)x.Alpha);
        Assert.Equal("((DateTime) x.[Alpha])", token.ToString());
        cast = Assert.IsType<DbTokenConvert.ToType>(token);
        Assert.Equal(typeof(DateTime), cast.Type);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Convert_GenericType()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenConvert.ToType cast;

        token = parser.Parse(x => x.Cast<string>(x.Alpha));
        Assert.Equal("((string) x.[Alpha])", token.ToString());
        cast = Assert.IsType<DbTokenConvert.ToType>(token);
        Assert.Equal(typeof(string), cast.Type);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Convert_ToSpec()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenConvert.ToSpec cast;

        token = parser.Parse(x => x.Convert("varchar", x.Alpha));
        Assert.Equal("((varchar) x.[Alpha])", token.ToString());
        cast = Assert.IsType<DbTokenConvert.ToSpec>(token);
        Assert.Equal("varchar", cast.Type);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Convert_ToType()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenConvert.ToType cast;

        token = parser.Parse(x => x.Convert(typeof(string), x.Alpha));
        Assert.Equal("((string) x.[Alpha])", token.ToString());
        cast = Assert.IsType<DbTokenConvert.ToType>(token);
        Assert.Equal(typeof(string), cast.Type);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Indexed_Argument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenIndexed indexed;
        DbTokenValue value;
        DbTokenIdentifier id;

        token = parser.Parse(x => x[33]);
        Assert.Equal("x['33']", token.ToString());
        indexed = Assert.IsType<DbTokenIndexed>(token);
        Assert.Single(indexed.Indexes);
        value = Assert.IsType<DbTokenValue>(indexed.Indexes[0]);
        Assert.Equal(33, value.Value);

        token = parser.Parse(x => x[x.Alpha]);
        Assert.Equal("x[x.[Alpha]]", token.ToString());
        indexed = Assert.IsType<DbTokenIndexed>(token);
        Assert.Single(indexed.Indexes);
        id = Assert.IsType<DbTokenIdentifier>(indexed.Indexes[0]); Assert.Equal("[Alpha]", id.Value);

        token = parser.Parse(x => x[x[x.Alpha]]);
        Assert.Equal("x[x[x.[Alpha]]]", token.ToString());
        indexed = Assert.IsType<DbTokenIndexed>(token);
        Assert.Single(indexed.Indexes);
        indexed = Assert.IsType<DbTokenIndexed>(indexed.Indexes[0]);
        id = Assert.IsType<DbTokenIdentifier>(indexed.Indexes[0]); Assert.Equal("[Alpha]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Indexed_Member()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenIndexed indexed;
        DbTokenIdentifier id;
        DbTokenValue value;

        token = parser.Parse(x => x.Alpha[x.Beta, null, "Other"]);
        Assert.Equal("x.[Alpha][x.[Beta], NULL, 'Other']", token.ToString());
        indexed = Assert.IsType<DbTokenIndexed>(token);
        Assert.Equal(3, indexed.Indexes.Length);
        id = Assert.IsType<DbTokenIdentifier>(indexed.Indexes[0]); Assert.Equal("[Beta]", id.Value);
        value = Assert.IsType<DbTokenValue>(indexed.Indexes[1]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(indexed.Indexes[2]); Assert.Equal("Other", value.Value);

        token = parser.Parse(x => x.Alpha[x.Beta[x.Delta[null, "Other"]]]);
        Assert.Equal("x.[Alpha][x.[Beta][x.[Delta][NULL, 'Other']]]", token.ToString());
        indexed = Assert.IsType<DbTokenIndexed>(token); Assert.Single(indexed.Indexes);
        indexed = Assert.IsType<DbTokenIndexed>(indexed.Indexes[0]); Assert.Single(indexed.Indexes);
        indexed = Assert.IsType<DbTokenIndexed>(indexed.Indexes[0]);
        Assert.Equal(2, indexed.Indexes.Length);
        value = Assert.IsType<DbTokenValue>(indexed.Indexes[0]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(indexed.Indexes[1]); Assert.Equal("Other", value.Value);

        token = parser.Parse(x => x.Alpha[x.Alpha[x.Alpha[null, "Other"]]]);
        Assert.Equal("x.[Alpha][x.[Alpha][x.[Alpha][NULL, 'Other']]]", token.ToString());
        indexed = Assert.IsType<DbTokenIndexed>(token); Assert.Single(indexed.Indexes);
        indexed = Assert.IsType<DbTokenIndexed>(indexed.Indexes[0]); Assert.Single(indexed.Indexes);
        indexed = Assert.IsType<DbTokenIndexed>(indexed.Indexes[0]);
        Assert.Equal(2, indexed.Indexes.Length);
        value = Assert.IsType<DbTokenValue>(indexed.Indexes[0]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(indexed.Indexes[1]); Assert.Equal("Other", value.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_OnArgument_Single()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenInvoke invoke;
        DbTokenValue value;
        DbTokenIdentifier id;

        token = parser.Parse(x => x());
        Assert.Equal("x()", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Empty(invoke.Arguments);

        token = parser.Parse(x => x(33));
        Assert.Equal("x('33')", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(invoke.Arguments);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[0]); Assert.Equal(33, value.Value);

        token = parser.Parse(x => x(x.Alpha));
        Assert.Equal("x(x.[Alpha])", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(invoke.Arguments);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]); Assert.Equal("[Alpha]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_OnArgument_Many()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenInvoke invoke;
        DbTokenValue value;
        DbTokenIdentifier id;

        token = parser.Parse(x => x("any", "other"));
        Assert.Equal("x('any', 'other')", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Equal(2, invoke.Arguments.Length);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[0]); Assert.Equal("any", value.Value);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[1]); Assert.Equal("other", value.Value);

        token = parser.Parse(x => x(x.Alpha, x.Beta, null, "any"));
        Assert.Equal("x(x.[Alpha], x.[Beta], NULL, 'any')", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Equal(4, invoke.Arguments.Length);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]); Assert.Equal("[Alpha]", id.Value);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[1]); Assert.Equal("[Beta]", id.Value);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[2]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[3]); Assert.Equal("any", value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_OnArgument_Chained()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenInvoke invoke;
        DbTokenIdentifier id;

        token = parser.Parse(x => x(x.Alpha)(x.Beta));
        Assert.Equal("x(x.[Alpha])(x.[Beta])", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(invoke.Arguments);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]); Assert.Equal("[Beta]", id.Value);
        invoke = Assert.IsType<DbTokenInvoke>(invoke.Host);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]); Assert.Equal("[Alpha]", id.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_ToLiteral()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenLiteral literal;
        DbTokenInvoke invoke;
        DbTokenValue value;

        // First-level invoke node with string argument...
        token = parser.Parse(x => x("any"));
        Assert.Equal("any", token.ToString());
        literal = Assert.IsType<DbTokenLiteral>(token); Assert.Equal("any", literal.Value);

        // Two-arguments invoke nodes do not qualify for conversion to literal...
        token = parser.Parse(x => x("Other", "Another"));
        Assert.Equal("x('Other', 'Another')", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Equal(2, invoke.Arguments.Length);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[0]); Assert.Equal("Other", value.Value);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[1]); Assert.Equal("Another", value.Value);

        // Conversion explicitly prevented...
        parser.PreventSingleStringValueToLiteral = true;
        token = parser.Parse(x => x("Other"));
        Assert.Equal("x('Other')", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(invoke.Arguments);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[0]);
        Assert.Equal("Other", value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_ToLiteral_Chained()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenChain chain;
        DbTokenInvoke invoke;
        DbTokenIdentifier id;
        DbTokenLiteral literal;

        // By just chained, we obtain a chain node...
        token = parser.Parse(x => x(x.Alpha)("+any"));
        Assert.Equal("[x(x.[Alpha]), +any]", token.ToString());
        chain = Assert.IsType<DbTokenChain>(token);
        Assert.Equal(2, chain.Count);
        invoke = Assert.IsType<DbTokenInvoke>(chain[0]);
        Assert.Single(invoke.Arguments);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]); Assert.Equal("[Alpha]", id.Value);
        literal = Assert.IsType<DbTokenLiteral>(chain[1]); Assert.Equal("+any", literal.Value);

        // We can also use a multi-argument invoke node...
        token = parser.Parse(x => x(x.Alpha, x("+any")));
        Assert.Equal("x(x.[Alpha], +any)", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Equal(2, invoke.Arguments.Length);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]);
        literal = Assert.IsType<DbTokenLiteral>(invoke.Arguments[1]);

        // Another example...
        token = parser.Parse(x => x(x("WHERE ID > "), x("007")));
        Assert.Equal("x(WHERE ID > , 007)", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Equal(2, invoke.Arguments.Length);
        literal = Assert.IsType<DbTokenLiteral>(invoke.Arguments[0]); Assert.Equal("WHERE ID > ", literal.Value);
        literal = Assert.IsType<DbTokenLiteral>(invoke.Arguments[1]); Assert.Equal("007", literal.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Member_Standard()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenIdentifier id;
        DbTokenArgument arg;

        token = parser.Parse(x => x.Alpha);
        Assert.Equal("x.[Alpha]", token.ToString());
        id = Assert.IsType<DbTokenIdentifier>(token); Assert.Equal("[Alpha]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host); Assert.Equal("x", arg.Name);

        token = parser.Parse(x => x.Alpha.Beta);
        Assert.Equal("x.[Alpha].[Beta]", token.ToString());
        id = Assert.IsType<DbTokenIdentifier>(token); Assert.Equal("[Beta]", id.Value);
        id = Assert.IsType<DbTokenIdentifier>(id.Host); Assert.Equal("[Alpha]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host); Assert.Equal("x", arg.Name);

        token = parser.Parse(x => x.Alpha.Beta.Delta);
        Assert.Equal("x.[Alpha].[Beta].[Delta]", token.ToString());
        id = Assert.IsType<DbTokenIdentifier>(token); Assert.Equal("[Delta]", id.Value);
        id = Assert.IsType<DbTokenIdentifier>(id.Host); Assert.Equal("[Beta]", id.Value);
        id = Assert.IsType<DbTokenIdentifier>(id.Host); Assert.Equal("[Alpha]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host); Assert.Equal("x", arg.Name);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Member_Embedded_Dynamic_Argument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenIdentifier id;
        DbTokenArgument arg;

        token = parser.Parse(x => x.x.Alpha);
        Assert.Equal("x..[Alpha]", token.ToString());
        id = Assert.IsType<DbTokenIdentifier>(token); Assert.Equal("[Alpha]", id.Value);
        id = Assert.IsType<DbTokenIdentifier>(id.Host); Assert.Null(id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host); Assert.Equal("x", arg.Name);

        token = parser.Parse(x => x.Alpha.x);
        Assert.Equal("x.[Alpha].", token.ToString());
        id = Assert.IsType<DbTokenIdentifier>(token); Assert.Null(id.Value);
        id = Assert.IsType<DbTokenIdentifier>(id.Host); Assert.Equal("[Alpha]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host); Assert.Equal("x", arg.Name);

        token = parser.Parse(x => x.x);
        Assert.Equal("x.", token.ToString());
        id = Assert.IsType<DbTokenIdentifier>(token); Assert.Null(id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host); Assert.Equal("x", arg.Name);

        token = parser.Parse(x => x.x.x);
        Assert.Equal("x..", token.ToString());
        id = Assert.IsType<DbTokenIdentifier>(token); Assert.Null(id.Value);
        id = Assert.IsType<DbTokenIdentifier>(id.Host); Assert.Null(id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host); Assert.Equal("x", arg.Name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Method()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenMethod method;
        DbTokenIdentifier name;
        DbTokenValue value;

        token = parser.Parse(x => x.Alpha());
        Assert.Equal("x.Alpha()", token.ToString());
        method = Assert.IsType<DbTokenMethod>(token);
        Assert.Empty(method.Arguments);

        token = parser.Parse(x => x.Alpha.Beta());
        Assert.Equal("x.[Alpha].Beta()", token.ToString());
        method = Assert.IsType<DbTokenMethod>(token);
        Assert.Empty(method.Arguments);

        token = parser.Parse(x => x.Alpha(x.Beta, null, 50));
        Assert.Equal("x.Alpha(x.[Beta], NULL, '50')", token.ToString());
        method = Assert.IsType<DbTokenMethod>(token);
        Assert.Equal(3, method.Arguments.Length);
        name = Assert.IsType<DbTokenIdentifier>(method.Arguments[0]); Assert.Equal("[Beta]", name.Value);
        value = Assert.IsType<DbTokenValue>(method.Arguments[1]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(method.Arguments[2]); Assert.Equal(50, value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_WithGenerics()
    {
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
        DbTokenMethod method;

        token = parser.Parse(x => x.Whatever<string>(x.Alpha));
        Assert.Equal("x.Whatever<string>(x.[Alpha])", token.ToString());
        method = Assert.IsType<DbTokenMethod>(token);
        Assert.Single(method.Arguments);
        Assert.IsType<DbTokenIdentifier>(method.Arguments[0]);
        Assert.Single(method.TypeArguments);
        Assert.IsType<Type>(method.TypeArguments[0], exactMatch: false);

        token = parser.Parse(x => x.Whatever<string, int>(x.Alpha));
        Assert.Equal("x.Whatever<string, int>(x.[Alpha])", token.ToString());
        method = Assert.IsType<DbTokenMethod>(token);
        Assert.Single(method.Arguments);
        Assert.IsType<DbTokenIdentifier>(method.Arguments[0]);
        Assert.Equal(2, method.TypeArguments.Length);
        Assert.True(method.TypeArguments[0] == typeof(string));
        Assert.True(method.TypeArguments[1] == typeof(int));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Method_AsInvoke_Standard()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenInvoke invoke;
        DbTokenIdentifier id;

        token = parser.Parse(x => x.Alpha.x());
        Assert.Equal("x.[Alpha]()", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Empty(invoke.Arguments);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Host); Assert.Equal("[Alpha]", id.Value);

        token = parser.Parse(x => x.Alpha.x(x.Beta));
        Assert.Equal("x.[Alpha](x.[Beta])", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(invoke.Arguments);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]); Assert.Equal("[Beta]", id.Value);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Host); Assert.Equal("[Alpha]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_AsInvoke_Chained()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenInvoke invoke;
        DbTokenIdentifier id;

        token = parser.Parse(x => x.Alpha.x(x.Beta)(x.Delta));
        Assert.Equal("x.[Alpha](x.[Beta])(x.[Delta])", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(invoke.Arguments);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]); Assert.Equal("[Delta]", id.Value);
        invoke = Assert.IsType<DbTokenInvoke>(invoke.Host);
        Assert.Single(invoke.Arguments);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]); Assert.Equal("[Beta]", id.Value);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Host); Assert.Equal("[Alpha]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_AsInvoke_ToLiteral()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenChain chain;
        DbTokenIdentifier id;
        DbTokenLiteral literal;

        token = parser.Parse(x => x.Alpha.x("any"));
        Assert.Equal("[x.[Alpha], any]", token.ToString());
        chain = Assert.IsType<DbTokenChain>(token);
        Assert.Equal(2, chain.Count);
        id = Assert.IsType<DbTokenIdentifier>(chain[0]); Assert.Equal("[Alpha]", id.Value);
        literal = Assert.IsType<DbTokenLiteral>(chain[1]); Assert.Equal("any", literal.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Setter_OnArgument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenArgument arg;
        DbTokenValue value;

        // Assigment of argument on argument just returns... the argument, obviously.
#pragma warning disable CS1717
        token = parser.Parse(x => x = x);
        Assert.Equal("x", token.ToString());
        arg = Assert.IsType<DbTokenArgument>(token); Assert.Equal("x", arg.Name);
#pragma warning restore

        // Assign a value to an argument just returns the value!
        token = parser.Parse(x => x = "007");
        Assert.Equal("'007'", token.ToString());
        value = Assert.IsType<DbTokenValue>(token); Assert.Equal("007", value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Setter_OnMember()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenSetter setter;
        DbTokenIdentifier id;
        DbTokenValue value;
        DbTokenArgument arg;

        token = parser.Parse(x => x.Alpha = x);
        Assert.Equal("(x.[Alpha] = x)", token.ToString());
        setter = Assert.IsType<DbTokenSetter>(token);
        id = Assert.IsType<DbTokenIdentifier>(setter.Target); Assert.Equal("[Alpha]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(setter.Value); Assert.Equal("x", arg.Name);

        token = parser.Parse(x => x.Alpha = "007");
        Assert.Equal("(x.[Alpha] = '007')", token.ToString());
        setter = Assert.IsType<DbTokenSetter>(token);
        id = Assert.IsType<DbTokenIdentifier>(setter.Target); Assert.Equal("[Alpha]", id.Value);
        value = Assert.IsType<DbTokenValue>(setter.Value); Assert.Equal("007", value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Setter_Dynamic()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenSetter setter;
        DbTokenIdentifier id;

        token = parser.Parse(x => x.Alpha = x.Beta);
        Assert.Equal("(x.[Alpha] = x.[Beta])", token.ToString());
        setter = Assert.IsType<DbTokenSetter>(token);
        id = Assert.IsType<DbTokenIdentifier>(setter.Target); Assert.Equal("[Alpha]", id.Value);
        id = Assert.IsType<DbTokenIdentifier>(setter.Value); Assert.Equal("[Beta]", id.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Ternary()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenTernary ternary;
        DbTokenIdentifier id;

        token = parser.Parse(x => x.Ternary(x.Alpha, x.Beta, x.Delta));
        Assert.Equal("(x.[Alpha] ? x.[Beta] : x.[Delta])", token.ToString());
        ternary = Assert.IsType<DbTokenTernary>(token);
        id = Assert.IsType<DbTokenIdentifier>(ternary.Left); Assert.Equal("[Alpha]", id.Value);
        id = Assert.IsType<DbTokenIdentifier>(ternary.Middle); Assert.Equal("[Beta]", id.Value);
        id = Assert.IsType<DbTokenIdentifier>(ternary.Right); Assert.Equal("[Delta]", id.Value);

        // LambdaParser cannot identify a ternary operation. This is a known limitation because
        // when the first element is 'executed', then either the middle or the right branch is
        // ignored, not even touched by the DLR.
        try
        {
            token = parser.Parse(x => x.Alpha ? x.Beta : x.Delta);
            Assert.IsType<DbTokenTernary>(token);
            Assert.Fail();
        }
        catch (Xunit.Sdk.IsTypeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Unary_OnArgument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenUnary unary;
        DbTokenArgument arg;
        DbTokenIdentifier id;

        token = parser.Parse(x => !x);
        Assert.Equal("(Not x)", token.ToString());
        unary = Assert.IsType<DbTokenUnary>(token);
        Assert.Equal(ExpressionType.Not, unary.Operation);
        arg = Assert.IsType<DbTokenArgument>(unary.Target); Assert.Equal("x", arg.Name);

        token = parser.Parse(x => -x.x.x);
        Assert.Equal("(Negate x..)", token.ToString());
        unary = Assert.IsType<DbTokenUnary>(token);
        Assert.Equal(ExpressionType.Negate, unary.Operation);
        id = Assert.IsType<DbTokenIdentifier>(unary.Target); Assert.Null(id.Value);
        id = Assert.IsType<DbTokenIdentifier>(id.Host); Assert.Null(id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host); Assert.Equal("x", arg.Name);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Unary_OnMember()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenUnary unary;
        DbTokenArgument arg;
        DbTokenIdentifier id;

        token = parser.Parse(x => !x.Alpha);
        Assert.Equal("(Not x.[Alpha])", token.ToString());
        unary = Assert.IsType<DbTokenUnary>(token);
        Assert.Equal(ExpressionType.Not, unary.Operation);
        id = Assert.IsType<DbTokenIdentifier>(unary.Target); Assert.Equal("[Alpha]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host); Assert.Equal("x", arg.Name);

        token = parser.Parse(x => -x.Alpha.Beta);
        Assert.Equal("(Negate x.[Alpha].[Beta])", token.ToString());
        unary = Assert.IsType<DbTokenUnary>(token);
        Assert.Equal(ExpressionType.Negate, unary.Operation);
        id = Assert.IsType<DbTokenIdentifier>(unary.Target); Assert.Equal("[Beta]", id.Value);
        id = Assert.IsType<DbTokenIdentifier>(id.Host); Assert.Equal("[Alpha]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host); Assert.Equal("x", arg.Name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Value_Standard()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenValue value;
        DateOnly date;

        token = parser.Parse(x => null);
        Assert.Equal("NULL", token.ToString());
        value = Assert.IsType<DbTokenValue>(token); Assert.Null(value.Value);

        token = parser.Parse(x => true);
        Assert.Equal("TRUE", token.ToString());
        value = Assert.IsType<DbTokenValue>(token); Assert.True((bool)value.Value!);

        token = parser.Parse(x => "any");
        Assert.Equal("'any'", token.ToString());
        value = Assert.IsType<DbTokenValue>(token); Assert.Equal("any", value.Value);

        // Cannot use ToString() because value's representation is culture-dependant...
        token = parser.Parse(x => new DateOnly(2000, 12, 31));
        value = Assert.IsType<DbTokenValue>(token);
        date = Assert.IsType<DateOnly>(value.Value);
        Assert.Equal(2000, date.Year);
        Assert.Equal(12, date.Month);
        Assert.Equal(31, date.Day);
    }
}