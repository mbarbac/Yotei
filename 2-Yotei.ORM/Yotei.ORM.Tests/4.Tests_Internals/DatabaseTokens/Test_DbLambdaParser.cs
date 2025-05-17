using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_DbLambdaParser
{
    //[Enforced]
    [Fact]
    public static void Test_ParseArgument()
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
    //[Fact]
    //public static void Test_ParseBinary()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    DbToken token;

    //    token = parser.Parse();
    //    Assert.Equal("", token.ToString());
    //    item = Assert.IsType<>(token);
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_ParseCoalesce()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    DbToken token;

    //    token = parser.Parse();
    //    Assert.Equal("", token.ToString());
    //    item = Assert.IsType<>(token);
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_ParseConvert()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    DbToken token;

    //    token = parser.Parse();
    //    Assert.Equal("", token.ToString());
    //    item = Assert.IsType<>(token);
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_ParseIndexed()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    DbToken token;

    //    token = parser.Parse();
    //    Assert.Equal("", token.ToString());
    //    item = Assert.IsType<>(token);
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_ParseInvoke()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    DbToken token;

    //    token = parser.Parse();
    //    Assert.Equal("", token.ToString());
    //    item = Assert.IsType<>(token);
    //}

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ParseMember_Standard()
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
    public static void Test_ParseMember_WithDynamicArgument()
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
    //[Fact]
    //public static void Test_ParseMethod()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    DbToken token;

    //    token = parser.Parse();
    //    Assert.Equal("", token.ToString());
    //    item = Assert.IsType<>(token);
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_ParseSetter()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    DbToken token;

    //    token = parser.Parse();
    //    Assert.Equal("", token.ToString());
    //    item = Assert.IsType<>(token);
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_ParseTernary()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    DbToken token;

    //    token = parser.Parse();
    //    Assert.Equal("", token.ToString());
    //    item = Assert.IsType<>(token);
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_ParseUnary()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    DbToken token;

    //    token = parser.Parse();
    //    Assert.Equal("", token.ToString());
    //    item = Assert.IsType<>(token);
    //}

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ParseValue_Standard()
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