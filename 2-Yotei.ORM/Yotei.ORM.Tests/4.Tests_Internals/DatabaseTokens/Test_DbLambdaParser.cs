using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;
using Xunit.Sdk;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_DbLambdaParser
{
    //[Enforced]
    [Fact]
    public static void Parse_Argument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken token;
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
        DbToken token;
        DbTokenBinary item;
        DbTokenValue value;

        token = parser.Parse(x => x.Alpha && x.Beta);
        Assert.Equal("(x.[Alpha] And x.[Beta])", token.ToString());
        item = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, item.Operation);
        Assert.IsType<DbTokenIdentifier>(item.Left);
        Assert.IsType<DbTokenIdentifier>(item.Right);

        token = parser.Parse(x => x.x.Alpha && null!);
        Assert.Equal("(x..[Alpha] And NULL)", token.ToString());
        item = Assert.IsType<DbTokenBinary>(token);
        Assert.Equal(ExpressionType.And, item.Operation);
        Assert.IsType<DbTokenIdentifier>(item.Left);
        value = Assert.IsType<DbTokenValue>(item.Right);
        Assert.Null(value.Value);

        token = parser.Parse(x => x.Alpha.x && "any");
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
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenBinary item;

        token = parser.Parse(x => x & "any");
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
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenBinary item;

        token = parser.Parse(x => x.Alpha >= x.Beta);
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
        var parser = new DbLambdaParser(engine);
        DbToken token;

        token = parser.Parse(x => x.Coalesce(x.Alpha, x.Beta));
        Assert.Equal("(x.[Alpha] ?? x.[Beta])", token.ToString());
        Assert.IsType<DbTokenCoalesce>(token);

        try
        {
            // The dynamic lambda expression parser cannot parse coalesce expressions, returning
            // just the left element. Hence why we need to use the above's 'x.Coalesce(...)'
            // method...

            token = parser.Parse(x => x.Alpha ?? x.Beta);
            Assert.IsType<DbTokenCoalesce>(token);
            Assert.Fail();
        }
        catch (IsTypeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Convert_Argument_ToType()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenConvert.ToType item;

        token = parser.Parse(x => (string)x);
        Assert.Equal("((String) x)", token.ToString());
        item = Assert.IsType<DbTokenConvert.ToType>(token);
        Assert.Equal(typeof(string), item.Type);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Convert_Member_ToType()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenConvert.ToType item;

        token = parser.Parse(x => (string)x.Alpha);
        Assert.Equal("((String) x.[Alpha])", token.ToString());
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
        DbToken token;
        DbTokenIndexed item;
        DbTokenValue value;

        token = parser.Parse(x => x[27]);
        Assert.Equal("x['27']", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token);
        Assert.Equal(1, item.Indexes.Count);
        value = Assert.IsType<DbTokenValue>(item.Indexes[0]);
        Assert.Equal(27, value.Value);

        token = parser.Parse(x => x[x.Alpha]);
        Assert.Equal("x[x.[Alpha]]", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token);
        Assert.Equal(1, item.Indexes.Count);
        Assert.IsType<DbTokenIdentifier>(item.Indexes[0]);

        token = parser.Parse(x => x[x[x.Alpha]]);
        Assert.Equal("x[x[x.[Alpha]]]", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token);
        Assert.Equal(1, item.Indexes.Count);
        item = Assert.IsType<DbTokenIndexed>(item.Indexes[0]);
        Assert.Equal(1, item.Indexes.Count);
        Assert.IsType<DbTokenIdentifier>(item.Indexes[0]);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Indexed_Member()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenIndexed item;
        DbTokenIdentifier id;
        DbTokenValue value;

        token = parser.Parse(x => x.Alpha[x.Beta, null, "Other"]);
        Assert.Equal("x.[Alpha][x.[Beta], NULL, 'Other']", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token);
        Assert.Equal(3, item.Indexes.Count);
        Assert.IsType<DbTokenIdentifier>(item.Indexes[0]);
        value = Assert.IsType<DbTokenValue>(item.Indexes[1]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(item.Indexes[2]); Assert.Equal("Other", value.Value);

        token = parser.Parse(x => x.Alpha[x.Beta[x.Delta[null, "Other"]]]);
        Assert.Equal("x.[Alpha][x.[Beta][x.[Delta][NULL, 'Other']]]", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token);
        id = Assert.IsType<DbTokenIdentifier>(item.Host); Assert.Equal("[Alpha]", id.Identifier.Value);
        Assert.Single(item.Indexes);
        item = Assert.IsType<DbTokenIndexed>(item.Indexes[0]);
        id = Assert.IsType<DbTokenIdentifier>(item.Host); Assert.Equal("[Beta]", id.Identifier.Value);
        Assert.Single(item.Indexes);
        item = Assert.IsType<DbTokenIndexed>(item.Indexes[0]);
        id = Assert.IsType<DbTokenIdentifier>(item.Host); Assert.Equal("[Delta]", id.Identifier.Value);
        Assert.Equal(2, item.Indexes.Count);
        value = Assert.IsType<DbTokenValue>(item.Indexes[0]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(item.Indexes[1]); Assert.Equal("Other", value.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_On_Argument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenInvoke item;
        DbTokenValue value;

        token = parser.Parse(x => x());
        Assert.Equal("x()", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        Assert.Empty(item.Arguments);

        token = parser.Parse(x => x(50));
        Assert.Equal("x('50')", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(item.Arguments);
        value = Assert.IsType<DbTokenValue>(item.Arguments[0]); Assert.Equal(50, value.Value);

        token = parser.Parse(x => x(x.Alpha, x.Beta, null, "any"));
        Assert.Equal("x(x.[Alpha], x.[Beta], NULL, 'any')", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        Assert.Equal(4, item.Arguments.Count);
        Assert.IsType<DbTokenIdentifier>(item.Arguments[0]);
        Assert.IsType<DbTokenIdentifier>(item.Arguments[1]);
        value = Assert.IsType<DbTokenValue>(item.Arguments[2]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(item.Arguments[3]); Assert.Equal("any", value.Value);

        token = parser.Parse(x => x(null, x(x.Beta), 50));
        Assert.Equal("x(NULL, x(x.[Beta]), '50')", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
    }
    
    //[Enforced]
    [Fact]
    public static void Parse_Invoke_Standard()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenInvoke item;

        token = parser.Parse(x => x("any"));
        Assert.Equal("x(any)", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(item.Arguments);
        Assert.IsType<DbTokenLiteral>(item.Arguments[0]);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_Chained()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenInvoke item;
        DbTokenIdentifier id;

        token = parser.Parse(x => x(x.Alpha)(x.Beta));
        Assert.Equal("x(x.[Alpha])(x.[Beta])", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        id = Assert.IsType<DbTokenIdentifier>(item.Arguments[0]);
        Assert.Equal("[Beta]", id.Identifier.Value);
        item = Assert.IsType<DbTokenInvoke>(item.Host);
        Assert.IsType<DbTokenArgument>(item.Host);
        id = Assert.IsType<DbTokenIdentifier>(item.Arguments[0]);
        Assert.Equal("[Alpha]", id.Identifier.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Member_Standard()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenIdentifier item;
        DbTokenArgument arg;

        token = parser.Parse(x => x.Alpha);
        Assert.Equal("x.[Alpha]", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        Assert.Equal("Alpha", item.Identifier.UnwrappedValue);
        Assert.True(item.IsPureIdentifier);
        arg = Assert.IsType<DbTokenArgument>(item.Host);
        Assert.Equal("x", arg.Name);

        token = parser.Parse(x => x.Alpha.Beta);
        Assert.Equal("x.[Alpha].[Beta]", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        item = Assert.IsType<DbTokenIdentifier>(item.Host);
        arg = Assert.IsType<DbTokenArgument>(item.Host);

        token = parser.Parse(x => x.Alpha.Beta.Delta);
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
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenIdentifier item;
        DbTokenArgument arg;

        token = parser.Parse(x => x.x.Alpha);
        Assert.Equal("x..[Alpha]", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        Assert.True(item.IsPureIdentifier);
        item = Assert.IsType<DbTokenIdentifier>(item.Host);
        Assert.Null(item.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(item.Host);
        Assert.Equal("x", arg.Name);

        token = parser.Parse(x => x.Alpha.x);
        Assert.Equal("x.[Alpha].", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        Assert.Null(item.Identifier.Value);
        Assert.True(item.IsPureIdentifier);
        item = Assert.IsType<DbTokenIdentifier>(item.Host);
        arg = Assert.IsType<DbTokenArgument>(item.Host);

        token = parser.Parse(x => x.x);
        Assert.Equal("x.", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        Assert.Null(item.Identifier.Value);
        Assert.True(item.IsPureIdentifier);
        arg = Assert.IsType<DbTokenArgument>(item.Host);

        token = parser.Parse(x => x.x.x);
        Assert.Equal("x..", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        Assert.Null(item.Identifier.Value);
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
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenMethod item;
        DbTokenIdentifier name;
        DbTokenValue value;

        token = parser.Parse(x => x.Alpha());
        Assert.Equal("x.Alpha()", token.ToString());
        item = Assert.IsType<DbTokenMethod>(token);
        Assert.Empty(item.Arguments);

        token = parser.Parse(x => x.Alpha(x.Beta, null, 50));
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
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenMethod item;

        token = parser.Parse(x => x.Alpha.Beta());
        Assert.Equal("x.[Alpha].Beta()", token.ToString());
        item = Assert.IsType<DbTokenMethod>(token);
        Assert.Empty(item.Arguments);

        token = parser.Parse(x => x.Alpha.Beta(x.Delta, null, 50));
        Assert.Equal("x.[Alpha].Beta(x.[Delta], NULL, '50')", token.ToString());
        item = Assert.IsType<DbTokenMethod>(token);
        Assert.Equal(3, item.Arguments.Count);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_WithGenerics()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenMethod item;

        token = parser.Parse(x => x.Whatever<string>(x.Alpha));
        Assert.Equal("x.Whatever<String>(x.[Alpha])", token.ToString());
        item = Assert.IsType<DbTokenMethod>(token);
        Assert.Single(item.Arguments);
        Assert.IsType<DbTokenIdentifier>(item.Arguments[0]);
        Assert.Single(item.TypeArguments);
        Assert.IsAssignableFrom<Type>(item.TypeArguments[0]);

        token = parser.Parse(x => x.Whatever<string, int>(x.Alpha));
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
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenInvoke item;

        token = parser.Parse(x => x.Alpha.x());
        Assert.Equal("x.[Alpha]()", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        Assert.Empty(item.Arguments);

        token = parser.Parse(x => x(x.Alpha).x(x.Beta));
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
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenInvoke item;
        DbTokenLiteral text;

        token = parser.Parse(x => x("any"));
        Assert.Equal("x(any)", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(item.Arguments);
        text = Assert.IsType<DbTokenLiteral>(item.Arguments[0]);
        Assert.Equal("any", text.Value);

        token = parser.Parse(x => x.Alpha.x("any"));
        Assert.Equal("x.[Alpha](any)", token.ToString());
        item = Assert.IsType<DbTokenInvoke>(token);
        Assert.Single(item.Arguments);
        text = Assert.IsType<DbTokenLiteral>(item.Arguments[0]);
        Assert.Equal("any", text.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_EmbeddedLiteral_AsEscapedInvokeArgument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenMethod item;
        DbTokenInvoke invoke;
        DbTokenLiteral text;

        token = parser.Parse(x => x.Alpha.Beta(x.Delta, null, x("any")));
        Assert.Equal("x.[Alpha].Beta(x.[Delta], NULL, x(any))", token.ToString());
        item = Assert.IsType<DbTokenMethod>(token);
        Assert.Equal(3, item.Arguments.Count);
        Assert.IsType<DbTokenIdentifier>(item.Arguments[0]);
        Assert.IsType<DbTokenValue>(item.Arguments[1]);

        invoke = Assert.IsType<DbTokenInvoke>(item.Arguments[2]);
        Assert.Single(invoke.Arguments);
        text = Assert.IsType<DbTokenLiteral>(invoke.Arguments[0]); // As non-capturable literal...
        Assert.Equal("any", text.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Method_AsConvert_ToType()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenConvert.ToType toType;

        token = parser.Parse(x => x.Cast(typeof(string), x.Alpha));
        Assert.Equal("((String) x.[Alpha])", token.ToString());
        toType = Assert.IsType<DbTokenConvert.ToType>(token);
        Assert.True(toType.Type == typeof(string));

        token = parser.Parse(x => x.Cast<int>(x.Alpha));
        Assert.Equal("((Int32) x.[Alpha])", token.ToString());
        toType = Assert.IsType<DbTokenConvert.ToType>(token);
        Assert.Equal(typeof(int), toType.Type);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_AsConvert_ToSpecification()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenConvert.ToSpec toSpec;

        token = parser.Parse(x => x.Cast("varchar", x.Alpha));
        Assert.Equal("((varchar) x.[Alpha])", token.ToString());
        toSpec = Assert.IsType<DbTokenConvert.ToSpec>(token);
        Assert.Equal("varchar", toSpec.Type);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
#pragma warning disable CS1717
    public static void Parse_Setter_OnArgument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenArgument arg;
        DbTokenValue value;

        token = parser.Parse(x => x = x);
        Assert.Equal("x", token.ToString());
        arg = Assert.IsType<DbTokenArgument>(token);

        token = parser.Parse(x => x = "007");
        Assert.Equal("'007'", token.ToString());
        value = Assert.IsType<DbTokenValue>(token);
        Assert.Equal("007", value.Value);
    }
#pragma warning restore
    
    //[Enforced]
    [Fact]
    public static void Parse_Setter_OnMember()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenSetter item;
        DbTokenValue value;

        token = parser.Parse(x => x.Alpha = "007");
        Assert.Equal("(x.[Alpha] = '007')", token.ToString());
        item = Assert.IsType<DbTokenSetter>(token);
        Assert.IsType<DbTokenIdentifier>(item.Target);
        value = Assert.IsType<DbTokenValue>(item.Value);
        Assert.Equal("007", value.Value);

        token = parser.Parse(x => x.Alpha = x);
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
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenSetter item;

        token = parser.Parse(x => x.Alpha = x.Beta);
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
        var parser = new DbLambdaParser(engine);
        DbToken token;

        token = parser.Parse(x => x.Ternary(x.Alpha, x.Beta, x.Delta));
        Assert.Equal("(x.[Alpha] ? x.[Beta] : x.[Delta])", token.ToString());
        Assert.IsType<DbTokenTernary>(token);

        try
        {
            // The dynamic lambda expression parser cannot parse ternary expressions, returning
            // just the left element. Hence why we need to use the above's 'x.Coalesce(...)'
            // method...

            token = parser.Parse(x => x.Alpha ? x.Beta : x.Delta);
            Assert.IsType<DbTokenTernary>(token);
            Assert.Fail();
        }
        catch (IsTypeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Unary_OnArgument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenUnary item;

        token = parser.Parse(x => !x);
        Assert.Equal("(Not x)", token.ToString());
        item = Assert.IsType<DbTokenUnary>(token);
        Assert.IsType<DbTokenArgument>(item.Target);

        token = parser.Parse(x => !x.x);
        Assert.Equal("(Not x.)", token.ToString());
        item = Assert.IsType<DbTokenUnary>(token);
        Assert.IsType<DbTokenIdentifier>(item.Target);

        token = parser.Parse(x => !x.x.x);
        Assert.Equal("(Not x..)", token.ToString());
        item = Assert.IsType<DbTokenUnary>(token);
        Assert.IsType<DbTokenIdentifier>(item.Target);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Unary_OnMember()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenUnary item;

        token = parser.Parse(x => !x.Alpha);
        Assert.Equal("(Not x.[Alpha])", token.ToString());
        item = Assert.IsType<DbTokenUnary>(token);
        Assert.IsType<DbTokenIdentifier>(item.Target);

        token = parser.Parse(x => -x.Alpha);
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
        var parser = new DbLambdaParser(engine);
        DbToken token;
        DbTokenValue item;

        token = parser.Parse(x => null!);
        Assert.Equal("NULL", token.ToString());
        item = Assert.IsType<DbTokenValue>(token);
        Assert.Null(item.Value);

        token = parser.Parse(x => true);
        Assert.Equal("TRUE", token.ToString());
        item = Assert.IsType<DbTokenValue>(token);
        Assert.True((bool)item.Value!);

        token = parser.Parse(x => "any");
        Assert.Equal("'any'", token.ToString());
        item = Assert.IsType<DbTokenValue>(token);
        Assert.Equal("any", item.Value);
    }
}