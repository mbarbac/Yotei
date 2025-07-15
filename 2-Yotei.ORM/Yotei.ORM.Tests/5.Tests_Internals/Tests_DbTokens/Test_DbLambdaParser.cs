using System.ComponentModel.DataAnnotations;

namespace Yotei.ORM.Tests.Internals.DbTokens;

// ========================================================
//[Enforced]
public static class Test_DbLambdaParser
{
    //[Enforced]
    [Fact]
    public static void Parse_Argument()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenArgument arg;

        token = DbLambdaParser.Parse(engine, x => x);
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
        IDbToken token;
        DbTokenBinary item;
        DbTokenValue value;

        token = DbLambdaParser.Parse(engine, x => x.Alpha && x.Beta);
        Assert.Equal("(x.[Alpha] And x.[Beta])", token.ToString());
        item = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, item.Operation);
        Assert.IsType<DbTokenIdentifier>(item.Left);
        Assert.IsType<DbTokenIdentifier>(item.Right);

        token = DbLambdaParser.Parse(engine, x => x.x.Alpha && null!);
        Assert.Equal("(x..[Alpha] And NULL)", token.ToString());
        item = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, item.Operation);
        Assert.IsType<DbTokenIdentifier>(item.Left);
        value = Assert.IsType<DbTokenValue>(item.Right);
        Assert.Null(value.Value);

        token = DbLambdaParser.Parse(engine, x => x.Alpha.x && "any");
        Assert.Equal("(x.[Alpha]. And 'any')", token.ToString());
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
        IDbToken token;
        DbTokenBinary item;

        token = DbLambdaParser.Parse(engine, x => x & "any");
        Assert.Equal("(x And 'any')", token.ToString());
        item = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, item.Operation);
        Assert.IsType<DbTokenArgument>(item.Left);
        Assert.IsType<DbTokenValue>(item.Right);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Others()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenBinary item;

        token = DbLambdaParser.Parse(engine, x => x.Alpha >= x.Beta);
        Assert.Equal("(x.[Alpha] GreaterThanOrEqual x.[Beta])", token.ToString());
        item = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.GreaterThanOrEqual, item.Operation);
        Assert.IsType<DbTokenIdentifier>(item.Left);
        Assert.IsType<DbTokenIdentifier>(item.Right);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Coalesce()
    {
        var engine = new FakeEngine();
        IDbToken token;

        token = DbLambdaParser.Parse(engine, x => x.Coalesce(x.Alpha, x.Beta));
        Assert.Equal("(x.[Alpha] ?? x.[Beta])", token.ToString());
        Assert.IsType<DbTokenCoalesce>(token);

        try
        {
            // The dynamic lambda expression parser cannot parse coalesce expressions, returning
            // just the left element. Hence why we need to use the above's 'x.Coalesce(...)'
            // method...
            token = DbLambdaParser.Parse(engine, x => x.Alpha ?? x.Beta);
            Assert.IsType<DbTokenCoalesce>(token);
            Assert.Fail();
        }
        catch (Xunit.Sdk.IsTypeException) { }

        // Special case when the left operand is a null-alike one...
        token = DbLambdaParser.Parse(engine, x => x.Coalesce(null, x.Beta));
        Assert.Equal("x.[Beta]", token.ToString());
        Assert.IsType<DbTokenIdentifier>(token);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Command()
    {
        IDbToken token;
        DbTokenCommand command;
        DbTokenMethod method;

        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var cmd = new FakeCommand(connection, "SELECT * FROM Employees WHERE Id = {0}", "007");

        token = DbLambdaParser.Parse(engine, x => cmd);
        command = Assert.IsType<DbTokenCommand>(token);
        Assert.Equal("(SELECT * FROM Employees WHERE Id = #0)", token.ToString());
        Assert.Equal(token.ToString(), cmd.GetCommandInfo().Text.Wrap('(', ')'));

        token = DbLambdaParser.Parse(engine, x => x(cmd));
        command = Assert.IsType<DbTokenCommand>(token);
        Assert.Equal("(SELECT * FROM Employees WHERE Id = #0)", token.ToString());
        Assert.Equal(token.ToString(), cmd.GetCommandInfo().Text.Wrap('(', ')'));

        token = DbLambdaParser.Parse(engine, x => x(cmd).As("Any"));
        method = Assert.IsType<DbTokenMethod>(token);
        Assert.Equal("As", method.Name);
        Assert.Single(method.Arguments);
        Assert.Equal("'Any'", method.Arguments[0].ToString());
        command = Assert.IsType<DbTokenCommand>(method.Host);
        Assert.Equal("(SELECT * FROM Employees WHERE Id = #0)", command.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Convert_Argument_ToType()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenConvert.ToType item;

        token = DbLambdaParser.Parse(engine, x => (string)x);
        Assert.Equal("((String) x)", token.ToString());
        item = Assert.IsType<DbTokenConvert.ToType>(token);
        Assert.Equal(typeof(string), item.Type);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Convert_Member_ToType()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenConvert.ToType item;

        token = DbLambdaParser.Parse(engine, x => (string)x.Alpha);
        Assert.Equal("((String) x.[Alpha])", token.ToString());
        item = Assert.IsType<DbTokenConvert.ToType>(token);
        Assert.Equal(typeof(string), item.Type);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Convert_Member_ToSpec()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenConvert.ToSpec spec;

        token = DbLambdaParser.Parse(engine, x => x.Convert("string", x.Alpha));
        Assert.Equal("((string) x.[Alpha])", token.ToString());
        spec = Assert.IsType<DbTokenConvert.ToSpec>(token);
        Assert.Equal("string", spec.Type);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Indexed_Argument()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenIndexed item;
        DbTokenValue value;

        token = DbLambdaParser.Parse(engine, x => x[27]);
        Assert.Equal("x['27']", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token);
        Assert.Single(item.Indexes);
        value = Assert.IsType<DbTokenValue>(item.Indexes[0]);
        Assert.Equal(27, value.Value);

        token = DbLambdaParser.Parse(engine, x => x[x.Alpha]);
        Assert.Equal("x[x.[Alpha]]", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token);
        Assert.Single(item.Indexes);
        Assert.IsType<DbTokenIdentifier>(item.Indexes[0]);

        token = DbLambdaParser.Parse(engine, x => x[x[x.Alpha]]);
        Assert.Equal("x[x[x.[Alpha]]]", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token);
        Assert.Single(item.Indexes);
        item = Assert.IsType<DbTokenIndexed>(item.Indexes[0]);
        Assert.Single(item.Indexes);
        Assert.IsType<DbTokenIdentifier>(item.Indexes[0]);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Indexed_Member()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenIndexed item;
        DbTokenIdentifier id;
        DbTokenValue value;

        token = DbLambdaParser.Parse(engine, x => x.Alpha[x.Beta, null, "Other"]);
        Assert.Equal("x.[Alpha][x.[Beta], NULL, 'Other']", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token);
        Assert.Equal(3, item.Indexes.Count);
        Assert.IsType<DbTokenIdentifier>(item.Indexes[0]);
        value = Assert.IsType<DbTokenValue>(item.Indexes[1]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(item.Indexes[2]); Assert.Equal("Other", value.Value);

        token = DbLambdaParser.Parse(engine, x => x.Alpha[x.Beta[x.Delta[null, "Other"]]]);
        Assert.Equal("x.[Alpha][x.[Beta][x.[Delta][NULL, 'Other']]]", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token);
        id = Assert.IsType<DbTokenIdentifier>(item.Host); Assert.Equal("[Alpha]", id.Value);
        Assert.Single(item.Indexes);
        item = Assert.IsType<DbTokenIndexed>(item.Indexes[0]);
        id = Assert.IsType<DbTokenIdentifier>(item.Host); Assert.Equal("[Beta]", id.Value);
        Assert.Single(item.Indexes);
        item = Assert.IsType<DbTokenIndexed>(item.Indexes[0]);
        id = Assert.IsType<DbTokenIdentifier>(item.Host); Assert.Equal("[Delta]", id.Value);
        Assert.Equal(2, item.Indexes.Count);
        value = Assert.IsType<DbTokenValue>(item.Indexes[0]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(item.Indexes[1]); Assert.Equal("Other", value.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_Single_Argument()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenInvoke invoke;
        DbTokenIdentifier id;
        DbTokenValue value;

        token = DbLambdaParser.Parse(engine, x => x());
        Assert.Equal("x()", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Empty(invoke.Arguments);

        token = DbLambdaParser.Parse(engine, x => x(50));
        Assert.Equal("x('50')", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(invoke.Arguments);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[0]); Assert.Equal(50, value.Value);

        token = DbLambdaParser.Parse(engine, x => x(x.Alpha));
        Assert.Equal("x(x.[Alpha])", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(invoke.Arguments);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]);
        Assert.Equal("[Alpha]", id.Value);

        token = DbLambdaParser.Parse(engine, x => x.x(x.Alpha));
        Assert.Equal("x(x.[Alpha])", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(invoke.Arguments);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]);
        Assert.Equal("[Alpha]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_Many_Arguments()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenInvoke invoke;
        DbTokenValue value;

        token = DbLambdaParser.Parse(engine, x => x("any", "other"));
        Assert.Equal("x('any', 'other')", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Equal(2, invoke.Arguments.Count);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[0]); Assert.Equal("any", value.Value);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[1]); Assert.Equal("other", value.Value);

        token = DbLambdaParser.Parse(engine, x => x(x.Alpha, x.Beta, null, "any"));
        Assert.Equal("x(x.[Alpha], x.[Beta], NULL, 'any')", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Equal(4, invoke.Arguments.Count);
        Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]);
        Assert.IsType<DbTokenIdentifier>(invoke.Arguments[1]);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[2]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[3]); Assert.Equal("any", value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_Chained()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenInvoke invoke;
        DbTokenIdentifier id;
        DbTokenValue value;

        token = DbLambdaParser.Parse(engine, x => x(x.Alpha)(33));
        Assert.Equal("x(x.[Alpha])('33')", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(invoke.Arguments);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[0]);
        Assert.Equal(33, value.Value);

        token = DbLambdaParser.Parse(engine, x => x(x.Alpha)(x.Beta));
        Assert.Equal("x(x.[Alpha])(x.[Beta])", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]);
        Assert.Equal("[Beta]", id.Value);
        invoke = Assert.IsType<DbTokenInvoke>(invoke.Host);
        Assert.IsType<DbTokenArgument>(invoke.Host);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]);
        Assert.Equal("[Alpha]", id.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_Escape()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenInvoke invoke;
        DbTokenLiteral text;

        // First-level sole string are translated to literals...
        token = DbLambdaParser.Parse(engine, x => x("any"));
        Assert.Equal("x(any)", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(invoke.Arguments);
        text = Assert.IsType<DbTokenLiteral>(invoke.Arguments[0]);
        Assert.Equal("any", text.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_Escape_Chained()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenInvoke invoke;
        DbTokenLiteral text;
        DbTokenValue value;

        // Escaping an argument to prevent capturing it...
        token = DbLambdaParser.Parse(engine, x => x(x.Alpha, x("any")));
        Assert.Equal("x(x.[Alpha], x(any))", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Equal(2, invoke.Arguments.Count);
        Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]);
        invoke = Assert.IsType<DbTokenInvoke>(invoke.Arguments[1]);
        Assert.Single(invoke.Arguments);
        text = Assert.IsType<DbTokenLiteral>(invoke.Arguments[0]); Assert.Equal("any", text.Value);

        // Because we concatenate, the last invoke is not a 1st-level one, and so it does not
        // escape its argument...
        token = DbLambdaParser.Parse(engine, x => x(x.Alpha)("33"));
        Assert.Equal("x(x.[Alpha])('33')", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(invoke.Arguments);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[0]);
        Assert.Equal("33", value.Value);

        // To un-scape, we need to have a 1st-level single-string invoke..
        token = DbLambdaParser.Parse(engine, x => x(x.Alpha)(x("33")));
        Assert.Equal("x(x.[Alpha])(x(33))", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(invoke.Arguments);
        invoke = Assert.IsType<DbTokenInvoke>(invoke.Arguments[0]);
        Assert.Single(invoke.Arguments);
        text = Assert.IsType<DbTokenLiteral>(invoke.Arguments[0]);
        Assert.Equal("33", text.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_Un_Escape()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenInvoke invoke;
        DbTokenLiteral literal;
        DbTokenMethod method;

        // Problem: concatenation requires chained invocation, but the argument of the 2nd
        // is escaped as a literal instead of being captured as an argument.
        token = DbLambdaParser.Parse(engine, x => x.Any(x("WHERE ID > ")("007")));
        //Assert.Equal("x.Any(x(WHERE ID > )(007))", token.ToString());
        //method = Assert.IsType<DbTokenMethod>(token);
        //Assert.Single(method.Arguments);
        //invoke = Assert.IsType<DbTokenInvoke>(method.Arguments[0]);
        //Assert.Single(invoke.Arguments);
        //literal = Assert.IsType<DbTokenLiteral>(invoke.Arguments[0]);
        //Assert.Equal("007", literal.Value);
        //invoke = Assert.IsType<DbTokenInvoke>(invoke.Host);
        //Assert.Single(invoke.Arguments);
        //literal = Assert.IsType<DbTokenLiteral>(invoke.Arguments[0]);
        //Assert.Equal("WHERE ID > ", literal.Value);

        var connection = new FakeConnection(engine);
        var visitor = new DbTokenVisitor(connection);
        var builder = visitor.Visit(token);
        Assert.Equal("Any(WHERE ID > 007)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Member_Standard()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenIdentifier item;
        DbTokenArgument arg;

        token = DbLambdaParser.Parse(engine, x => x.Alpha);
        Assert.Equal("x.[Alpha]", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        Assert.Equal("Alpha", item.Identifier.UnwrappedValue);
        Assert.True(item.IsPureIdentifier);
        arg = Assert.IsType<DbTokenArgument>(item.Host);
        Assert.Equal("x", arg.Name);

        token = DbLambdaParser.Parse(engine, x => x.Alpha.Beta);
        Assert.Equal("x.[Alpha].[Beta]", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        item = Assert.IsType<DbTokenIdentifier>(item.Host);
        arg = Assert.IsType<DbTokenArgument>(item.Host);

        token = DbLambdaParser.Parse(engine, x => x.Alpha.Beta.Delta);
        Assert.Equal("x.[Alpha].[Beta].[Delta]", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        item = Assert.IsType<DbTokenIdentifier>(item.Host);
        item = Assert.IsType<DbTokenIdentifier>(item.Host);
        arg = Assert.IsType<DbTokenArgument>(item.Host);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Member_WithDynamicArgument()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenIdentifier item;
        DbTokenArgument arg;

        token = DbLambdaParser.Parse(engine, x => x.x.Alpha);
        Assert.Equal("x..[Alpha]", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        Assert.True(item.IsPureIdentifier);
        item = Assert.IsType<DbTokenIdentifier>(item.Host);
        Assert.Null(item.Value);
        arg = Assert.IsType<DbTokenArgument>(item.Host);
        Assert.Equal("x", arg.Name);

        token = DbLambdaParser.Parse(engine, x => x.Alpha.x);
        Assert.Equal("x.[Alpha].", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        Assert.Null(item.Value);
        Assert.True(item.IsPureIdentifier);
        item = Assert.IsType<DbTokenIdentifier>(item.Host);
        arg = Assert.IsType<DbTokenArgument>(item.Host);

        token = DbLambdaParser.Parse(engine, x => x.x);
        Assert.Equal("x.", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        Assert.Null(item.Value);
        Assert.True(item.IsPureIdentifier);
        arg = Assert.IsType<DbTokenArgument>(item.Host);

        token = DbLambdaParser.Parse(engine, x => x.x.x);
        Assert.Equal("x..", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        Assert.Null(item.Value);
        Assert.True(item.IsPureIdentifier);
        item = Assert.IsType<DbTokenIdentifier>(item.Host);
        arg = Assert.IsType<DbTokenArgument>(item.Host);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Method_OnArgument()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenMethod item;
        DbTokenIdentifier name;
        DbTokenValue value;

        token = DbLambdaParser.Parse(engine, x => x.Alpha());
        Assert.Equal("x.Alpha()", token.ToString());
        item = Assert.IsType<DbTokenMethod>(token);
        Assert.Empty(item.Arguments);

        token = DbLambdaParser.Parse(engine, x => x.Alpha(x.Beta, null, 50));
        Assert.Equal("x.Alpha(x.[Beta], NULL, '50')", token.ToString());
        item = Assert.IsType<DbTokenMethod>(token);
        Assert.Equal(3, item.Arguments.Count);
        name = Assert.IsType<DbTokenIdentifier>(item.Arguments[0]); Assert.Equal("Beta", name.Identifier.UnwrappedValue);
        value = Assert.IsType<DbTokenValue>(item.Arguments[1]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(item.Arguments[2]); Assert.Equal(50, value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_OnMember()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenMethod item;

        token = DbLambdaParser.Parse(engine, x => x.Alpha.Beta());
        Assert.Equal("x.[Alpha].Beta()", token.ToString());
        item = Assert.IsType<DbTokenMethod>(token);
        Assert.Empty(item.Arguments);

        token = DbLambdaParser.Parse(engine, x => x.Alpha.Beta(x.Delta, null, 50));
        Assert.Equal("x.[Alpha].Beta(x.[Delta], NULL, '50')", token.ToString());
        item = Assert.IsType<DbTokenMethod>(token);
        Assert.Equal(3, item.Arguments.Count);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_WithGenerics()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenMethod item;

        token = DbLambdaParser.Parse(engine, x => x.Whatever<string>(x.Alpha));
        Assert.Equal("x.Whatever<String>(x.[Alpha])", token.ToString());
        item = Assert.IsType<DbTokenMethod>(token);
        Assert.Single(item.Arguments);
        Assert.IsType<DbTokenIdentifier>(item.Arguments[0]);
        Assert.Single(item.TypeArguments);
        Assert.IsType<Type>(item.TypeArguments[0], exactMatch: false);

        token = DbLambdaParser.Parse(engine, x => x.Whatever<string, int>(x.Alpha));
        Assert.Equal("x.Whatever<String, Int32>(x.[Alpha])", token.ToString());
        item = Assert.IsType<DbTokenMethod>(token);
        Assert.Single(item.Arguments);
        Assert.IsType<DbTokenIdentifier>(item.Arguments[0]);
        Assert.Equal(2, item.TypeArguments.Length);
        Assert.True(item.TypeArguments[0] == typeof(string));
        Assert.True(item.TypeArguments[1] == typeof(int));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Method_AsInvoke_Standard()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenInvoke item;

        token = DbLambdaParser.Parse(engine, x => x.Alpha.x());
        Assert.Equal("x.[Alpha]()", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        Assert.Empty(item.Arguments);

        token = DbLambdaParser.Parse(engine, x => x(x.Alpha).x(x.Beta));
        Assert.Equal("x(x.[Alpha])(x.[Beta])", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        item = Assert.IsType<DbTokenInvoke>(item.Host);
        Assert.IsType<DbTokenArgument>(item.Host);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_AsInvoke_ToLiteral()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenIdentifier id;
        DbTokenInvoke invoke;
        DbTokenLiteral text;

        token = DbLambdaParser.Parse(engine, x => x("any"));
        Assert.Equal("x(any)", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(invoke.Arguments);
        text = Assert.IsType<DbTokenLiteral>(invoke.Arguments[0]);
        Assert.Equal("any", text.Value);

        token = DbLambdaParser.Parse(engine, x => x.Alpha.x("any"));
        Assert.Equal("x.[Alpha](any)", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        id = Assert.IsType<DbTokenIdentifier>(invoke.Host);
        Assert.Equal("[Alpha]", id.Value);
        Assert.Single(invoke.Arguments);
        text = Assert.IsType<DbTokenLiteral>(invoke.Arguments[0]);
        Assert.Equal("any", text.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_ArgumentInvokeString_As_LiteralArgument()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenMethod item;
        DbTokenInvoke invoke;
        DbTokenLiteral text;

        token = DbLambdaParser.Parse(engine, x => x.Alpha.Beta(x.Delta, null, x("any")));
        Assert.Equal("x.[Alpha].Beta(x.[Delta], NULL, x(any))", token.ToString());
        item = Assert.IsType<DbTokenMethod>(token);
        Assert.Equal(3, item.Arguments.Count);
        Assert.IsType<DbTokenIdentifier>(item.Arguments[0]);
        Assert.IsType<DbTokenValue>(item.Arguments[1]);

        invoke = Assert.IsType<DbTokenInvoke>(item.Arguments[2]);
        text = Assert.IsType<DbTokenLiteral>(invoke.Arguments[0]);
        Assert.Equal("any", text.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Method_AsConvert_ToType()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenConvert.ToType toType;

        token = DbLambdaParser.Parse(engine, x => x.Cast(typeof(string), x.Alpha));
        Assert.Equal("((String) x.[Alpha])", token.ToString());
        toType = Assert.IsType<DbTokenConvert.ToType>(token);
        Assert.True(toType.Type == typeof(string));

        token = DbLambdaParser.Parse(engine, x => x.Cast<int>(x.Alpha));
        Assert.Equal("((Int32) x.[Alpha])", token.ToString());
        toType = Assert.IsType<DbTokenConvert.ToType>(token);
        Assert.Equal(typeof(int), toType.Type);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_AsConvert_ToSpecification()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenConvert.ToSpec toSpec;

        token = DbLambdaParser.Parse(engine, x => x.Cast("varchar", x.Alpha));
        Assert.Equal("((varchar) x.[Alpha])", token.ToString());
        toSpec = Assert.IsType<DbTokenConvert.ToSpec>(token);
        Assert.Equal("varchar", toSpec.Type);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Setter_OnArgument()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenArgument arg;
        DbTokenValue value;

#pragma warning disable CS1717
        token = DbLambdaParser.Parse(engine, x => x = x);
        Assert.Equal("x", token.ToString());
        arg = Assert.IsType<DbTokenArgument>(token);
#pragma warning restore

        token = DbLambdaParser.Parse(engine, x => x = "007");
        Assert.Equal("'007'", token.ToString());
        value = Assert.IsType<DbTokenValue>(token);
        Assert.Equal("007", value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Setter_OnMember()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenSetter item;
        DbTokenValue value;

        token = DbLambdaParser.Parse(engine, x => x.Alpha = "007");
        Assert.Equal("(x.[Alpha] = '007')", token.ToString());
        item = Assert.IsType<DbTokenSetter>(token);
        Assert.IsType<DbTokenIdentifier>(item.Target);
        value = Assert.IsType<DbTokenValue>(item.Value);
        Assert.Equal("007", value.Value);

        token = DbLambdaParser.Parse(engine, x => x.Alpha = x);
        Assert.Equal("(x.[Alpha] = x)", token.ToString());
        item = Assert.IsType<DbTokenSetter>(token);
        Assert.IsType<DbTokenIdentifier>(item.Target);
        Assert.IsType<DbTokenArgument>(item.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Setter_Dynamic()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenSetter item;

        token = DbLambdaParser.Parse(engine, x => x.Alpha = x.Beta);
        Assert.Equal("(x.[Alpha] = x.[Beta])", token.ToString());
        item = Assert.IsType<DbTokenSetter>(token);
        Assert.IsType<DbTokenIdentifier>(item.Target);
        Assert.IsType<DbTokenIdentifier>(item.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Ternary()
    {
        var engine = new FakeEngine();
        IDbToken token;

        token = DbLambdaParser.Parse(engine, x => x.Ternary(x.Alpha, x.Beta, x.Delta));
        Assert.Equal("(x.[Alpha] ? x.[Beta] : x.[Delta])", token.ToString());
        Assert.IsType<DbTokenTernary>(token);

        try
        {
            // The dynamic lambda expression parser cannot parse ternary expressions, returning
            // just the left element. Hence why we need to use the above's 'x.Coalesce(...)'
            // method...

            token = DbLambdaParser.Parse(engine, x => x.Alpha ? x.Beta : x.Delta);
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
        IDbToken token;
        DbTokenUnary item;

        token = DbLambdaParser.Parse(engine, x => !x);
        Assert.Equal("(Not x)", token.ToString());
        item = Assert.IsType<DbTokenUnary>(token);
        Assert.IsType<DbTokenArgument>(item.Target);

        token = DbLambdaParser.Parse(engine, x => !x.x);
        Assert.Equal("(Not x.)", token.ToString());
        item = Assert.IsType<DbTokenUnary>(token);
        Assert.IsType<DbTokenIdentifier>(item.Target);

        token = DbLambdaParser.Parse(engine, x => !x.x.x);
        Assert.Equal("(Not x..)", token.ToString());
        item = Assert.IsType<DbTokenUnary>(token);
        Assert.IsType<DbTokenIdentifier>(item.Target);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Unary_OnMember()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenUnary item;

        token = DbLambdaParser.Parse(engine, x => !x.Alpha);
        Assert.Equal("(Not x.[Alpha])", token.ToString());
        item = Assert.IsType<DbTokenUnary>(token);
        Assert.IsType<DbTokenIdentifier>(item.Target);

        token = DbLambdaParser.Parse(engine, x => -x.Alpha);
        Assert.Equal("(Negate x.[Alpha])", token.ToString());
        item = Assert.IsType<DbTokenUnary>(token);
        Assert.IsType<DbTokenIdentifier>(item.Target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Value_Standard()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenValue item;

        token = DbLambdaParser.Parse(engine, x => null);
        Assert.Equal("NULL", token.ToString());
        item = Assert.IsType<DbTokenValue>(token);
        Assert.Null(item.Value);

        token = DbLambdaParser.Parse(engine, x => true);
        Assert.Equal("TRUE", token.ToString());
        item = Assert.IsType<DbTokenValue>(token);
        Assert.True((bool)item.Value!);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Value_First_Level_Strings()
    {
        var engine = new FakeEngine();
        IDbToken token;
        DbTokenValue item;

        token = DbLambdaParser.Parse(engine, x => "any");
        Assert.Equal("'any'", token.ToString());
        item = Assert.IsType<DbTokenValue>(token);
        Assert.Equal("any", item.Value);
    }
}