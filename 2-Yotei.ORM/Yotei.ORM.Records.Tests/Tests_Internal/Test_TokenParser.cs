using Xunit.Sdk;

namespace Yotei.ORM.Records.Tests;

// ========================================================
//[Enforced]
public static class Test_TokenParser
{
    //[Enforced]
    [Fact]
    public static void Parse_Argument()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenArgument item;

        token = parser.Parse(x => x);
        Assert.Equal("x", token.ToString());
        item = Assert.IsType<TokenArgument>(token);
        Assert.Equal("x", item.Name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Binary_And()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenBinary item;
        TokenValue value;

        token = parser.Parse(x => x.Alpha && x.Beta);
        Assert.Equal("(x.[Alpha] And x.[Beta])", token.ToString());
        item = Assert.IsType<TokenBinary>(token);
        Assert.Equal(ExpressionType.And, item.Operation);
        Assert.IsType<TokenIdentifier>(item.Left);
        Assert.IsType<TokenIdentifier>(item.Right);

        token = parser.Parse(x => x.x.Alpha && null!);
        Assert.Equal("(x..[Alpha] And NULL)", token.ToString());
        item = Assert.IsType<TokenBinary>(token);
        Assert.Equal(ExpressionType.And, item.Operation);
        Assert.IsType<TokenIdentifier>(item.Left);
        value = Assert.IsType<TokenValue>(item.Right);
        Assert.Null(value.Value);

        token = parser.Parse(x => x.Alpha.x && "any");
        Assert.Equal("(x.[Alpha]. And 'any')", token.ToString());
        item = Assert.IsType<TokenBinary>(token);
        Assert.Equal(ExpressionType.And, item.Operation);
        Assert.IsType<TokenIdentifier>(item.Left);
        value = Assert.IsType<TokenValue>(item.Right);
        Assert.Equal("any", value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_SingleAnd()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenBinary item;

        token = parser.Parse(x => x & "any");
        Assert.Equal("(x And 'any')", token.ToString());
        item = Assert.IsType<TokenBinary>(token);
        Assert.Equal(ExpressionType.And, item.Operation);
        Assert.IsType<TokenArgument>(item.Left);
        Assert.IsType<TokenValue>(item.Right);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Binary_Others()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenBinary item;

        token = parser.Parse(x => x.Alpha >= x.Beta);
        Assert.Equal("(x.[Alpha] GreaterThanOrEqual x.[Beta])", token.ToString());
        item = Assert.IsType<TokenBinary>(token);
        Assert.Equal(ExpressionType.GreaterThanOrEqual, item.Operation);
        Assert.IsType<TokenIdentifier>(item.Left);
        Assert.IsType<TokenIdentifier>(item.Right);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Coalesce()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;

        try
        {
            token = parser.Parse(x => x.Alpha ?? x.Beta);
            Assert.IsType<TokenCoalesce>(token);
            Assert.Fail();
        }
        catch (IsTypeException) { }

        token = parser.Parse(x => x.Coalesce(x.Alpha, x.Beta));
        Assert.Equal("(x.[Alpha] ?? x.[Beta])", token.ToString());
        Assert.IsType<TokenCoalesce>(token);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Convert_Argument_To_Type()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenConvert.ToType item;

        token = parser.Parse(x => (string)x);
        Assert.Equal("((String) x)", token.ToString());
        item = Assert.IsType<TokenConvert.ToType>(token);
        Assert.Equal(typeof(string), item.Type);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Convert_Member_To_Type()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenConvert.ToType item;

        token = parser.Parse(x => (string)x.Alpha);
        Assert.Equal("((String) x.[Alpha])", token.ToString());
        item = Assert.IsType<TokenConvert.ToType>(token);
        Assert.Equal(typeof(string), item.Type);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Indexed_Argument()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenIndexed item;
        TokenValue value;

        token = parser.Parse(x => x[27]);
        Assert.Equal("x['27']", token.ToString());
        item = Assert.IsType<TokenIndexed>(token);
        Assert.Equal(1, item.Indexes.Count);
        value = Assert.IsType<TokenValue>(item.Indexes[0]);
        Assert.Equal(27, value.Value);

        token = parser.Parse(x => x[x.Alpha]);
        Assert.Equal("x[x.[Alpha]]", token.ToString());
        item = Assert.IsType<TokenIndexed>(token);
        Assert.Equal(1, item.Indexes.Count);
        Assert.IsType<TokenIdentifier>(item.Indexes[0]);

        token = parser.Parse(x => x[x[x.Alpha]]);
        Assert.Equal("x[x[x.[Alpha]]]", token.ToString());
        item = Assert.IsType<TokenIndexed>(token);
        Assert.Equal(1, item.Indexes.Count);
        item = Assert.IsType<TokenIndexed>(item.Indexes[0]);
        Assert.Equal(1, item.Indexes.Count);
        Assert.IsType<TokenIdentifier>(item.Indexes[0]);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Indexed_Member()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenIndexed item;
        TokenValue value;

        token = parser.Parse(x => x.Alpha[x.Beta, null, "Other"]);
        Assert.Equal("x.[Alpha][x.[Beta], NULL, 'Other']", token.ToString());
        item = Assert.IsType<TokenIndexed>(token);
        Assert.Equal(3, item.Indexes.Count);
        Assert.IsType<TokenIdentifier>(item.Indexes[0]);
        value = Assert.IsType<TokenValue>(item.Indexes[1]); Assert.Null(value.Value);
        value = Assert.IsType<TokenValue>(item.Indexes[2]); Assert.Equal("Other", value.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_On_Argument()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenInvoke item;
        TokenValue value;

        token = parser.Parse(x => x());
        Assert.Equal("x()", token.ToString());
        item = Assert.IsType<TokenInvoke>(token);
        Assert.Empty(item.Arguments);

        token = parser.Parse(x => x(50));
        Assert.Equal("x('50')", token.ToString());
        item = Assert.IsType<TokenInvoke>(token);
        Assert.Single(item.Arguments);
        value = Assert.IsType<TokenValue>(item.Arguments[0]); Assert.Equal(50, value.Value);

        token = parser.Parse(x => x(x.Alpha, x.Beta, null, "any"));
        Assert.Equal("x(x.[Alpha], x.[Beta], NULL, 'any')", token.ToString());
        item = Assert.IsType<TokenInvoke>(token);
        Assert.Equal(4, item.Arguments.Count);
        Assert.IsType<TokenIdentifier>(item.Arguments[0]);
        Assert.IsType<TokenIdentifier>(item.Arguments[1]);
        value = Assert.IsType<TokenValue>(item.Arguments[2]); Assert.Null(value.Value);
        value = Assert.IsType<TokenValue>(item.Arguments[3]); Assert.Equal("any", value.Value);

        token = parser.Parse(x => x(null, x(x.Beta), 50));
        Assert.Equal("x(NULL, x(x.[Beta]), '50')", token.ToString());
        item = Assert.IsType<TokenInvoke>(token);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_Chained()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenInvoke item;

        token = parser.Parse(x => x(x.Alpha)(x.Beta));
        Assert.Equal("x(x.[Alpha])(x.[Beta])", token.ToString());
        item = Assert.IsType<TokenInvoke>(token);
        item = Assert.IsType<TokenInvoke>(item.Host);
        Assert.IsType<TokenArgument>(item.Host);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenInvoke item;

        token = parser.Parse(x => x("any"));
        Assert.Equal("x(any)", token.ToString());
        item = Assert.IsType<TokenInvoke>(token);
        Assert.Single(item.Arguments);
        Assert.IsType<TokenLiteral>(item.Arguments[0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Member()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenIdentifier item;
        TokenArgument arg;

        token = parser.Parse(x => x.Alpha);
        Assert.Equal("x.[Alpha]", token.ToString());
        item = Assert.IsType<TokenIdentifier>(token);
        Assert.Equal("Alpha", item.Identifier.UnwrappedValue);
        Assert.True(item.IsPureIdentifier);
        arg = Assert.IsType<TokenArgument>(item.Host);
        Assert.Equal("x", arg.Name);

        token = parser.Parse(x => x.Alpha.Beta);
        Assert.Equal("x.[Alpha].[Beta]", token.ToString());
        item = Assert.IsType<TokenIdentifier>(token);
        item = Assert.IsType<TokenIdentifier>(item.Host);
        arg = Assert.IsType<TokenArgument>(item.Host);

        token = parser.Parse(x => x.Alpha.Beta.Delta);
        Assert.Equal("x.[Alpha].[Beta].[Delta]", token.ToString());
        item = Assert.IsType<TokenIdentifier>(token);
        item = Assert.IsType<TokenIdentifier>(item.Host);
        item = Assert.IsType<TokenIdentifier>(item.Host);
        arg = Assert.IsType<TokenArgument>(item.Host);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Member_With_Dynamic()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenIdentifier item;
        TokenArgument arg;

        token = parser.Parse(x => x.x.Alpha);
        Assert.Equal("x..[Alpha]", token.ToString());
        item = Assert.IsType<TokenIdentifier>(token);
        Assert.Equal("Alpha", item.Identifier.UnwrappedValue);
        Assert.True(item.IsPureIdentifier);
        item = Assert.IsType<TokenIdentifier>(item.Host);
        Assert.Null(item.Identifier.Value);
        arg = Assert.IsType<TokenArgument>(item.Host);
        Assert.Equal("x", arg.Name);

        token = parser.Parse(x => x.Alpha.x);
        Assert.Equal("x.[Alpha].", token.ToString());
        item = Assert.IsType<TokenIdentifier>(token);
        Assert.Null(item.Identifier.Value);
        Assert.True(item.IsPureIdentifier);
        item = Assert.IsType<TokenIdentifier>(item.Host);
        arg = Assert.IsType<TokenArgument>(item.Host);

        token = parser.Parse(x => x.x.x);
        Assert.Equal("x..", token.ToString());
        item = Assert.IsType<TokenIdentifier>(token);
        Assert.Null(item.Identifier.Value);
        Assert.True(item.IsPureIdentifier);
        item = Assert.IsType<TokenIdentifier>(item.Host);
        arg = Assert.IsType<TokenArgument>(item.Host);

        token = parser.Parse(x => x.x);
        Assert.Equal("x.", token.ToString());
        item = Assert.IsType<TokenIdentifier>(token);
        Assert.Null(item.Identifier.Value);
        Assert.True(item.IsPureIdentifier);
        arg = Assert.IsType<TokenArgument>(item.Host);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Method_On_Argument()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenMethod item;
        TokenIdentifier name;
        TokenValue value;

        token = parser.Parse(x => x.Alpha());
        Assert.Equal("x.Alpha()", token.ToString());
        item = Assert.IsType<TokenMethod>(token);
        Assert.Empty(item.Arguments);

        token = parser.Parse(x => x.Alpha(x.Beta, null, 50));
        Assert.Equal("x.Alpha(x.[Beta], NULL, '50')", token.ToString());
        item = Assert.IsType<TokenMethod>(token);
        Assert.Equal(3, item.Arguments.Count);
        name = Assert.IsType<TokenIdentifier>(item.Arguments[0]); Assert.Equal("Beta", name.Identifier.UnwrappedValue);
        value = Assert.IsType<TokenValue>(item.Arguments[1]); Assert.Null(value.Value);
        value = Assert.IsType<TokenValue>(item.Arguments[2]); Assert.Equal(50, value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_On_Member()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenMethod item;

        token = parser.Parse(x => x.Alpha.Beta());
        Assert.Equal("x.[Alpha].Beta()", token.ToString());
        item = Assert.IsType<TokenMethod>(token);
        Assert.Empty(item.Arguments);

        token = parser.Parse(x => x.Alpha.Beta(x.Delta, null, 50));
        Assert.Equal("x.[Alpha].Beta(x.[Delta], NULL, '50')", token.ToString());
        item = Assert.IsType<TokenMethod>(token);
        Assert.Equal(3, item.Arguments.Count);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_With_Generics()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenMethod item;

        token = parser.Parse(x => x.Whatever<string>(x.Alpha));
        Assert.Equal("x.Whatever<String>(x.[Alpha])", token.ToString());
        item = Assert.IsType<TokenMethod>(token);
        Assert.Single(item.Arguments);
        Assert.IsType<TokenIdentifier>(item.Arguments[0]);
        Assert.Single(item.TypeArguments);
        Assert.IsAssignableFrom<Type>(item.TypeArguments[0]);

        token = parser.Parse(x => x.Whatever<string, int>(x.Alpha));
        Assert.Equal("x.Whatever<String, Int32>(x.[Alpha])", token.ToString());
        item = Assert.IsType<TokenMethod>(token);
        Assert.Single(item.Arguments);
        Assert.IsType<TokenIdentifier>(item.Arguments[0]);
        Assert.Equal(2, item.TypeArguments.Length);
        Assert.True(item.TypeArguments[0] == typeof(string));
        Assert.True(item.TypeArguments[1] == typeof(int));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Method_As_Invoke()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenInvoke item;

        token = parser.Parse(x => x.Alpha.x());
        Assert.Equal("x.[Alpha]()", token.ToString());
        item = Assert.IsType<TokenInvoke>(token);
        Assert.Empty(item.Arguments);

        token = parser.Parse(x => x(x.Alpha).x(x.Beta));
        Assert.Equal("x(x.[Alpha])(x.[Beta])", token.ToString());
        item = Assert.IsType<TokenInvoke>(token);
        item = Assert.IsType<TokenInvoke>(item.Host);
        Assert.IsType<TokenArgument>(item.Host);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_As_Invoke_ToLiteral()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenInvoke item;

        token = parser.Parse(x => x("any"));
        Assert.Equal("x(any)", token.ToString());
        item = Assert.IsType<TokenInvoke>(token);
        Assert.Single(item.Arguments);
        Assert.IsType<TokenLiteral>(item.Arguments[0]);

        token = parser.Parse(x => x.Alpha.x("any"));
        Assert.Equal("x.[Alpha](any)", token.ToString());
        item = Assert.IsType<TokenInvoke>(token);
        Assert.Single(item.Arguments);
        Assert.IsType<TokenLiteral>(item.Arguments[0]);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_Embedded_Literal_As_Invoke_Argument()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenMethod item;
        TokenInvoke invoke;

        token = parser.Parse(x => x.Alpha.Beta(x.Delta, null, x("any")));
        Assert.Equal("x.[Alpha].Beta(x.[Delta], NULL, x(any))", token.ToString());
        item = Assert.IsType<TokenMethod>(token);
        Assert.Equal(3, item.Arguments.Count);

        invoke = Assert.IsType<TokenInvoke>(item.Arguments[2]);
        Assert.Single(invoke.Arguments);
        Assert.IsType<TokenLiteral>(invoke.Arguments[0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Method_As_Convert_To_Type()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenConvert.ToType toType;

        token = parser.Parse(x => x.Cast(typeof(string), x.Alpha));
        Assert.Equal("((String) x.[Alpha])", token.ToString());
        toType = Assert.IsType<TokenConvert.ToType>(token);
        Assert.True(toType.Type == typeof(string));

        token = parser.Parse(x => x.Cast<int>(x.Alpha));
        Assert.Equal("((Int32) x.[Alpha])", token.ToString());
        toType = Assert.IsType<TokenConvert.ToType>(token);
        Assert.Equal(typeof(int), toType.Type);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_As_Convert_To_Specification()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenConvert.ToSpec toSpec;

        token = parser.Parse(x => x.Cast("varchar", x.Alpha));
        Assert.Equal("((varchar) x.[Alpha])", token.ToString());
        toSpec = Assert.IsType<TokenConvert.ToSpec>(token);
        Assert.Equal("varchar", toSpec.Type);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Method_As_Coalesce()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenCoalesce item;

        token = parser.Parse(x => x.Coalesce(x.Alpha, x.Beta));
        Assert.Equal("(x.[Alpha] ?? x.[Beta])", token.ToString());
        item = Assert.IsType<TokenCoalesce>(token);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_As_Ternary()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenTernary item;

        token = parser.Parse(x => x.Ternary(x.Alpha, x.Beta, x.Delta));
        Assert.Equal("(x.[Alpha] ? x.[Beta] : x.[Delta])", token.ToString());
        item = Assert.IsType<TokenTernary>(token);
    }

    // ----------------------------------------------------

#pragma warning disable CS1717
    //[Enforced]
    [Fact]
    public static void Parse_Setter_On_Argument()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenArgument arg;
        TokenValue value;
        token = parser.Parse(x => x = x);
        Assert.Equal("x", token.ToString());
        arg = Assert.IsType<TokenArgument>(token);

        token = parser.Parse(x => x = "007");
        Assert.Equal("'007'", token.ToString());
        value = Assert.IsType<TokenValue>(token);
        Assert.Equal("007", value.Value);
    }
#pragma warning restore

    //[Enforced]
    [Fact]
    public static void Parse_Setter_On_Member()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenSetter item;
        TokenValue value;

        token = parser.Parse(x => x.Alpha = "007");
        Assert.Equal("(x.[Alpha] = '007')", token.ToString());
        item = Assert.IsType<TokenSetter>(token);
        Assert.IsType<TokenIdentifier>(item.Target);
        value = Assert.IsType<TokenValue>(item.Value);
        Assert.Equal("007", value.Value);

        token = parser.Parse(x => x.Alpha = x);
        Assert.Equal("(x.[Alpha] = x)", token.ToString());
        item = Assert.IsType<TokenSetter>(token);
        Assert.IsType<TokenIdentifier>(item.Target);
        Assert.IsType<TokenArgument>(item.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Setter_Dynamic()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenSetter item;

        token = parser.Parse(x => x.Alpha = x.Beta);
        Assert.Equal("(x.[Alpha] = x.[Beta])", token.ToString());
        item = Assert.IsType<TokenSetter>(token);
        Assert.IsType<TokenIdentifier>(item.Target);
        Assert.IsType<TokenIdentifier>(item.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Ternary()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;

        try
        {
            token = parser.Parse(x => x.Alpha ? x.Beta : x.Delta);
            Assert.IsType<TokenTernary>(token);
            Assert.Fail();
        }
        catch (IsTypeException) { }

        token = parser.Parse(x => x.Ternary(x.Alpha, x.Beta, x.Delta));
        Assert.Equal("(x.[Alpha] ? x.[Beta] : x.[Delta])", token.ToString());
        Assert.IsType<TokenTernary>(token);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Unary_On_Argument()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenUnary item;

        token = parser.Parse(x => !x);
        Assert.Equal("(Not x)", token.ToString());
        item = Assert.IsType<TokenUnary>(token);
        Assert.IsType<TokenArgument>(item.Target);

        token = parser.Parse(x => !x.x);
        Assert.Equal("(Not x.)", token.ToString());
        item = Assert.IsType<TokenUnary>(token);
        Assert.IsType<TokenIdentifier>(item.Target);

        token = parser.Parse(x => !x.x.x);
        Assert.Equal("(Not x..)", token.ToString());
        item = Assert.IsType<TokenUnary>(token);
        Assert.IsType<TokenIdentifier>(item.Target);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Unary_OnMember()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenUnary item;

        token = parser.Parse(x => !x.Alpha);
        Assert.Equal("(Not x.[Alpha])", token.ToString());
        item = Assert.IsType<TokenUnary>(token);
        Assert.IsType<TokenIdentifier>(item.Target);

        token = parser.Parse(x => -x.Alpha);
        Assert.Equal("(Negate x.[Alpha])", token.ToString());
        item = Assert.IsType<TokenUnary>(token);
        Assert.IsType<TokenIdentifier>(item.Target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Value_Standard()
    {
        var engine = new FakeEngine();
        var parser = new ExpressionParser(engine);
        Token token;
        TokenValue item;

        token = parser.Parse(x => null!);
        Assert.Equal("NULL", token.ToString());
        item = Assert.IsType<TokenValue>(token);
        Assert.Null(item.Value);

        token = parser.Parse(x => true);
        Assert.Equal("TRUE", token.ToString());
        item = Assert.IsType<TokenValue>(token);
        Assert.True((bool)item.Value!);

        token = parser.Parse(x => "any");
        Assert.Equal("'any'", token.ToString());
        item = Assert.IsType<TokenValue>(token);
        Assert.Equal("any", (string)item.Value!);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Value_Command()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var parser = new ExpressionParser(engine);
        Token token;
        TokenCommand item;

        var command = new RawCommand(connection,
            "WHERE [LastName] = {0}",
            "Bond");

        token = parser.Parse(x => command);
        Assert.Equal("(WHERE [LastName] = #0)", token.ToString());
        item = Assert.IsType<TokenCommand>(token);
        Assert.NotSame(command, item.Command);

        token = parser.Parse(x => x(command));
        Assert.Equal("(WHERE [LastName] = #0)", token.ToString());
        item = Assert.IsType<TokenCommand>(token);
        Assert.NotSame(command, item.Command);
    }
}