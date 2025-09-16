namespace Yotei.ORM.Tests.Internals.DbTokens;

// ========================================================
//[Enforced]
public static class Test_DbLambdaParser
{
    //[Enforced]
    [Fact]
    public static void Parse_Argument()
    {
        var parser = new DbLambdaParser(new FakeEngine());
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
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
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
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
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
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
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
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;

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
        catch (Xunit.Sdk.IsTypeException) { }

        // Special case when the left operand is a null-alike one...
        token = parser.Parse(x => x.Coalesce(null, x.Beta));
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

        var parser = new DbLambdaParser(new FakeEngine());
        var connection = new FakeConnection(parser.Engine);
        var cmd = new FakeCommand(connection, "SELECT * FROM Employees WHERE Id = {0}", "007");

        token = parser.Parse(x => cmd);
        command = Assert.IsType<DbTokenCommand>(token);
        Assert.Equal("(SELECT * FROM Employees WHERE Id = #0)", token.ToString());
        Assert.Equal(token.ToString(), cmd.GetCommandInfo().Text.Wrap('(', ')'));

        token = parser.Parse(x => x(cmd));
        command = Assert.IsType<DbTokenCommand>(token);
        Assert.Equal("(SELECT * FROM Employees WHERE Id = #0)", token.ToString());
        Assert.Equal(token.ToString(), cmd.GetCommandInfo().Text.Wrap('(', ')'));

        token = parser.Parse(x => x(cmd).As("Any"));
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
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
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
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
        DbTokenConvert.ToType item;

        token = parser.Parse(x => (string)x.Alpha);
        Assert.Equal("((String) x.[Alpha])", token.ToString());
        item = Assert.IsType<DbTokenConvert.ToType>(token);
        Assert.Equal(typeof(string), item.Type);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Convert_Member_ToSpec()
    {
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
        DbTokenConvert.ToSpec spec;

        token = parser.Parse(x => x.Convert("string", x.Alpha));
        Assert.Equal("((string) x.[Alpha])", token.ToString());
        spec = Assert.IsType<DbTokenConvert.ToSpec>(token);
        Assert.Equal("string", spec.Type);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Indexed_Argument()
    {
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
        DbTokenIndexed item;
        DbTokenValue value;

        token = parser.Parse(x => x[27]);
        Assert.Equal("x['27']", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token);
        Assert.Single(item.Indexes);
        value = Assert.IsType<DbTokenValue>(item.Indexes[0]);
        Assert.Equal(27, value.Value);

        token = parser.Parse(x => x[x.Alpha]);
        Assert.Equal("x[x.[Alpha]]", token.ToString());
        item = Assert.IsType<DbTokenIndexed>(token);
        Assert.Single(item.Indexes);
        Assert.IsType<DbTokenIdentifier>(item.Indexes[0]);

        token = parser.Parse(x => x[x[x.Alpha]]);
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
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
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
    //[Fact]
    //public static void Parse_Invoke_Single_Argument()


    //[Enforced]
    [Fact]
    public static void Parse_Invoke_Many_Arguments()
    {
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
        DbTokenInvoke invoke;
        DbTokenValue value;

        token = parser.Parse(x => x("any", "other"));
        Assert.Equal("x('any', 'other')", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Equal(2, invoke.Arguments.Count);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[0]); Assert.Equal("any", value.Value);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[1]); Assert.Equal("other", value.Value);

        token = parser.Parse(x => x(x.Alpha, x.Beta, null, "any"));
        Assert.Equal("x(x.[Alpha], x.[Beta], NULL, 'any')", token.ToString());
        invoke = Assert.IsType<DbTokenInvoke>(token);
        Assert.Equal(4, invoke.Arguments.Count);
        Assert.IsType<DbTokenIdentifier>(invoke.Arguments[0]);
        Assert.IsType<DbTokenIdentifier>(invoke.Arguments[1]);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[2]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(invoke.Arguments[3]); Assert.Equal("any", value.Value);
    }

    //[Enforced]
    //[Fact]
    //public static void Parse_Invoke_Chained()

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Invoke_Escape()

    //[Enforced]
    //[Fact]
    //public static void Parse_Invoke_Escape_Chained()

    //[Enforced]
    //[Fact]
    //public static void Parse_Invoke_Escape_Arguments_Concatenated()

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Member_Standard()
    {
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
        DbTokenIdentifier item;
        DbTokenArgument arg;

        token = parser.Parse(x => x.Alpha);
        Assert.Equal("x.[Alpha]", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        Assert.Equal("Alpha", item.Identifier.RawValue);
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
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
        DbTokenIdentifier item;
        DbTokenArgument arg;

        token = parser.Parse(x => x.x.Alpha);
        Assert.Equal("x..[Alpha]", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        Assert.True(item.IsPureIdentifier);
        item = Assert.IsType<DbTokenIdentifier>(item.Host);
        Assert.Null(item.Value);
        arg = Assert.IsType<DbTokenArgument>(item.Host);
        Assert.Equal("x", arg.Name);

        token = parser.Parse(x => x.Alpha.x);
        Assert.Equal("x.[Alpha].", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        Assert.Null(item.Value);
        Assert.True(item.IsPureIdentifier);
        item = Assert.IsType<DbTokenIdentifier>(item.Host);
        arg = Assert.IsType<DbTokenArgument>(item.Host);

        token = parser.Parse(x => x.x);
        Assert.Equal("x.", token.ToString());
        item = Assert.IsType<DbTokenIdentifier>(token);
        Assert.Null(item.Value);
        Assert.True(item.IsPureIdentifier);
        arg = Assert.IsType<DbTokenArgument>(item.Host);

        token = parser.Parse(x => x.x.x);
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
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
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
        name = Assert.IsType<DbTokenIdentifier>(item.Arguments[0]); Assert.Equal("Beta", name.Identifier.RawValue);
        value = Assert.IsType<DbTokenValue>(item.Arguments[1]); Assert.Null(value.Value);
        value = Assert.IsType<DbTokenValue>(item.Arguments[2]); Assert.Equal(50, value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Method_OnMember()
    {
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
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
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
        DbTokenMethod item;

        token = parser.Parse(x => x.Whatever<string>(x.Alpha));
        Assert.Equal("x.Whatever<String>(x.[Alpha])", token.ToString());
        item = Assert.IsType<DbTokenMethod>(token);
        Assert.Single(item.Arguments);
        Assert.IsType<DbTokenIdentifier>(item.Arguments[0]);
        Assert.Single(item.TypeArguments);
        Assert.IsType<Type>(item.TypeArguments[0], exactMatch: false);

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
    //[Fact]
    //public static void Parse_Method_AsInvoke_Standard()

    //[Enforced]
    //[Fact]
    //public static void Parse_Method_AsInvoke_ToLiteral()

    //[Enforced]
    //[Fact]
    //public static void Parse_Method_ArgumentInvokeString_As_LiteralArgument()

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Method_AsConvert_ToType()
    {
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
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
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
        DbTokenConvert.ToSpec toSpec;

        token = parser.Parse(x => x.Cast("varchar", x.Alpha));
        Assert.Equal("((varchar) x.[Alpha])", token.ToString());
        toSpec = Assert.IsType<DbTokenConvert.ToSpec>(token);
        Assert.Equal("varchar", toSpec.Type);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Setter_OnArgument()
    {
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
        DbTokenArgument arg;
        DbTokenValue value;

#pragma warning disable CS1717
        token = parser.Parse(x => x = x);
        Assert.Equal("x", token.ToString());
        arg = Assert.IsType<DbTokenArgument>(token);
#pragma warning restore

        token = parser.Parse(x => x = "007");
        Assert.Equal("'007'", token.ToString());
        value = Assert.IsType<DbTokenValue>(token);
        Assert.Equal("007", value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Setter_OnMember()
    {
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
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
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
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
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;

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
        catch (Xunit.Sdk.IsTypeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Unary_OnArgument()
    {
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
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
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
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
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
        DbTokenValue item;

        token = parser.Parse(x => null);
        Assert.Equal("NULL", token.ToString());
        item = Assert.IsType<DbTokenValue>(token);
        Assert.Null(item.Value);

        token = parser.Parse(x => true);
        Assert.Equal("TRUE", token.ToString());
        item = Assert.IsType<DbTokenValue>(token);
        Assert.True((bool)item.Value!);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Value_First_Level_Strings()
    {
        var parser = new DbLambdaParser(new FakeEngine());
        IDbToken token;
        DbTokenValue item;

        token = parser.Parse(x => "any");
        Assert.Equal("'any'", token.ToString());
        item = Assert.IsType<DbTokenValue>(token);
        Assert.Equal("any", item.Value);
    }
}