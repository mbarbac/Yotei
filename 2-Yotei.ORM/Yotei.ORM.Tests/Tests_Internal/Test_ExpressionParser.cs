/*using Yotei.ORM.Internal;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_ExpressionParser
{
    //[Enforced]
    [Fact]
    public static void Parse_Argument()
    {
        Token token;
        TokenArgument item;

        token = ExpressionParser.Parse(x => x);
        Assert.Equal("x", token.ToString());
        item = Assert.IsType<TokenArgument>(token);
        Assert.Equal("x", item.Name);
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Binary()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Convert()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Indexed()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Invoke()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Member()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Method()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Setter()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Unary()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Value_Standard()
    {
        Token token;
        TokenValue item;

        token = ExpressionParser.Parse(x => null!);
        Assert.Equal("NULL", token.ToString());
        item = Assert.IsType<TokenValue>(token);
        Assert.Null(item.Value);

        token = ExpressionParser.Parse(x => true);
        Assert.Equal("TRUE", token.ToString());
        item = Assert.IsType<TokenValue>(token);
        Assert.True((bool)item.Value!);

        token = ExpressionParser.Parse(x => "any");
        Assert.Equal("'any'", token.ToString());
        item = Assert.IsType<TokenValue>(token);
        Assert.Equal("any", (string)item.Value!);
    }

    //[Enforced]
    //[Fact]
    //public static void Parse_Value_Command()
    //{
    //    var engine = new FakeEngine();
    //    var connection = new FakeConnection(engine);
    //    var command = new FakeCommand(connection,
    //        "SELECT * FROM [Employees] WHERE [LastName] = #0",
    //        new Code.ParameterList(engine, new Code.Parameter("#0", null)));

    //    Token token;
    //    TokenCommand item;

    //    token = ExpressionParser.Parse(x => command);
    //    Assert.Equal("(SELECT * FROM [Employees] WHERE [LastName] = #0)", token.ToString());
    //    item = Assert.IsType<TokenCommand>(token);
    //    Assert.Same(command, item.Command);
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Coalesce()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Parse_Ternary()
    //{
    //}
}*/