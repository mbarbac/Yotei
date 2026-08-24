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
    //[Fact]
    //public static void Parse_Command()
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
    //public static void Parse_Convert()
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
    //public static void Parse_Indexed()
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
    //public static void Parse_Invoke()
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