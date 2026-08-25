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
        DbTokenArgument item;

        token = parser.Parse(x => x);
        Assert.Equal("x", token.ToString());
        item = Assert.IsType<DbTokenArgument>(token);
        Assert.Equal("x", item.Name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Binary_And()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenBinary item;
        DbTokenValue value;

        token = parser.Parse(x => x.Alpha && x.Beta);
        Assert.Equal("(x.[Alpha] And x.[Beta])", token.ToString());
        item = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, item.Operation);
        Assert.IsType<DbTokenIdentifier>(item.Left);
        Assert.IsType<DbTokenIdentifier>(item.Right);

        token = parser.Parse(x => x.x.Alpha && null);
        Assert.Equal("(x..[Alpha] And NULL)", token.ToString());
        item = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, item.Operation);
        Assert.IsType<DbTokenIdentifier>(item.Left);
        value = Assert.IsType<DbTokenValue>(item.Right);
        Assert.Null(value.Value);

        token = parser.Parse(x => x.x.Alpha && "any");
        Assert.Equal("(x..[Alpha] And 'any')", token.ToString());
        item = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, item.Operation);
        Assert.IsType<DbTokenIdentifier>(item.Left);
        value = Assert.IsType<DbTokenValue>(item.Right);
        Assert.Equal("any", value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_SingleAnd()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenBinary item;
        DbTokenValue value;

        token = parser.Parse(x => x.Alpha & x.Beta);
        Assert.Equal("(x.[Alpha] And x.[Beta])", token.ToString());
        item = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, item.Operation);
        Assert.IsType<DbTokenIdentifier>(item.Left);
        Assert.IsType<DbTokenIdentifier>(item.Right);

        token = parser.Parse(x => x.x.Alpha & null);
        Assert.Equal("(x..[Alpha] And NULL)", token.ToString());
        item = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, item.Operation);
        Assert.IsType<DbTokenIdentifier>(item.Left);
        value = Assert.IsType<DbTokenValue>(item.Right);
        Assert.Null(value.Value);

        token = parser.Parse(x => x.x.Alpha & "any");
        Assert.Equal("(x..[Alpha] And 'any')", token.ToString());
        item = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, item.Operation);
        Assert.IsType<DbTokenIdentifier>(item.Left);
        value = Assert.IsType<DbTokenValue>(item.Right);
        Assert.Equal("any", value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Others()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenBinary item;
        DbTokenValue value;

        token = parser.Parse(x => x.Alpha >= x.Beta);
        Assert.Equal("(x.[Alpha] GreaterThanOrEqual x.[Beta])", token.ToString());
        item = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.GreaterThanOrEqual, item.Operation);
        Assert.IsType<DbTokenIdentifier>(item.Left);
        Assert.IsType<DbTokenIdentifier>(item.Right);

        token = parser.Parse(x => x.x.Alpha != "any");
        Assert.Equal("(x..[Alpha] NotEqual 'any')", token.ToString());
        item = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.NotEqual, item.Operation);
        Assert.IsType<DbTokenIdentifier>(item.Left);
        value = Assert.IsType<DbTokenValue>(item.Right);
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
        DbTokenCommandInfo item;

        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection, "SELECT * FROM Employees WHERE Id = {0}", "007");

        token = parser.Parse(x => command);
        Assert.Equal("(SELECT * FROM Employees WHERE Id = #0)", token.ToString());
        item = Assert.IsType<DbTokenCommandInfo>(token);
        Assert.Equal("SELECT * FROM Employees WHERE Id = #0", item.CommandInfo.Text);
        Assert.Single(item.CommandInfo.Parameters);
        Assert.Equal("#0", item.CommandInfo.Parameters[0].Name);
        Assert.Equal("007", item.CommandInfo.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Command_Wrapped()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenCommandInfo item;

        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection, "SELECT * FROM Employees WHERE Id = {0}", "007");

        token = parser.Parse(x => x(command));
        Assert.Equal("(SELECT * FROM Employees WHERE Id = #0)", token.ToString());
        item = Assert.IsType<DbTokenCommandInfo>(token);
        Assert.Equal("SELECT * FROM Employees WHERE Id = #0", item.CommandInfo.Text);
        Assert.Single(item.CommandInfo.Parameters);
        Assert.Equal("#0", item.CommandInfo.Parameters[0].Name);
        Assert.Equal("007", item.CommandInfo.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Command_Wrapped_With_As()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenMethod item;
        DbTokenCommandInfo cmd;

        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection, "SELECT * FROM Employees WHERE Id = {0}", "007");

        token = parser.Parse(x => x(command).As(x.Any));
        Assert.Equal("(SELECT * FROM Employees WHERE Id = #0).As(x.[Any])", token.ToString());
        item = Assert.IsType<DbTokenMethod>(token);
        Assert.Equal("As", item.Name);
        Assert.Single(item.Arguments);
        Assert.IsType<DbTokenIdentifier>(item.Arguments[0]);

        cmd = Assert.IsType<DbTokenCommandInfo>(item.Host);
        Assert.Equal("SELECT * FROM Employees WHERE Id = #0", cmd.CommandInfo.Text);
        Assert.Single(cmd.CommandInfo.Parameters);
        Assert.Equal("#0", cmd.CommandInfo.Parameters[0].Name);
        Assert.Equal("007", cmd.CommandInfo.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Cast_Argument_ToType()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenConvert.ToType item;

        token = parser.Parse(x => (string)x);
        Assert.Equal("((string) x)", token.ToString());
        item = Assert.IsType<DbTokenConvert.ToType>(token);
        Assert.Equal(typeof(string), item.Type);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Cast_Member_ToType()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenConvert.ToType item;

        token = parser.Parse(x => (DateTime)x.Alpha);
        Assert.Equal("((DateTime) x.[Alpha])", token.ToString());
        item = Assert.IsType<DbTokenConvert.ToType>(token);
        Assert.Equal(typeof(DateTime), item.Type);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Convert_GenericType()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenConvert.ToType item;

        token = parser.Parse(x => x.Cast<string>(x.Alpha));
        Assert.Equal("((string) x.[Alpha])", token.ToString());
        item = Assert.IsType<DbTokenConvert.ToType>(token);
        Assert.Equal(typeof(string), item.Type);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Convert_ToSpec()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenConvert.ToSpec item;

        token = parser.Parse(x => x.Convert("string", x.Alpha));
        Assert.Equal("((string) x.[Alpha])", token.ToString());
        item = Assert.IsType<DbTokenConvert.ToSpec>(token);
        Assert.Equal("string", item.Type);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Convert_ToType()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenConvert.ToType item;

        token = parser.Parse(x => x.Convert(typeof(string), x.Alpha));
        Assert.Equal("((string) x.[Alpha])", token.ToString());
        item = Assert.IsType<DbTokenConvert.ToType>(token);
        Assert.Equal(typeof(string), item.Type);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Indexed_Argument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenIndexed item;
        DbTokenValue value;
        DbTokenIdentifier id;

        token = parser.Parse(x => x[33]);
        Assert.Equal("x['33']", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token);
        Assert.Single(item.Indexes);
        value = Assert.IsType<DbTokenValue>(item.Indexes[0]);
        Assert.Equal(33, value.Value);

        token = parser.Parse(x => x[x.Alpha]);
        Assert.Equal("x[x.[Alpha]]", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token);
        Assert.Single(item.Indexes);
        id = Assert.IsType<DbTokenIdentifier>(item.Indexes[0]);
        Assert.Equal("[Alpha]", id.Identifier.Value);

        token = parser.Parse(x => x[x[x.Alpha]]);
        Assert.Equal("x[x[x.[Alpha]]]", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token);
        Assert.Single(item.Indexes);
        item = Assert.IsType<DbTokenIndexed>(item.Indexes[0]);
        id = Assert.IsType<DbTokenIdentifier>(item.Indexes[0]);
        Assert.Equal("[Alpha]", id.Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Indexed_Member()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenIndexed item;
        DbTokenIdentifier id;
        DbTokenValue value;

        token = parser.Parse(x => x.Alpha[x.Beta, null, "Other"]);
        Assert.Equal("x.[Alpha][x.[Beta], NULL, 'Other']", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token);
        Assert.Equal(3, item.Indexes.Length);
        id = Assert.IsType<DbTokenIdentifier>(item.Indexes[0]);
        Assert.Equal("[Beta]", id.Identifier.Value);
        value = Assert.IsType<DbTokenValue>(item.Indexes[1]);
        Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(item.Indexes[2]);
        Assert.Equal("Other", value.Value);

        token = parser.Parse(x => x.Alpha[x.Beta[x.Delta[null, "Other"]]]);
        Assert.Equal("x.[Alpha][x.[Beta][x.[Delta][NULL, 'Other']]]", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token); Assert.Single(item.Indexes);
        item = Assert.IsType<DbTokenIndexed>(item.Indexes[0]); Assert.Single(item.Indexes);
        item = Assert.IsType<DbTokenIndexed>(item.Indexes[0]);
        Assert.Equal(2, item.Indexes.Length);
        value = Assert.IsType<DbTokenValue>(item.Indexes[0]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(item.Indexes[1]); Assert.Equal("Other", value.Value);

        token = parser.Parse(x => x.Alpha[x.Alpha[x.Alpha[null, "Other"]]]);
        Assert.Equal("x.[Alpha][x.[Alpha][x.[Alpha][NULL, 'Other']]]", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token); Assert.Single(item.Indexes);
        item = Assert.IsType<DbTokenIndexed>(item.Indexes[0]); Assert.Single(item.Indexes);
        item = Assert.IsType<DbTokenIndexed>(item.Indexes[0]);
        Assert.Equal(2, item.Indexes.Length);
        value = Assert.IsType<DbTokenValue>(item.Indexes[0]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(item.Indexes[1]); Assert.Equal("Other", value.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_FirstLevel_SingleArgument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenInvoke item;
        DbTokenValue value;
        DbTokenIdentifier id;

        token = parser.Parse(x => x());
        Assert.Equal("x()", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        Assert.Empty(item.Arguments);

        token = parser.Parse(x => x(33));
        Assert.Equal("x('33')", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(item.Arguments);
        value = Assert.IsType<DbTokenValue>(item.Arguments[0]);
        Assert.Equal(33, value.Value);

        token = parser.Parse(x => x(x.Alpha));
        Assert.Equal("x(x.[Alpha])", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(item.Arguments);
        id = Assert.IsType<DbTokenIdentifier>(item.Arguments[0]);
        Assert.Equal("[Alpha]", id.Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_FirstLevel_ManyArgument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenInvoke item;
        DbTokenValue value;
        DbTokenIdentifier id;

        token = parser.Parse(x => x("any", "other"));
        Assert.Equal("x('any', 'other')", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        Assert.Equal(2, item.Arguments.Length);
        value = Assert.IsType<DbTokenValue>(item.Arguments[0]); Assert.Equal("any", value.Value);
        value = Assert.IsType<DbTokenValue>(item.Arguments[1]); Assert.Equal("other", value.Value);

        token = parser.Parse(x => x(x.Alpha, x.Beta, null, "any"));
        Assert.Equal("x(x.[Alpha], x.[Beta], NULL, 'any')", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        Assert.Equal(4, item.Arguments.Length);
        id = Assert.IsType<DbTokenIdentifier>(item.Arguments[0]); Assert.Equal("[Alpha]", id.Identifier.Value);
        id = Assert.IsType<DbTokenIdentifier>(item.Arguments[1]); Assert.Equal("[Beta]", id.Identifier.Value);
        value = Assert.IsType<DbTokenValue>(item.Arguments[2]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(item.Arguments[3]); Assert.Equal("any", value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_FirstLevel_Chained()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenInvoke item;
        DbTokenValue value;
        DbTokenIdentifier id;

        token = parser.Parse(x => x(x.Alpha)(33));
        Assert.Equal("x(x.[Alpha])('33')", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(item.Arguments);
        value = Assert.IsType<DbTokenValue>(item.Arguments[0]); Assert.Equal(33, value.Value);
        item = Assert.IsType<DbTokenInvoke>(item.Host);
        Assert.Single(item.Arguments);
        id = Assert.IsType<DbTokenIdentifier>(item.Arguments[0]); Assert.Equal("[Alpha]", id.Identifier.Value);

        token = parser.Parse(x => x(x.Alpha)(x.Beta));
        Assert.Equal("x(x.[Alpha])(x.[Beta])", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(item.Arguments);
        id = Assert.IsType<DbTokenIdentifier>(item.Arguments[0]); Assert.Equal("[Beta]", id.Identifier.Value);
        item = Assert.IsType<DbTokenInvoke>(item.Host);
        Assert.Single(item.Arguments);
        id = Assert.IsType<DbTokenIdentifier>(item.Arguments[0]); Assert.Equal("[Alpha]", id.Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_OnMemberArgument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenInvoke item;
        DbTokenIdentifier id;
        DbTokenArgument arg;

        token = parser.Parse(x => x.x(x.Alpha));
        Assert.Equal("x(x.[Alpha])", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(item.Arguments);
        id = Assert.IsType<DbTokenIdentifier>(item.Arguments[0]); Assert.Equal("[Alpha]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(item.Host); Assert.Equal("x", arg.Name);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_OnMember()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenMethod item;
        DbTokenIdentifier id;
        DbTokenArgument arg;

        token = parser.Parse(x => x.Alpha(x.Beta));
        Assert.Equal("x.Alpha(x.[Beta])", token.ToString());
        item = Assert.IsType<DbTokenMethod>(token);
        Assert.Equal("Alpha", item.Name);
        Assert.Single(item.Arguments);
        id = Assert.IsType<DbTokenIdentifier>(item.Arguments[0]); Assert.Equal("[Beta]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(item.Host); Assert.Equal("x", arg.Name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_ToLiteral()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenLiteral item;
        DbTokenInvoke invoke;
        DbTokenValue value;

        token = parser.Parse(x => x("Other"));
        Assert.Equal("Other", token.ToString());
        item = Assert.IsType<DbTokenLiteral>(token);
        Assert.Equal("Other", item.Value);

        // Two arguments not qualify for conversion to literal...
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
        DbTokenInvoke invoke;
        DbTokenIdentifier id;
        DbTokenLiteral literal;
        DbTokenValue value;

        // Escaping an argument to prevent capturing it...
        token = parser.Parse(x => x(x.Alpha, x("any")));
        Assert.Equal("x(x.[Alpha], any)", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Equal(2, invoke.Arguments.Length);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]); Assert.Equal("[Alpha]", id.Identifier.Value);
        literal = Assert.IsType<DbTokenLiteral>(invoke.Arguments[1]); Assert.Equal("any", literal.Value);

        // Last invoke is not a 1st-level one, so it does not escape...
        token = parser.Parse(x => x(x.Alpha, x(x.Alpha)("33")));
        Assert.Equal("x(x.[Alpha], x(x.[Alpha])('33'))", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Equal(2, invoke.Arguments.Length);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]); Assert.Equal("[Alpha]", id.Identifier.Value);
        invoke = Assert.IsType<DbTokenInvoke>(invoke.Arguments[1]);
        Assert.Single(invoke.Arguments);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[0]); Assert.Equal("33", value.Value);
        invoke = Assert.IsType<DbTokenInvoke>(invoke.Host);
        Assert.Single(invoke.Arguments);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]); Assert.Equal("[Alpha]", id.Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_ToLiteral_Concatenated()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenInvoke invoke;
        DbTokenMethod method;
        DbTokenValue value;

        token = parser.Parse(x => x.Any(x("WHERE ID > ")("007")));
        Assert.Equal("x.Any(WHERE ID > ('007'))", token.ToString());
        method = Assert.IsType<DbTokenMethod>(token);
        Assert.Equal("Any", method.Name);
        Assert.Single(method.Arguments);
        invoke = Assert.IsType<DbTokenInvoke>(method.Arguments[0]);
        Assert.Single(invoke.Arguments);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Member()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    IDbToken token;
    //    xxx item;
    //
    //    token = parser.Parse(x => );
    //    Assert.Equal("", token.ToString());
    //    item = Assert.IsType<xxx>(token);
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Method()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    IDbToken token;
    //    xxx item;
    //
    //    token = parser.Parse(x => );
    //    Assert.Equal("", token.ToString());
    //    item = Assert.IsType<xxx>(token);
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Setter()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    IDbToken token;
    //    xxx item;
    //
    //    token = parser.Parse(x => );
    //    Assert.Equal("", token.ToString());
    //    item = Assert.IsType<xxx>(token);
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Ternary()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    IDbToken token;
    //    xxx item;
    //
    //    token = parser.Parse(x => );
    //    Assert.Equal("", token.ToString());
    //    item = Assert.IsType<xxx>(token);
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Unary()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    IDbToken token;
    //    xxx item;
    //
    //    token = parser.Parse(x => );
    //    Assert.Equal("", token.ToString());
    //    item = Assert.IsType<xxx>(token);
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Value()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    IDbToken token;
    //    xxx item;
    //
    //    token = parser.Parse(x => );
    //    Assert.Equal("", token.ToString());
    //    item = Assert.IsType<xxx>(token);
    //}
}