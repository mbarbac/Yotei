using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_DbTokenParts
{
    //[Enforced]
    [Fact]
    public static void Test_NoHead_NoTail()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbTokenParts parts;

        source = parser.Parse(x => x);
        parts = DbTokenParts.Create(source);
        Assert.Null(parts.Head);
        Assert.IsType<DbTokenArgument>(parts.Body); Assert.Equal("x", parts.Body.ToString());
        Assert.Null(parts.Tail);

        source = parser.Parse(x => x.b);
        parts = DbTokenParts.Create(source);
        Assert.Null(parts.Head);
        Assert.IsType<DbTokenIdentifier>(parts.Body); Assert.Equal("x.[b]", parts.Body.ToString());
        Assert.Null(parts.Tail);

        source = parser.Parse(x => x.b1.b2);
        parts = DbTokenParts.Create(source);
        Assert.Null(parts.Head);
        Assert.IsType<DbTokenIdentifier>(parts.Body); Assert.Equal("x.[b1].[b2]", parts.Body.ToString());
        Assert.Null(parts.Tail);

        source = parser.Parse(x => x.b1.x("any").b2);
        parts = DbTokenParts.Create(source);
        Assert.Null(parts.Head);
        Assert.IsType<DbTokenIdentifier>(parts.Body); Assert.Equal("x.[b1](any).[b2]", parts.Body.ToString());
        Assert.Null(parts.Tail);
    }

    //[Enforced]
    [Fact]
    public static void Test_All_Invokes()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbTokenParts parts;

        source = parser.Parse(x => x()()());
        parts = DbTokenParts.Create(source);
        Assert.Null(parts.Head);
        Assert.IsType<DbTokenArgument>(parts.Body); Assert.Equal("x", parts.Body.ToString());
        Assert.IsType<DbTokenInvoke>(parts.Tail); Assert.Equal("x()", parts.Tail.ToString());

        source = parser.Parse(x => x("a")()("b"));
        parts = DbTokenParts.Create(source);
        Assert.Null(parts.Head);
        Assert.IsType<DbTokenArgument>(parts.Body); Assert.Equal("x", parts.Body.ToString());
        Assert.IsType<DbTokenInvoke>(parts.Tail); Assert.Equal("x(a, b)", parts.Tail.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Head()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbTokenParts parts;

        source = parser.Parse(x => x.x("t")); // No head part, using tail by default...
        parts = DbTokenParts.Create(source);
        Assert.Null(parts.Head);
        Assert.IsType<DbTokenArgument>(parts.Body); Assert.Equal("x", parts.Body.ToString());
        Assert.IsType<DbTokenInvoke>(parts.Tail); Assert.Equal("x(t)", parts.Tail.ToString());

        source = parser.Parse(x => x.x("h").b);
        parts = DbTokenParts.Create(source);
        Assert.IsType<DbTokenInvoke>(parts.Head); Assert.Equal("x(h)", parts.Head.ToString());
        Assert.IsType<DbTokenIdentifier>(parts.Body); Assert.Equal("x.[b]", parts.Body.ToString());
        Assert.Null(parts.Tail);

        //source = parser.Parse(x => x.x("t1")("t2"));
        //parts = DbTokenParts.Create(source);
        //Assert.Null(parts.Head);
        //Assert.IsType<DbTokenArgument>(parts.Body); Assert.Equal("x", parts.Body.ToString());
        //Assert.IsType<DbTokenInvoke>(parts.Tail); Assert.Equal("x(t1, t2)", parts.Tail.ToString());

        //source = parser.Parse(x => x.x("t1").x("t2"));
        //parts = DbTokenParts.Create(source);
        //Assert.Null(parts.Head);
        //Assert.IsType<DbTokenArgument>(parts.Body); Assert.Equal("x", parts.Body.ToString());
        //Assert.IsType<DbTokenInvoke>(parts.Tail); Assert.Equal("x(t1, t2)", parts.Tail.ToString());

        //source = parser.Parse(x => x.b.x("t"));
        //parts = DbTokenParts.Create(source);
        //Assert.Null(parts.Head);
        //Assert.IsType<DbTokenIdentifier>(parts.Body); Assert.Equal("x.[b]", parts.Body.ToString());
        //Assert.IsType<DbTokenInvoke>(parts.Tail); Assert.Equal("x(t)", parts.Tail.ToString());

        //source = parser.Parse(x => x.b1.b2.x("t1").x("t2"));
        //parts = DbTokenParts.Create(source);
        //Assert.Null(parts.Head);
        //Assert.IsType<DbTokenIdentifier>(parts.Body); Assert.Equal("x.[b1].[b2]", parts.Body.ToString());
        //Assert.IsType<DbTokenInvoke>(parts.Tail); Assert.Equal("x(t1, t2)", parts.Tail.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Tail()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbTokenParts parts;

        source = parser.Parse(x => x.x("t"));
        parts = DbTokenParts.Create(source);
        Assert.Null(parts.Head);
        Assert.IsType<DbTokenArgument>(parts.Body); Assert.Equal("x", parts.Body.ToString());
        Assert.IsType<DbTokenInvoke>(parts.Tail); Assert.Equal("x(t)", parts.Tail.ToString());

        source = parser.Parse(x => x.x("t1")("t2"));
        parts = DbTokenParts.Create(source);
        Assert.Null(parts.Head);
        Assert.IsType<DbTokenArgument>(parts.Body); Assert.Equal("x", parts.Body.ToString());
        Assert.IsType<DbTokenInvoke>(parts.Tail); Assert.Equal("x(t1, t2)", parts.Tail.ToString());

        source = parser.Parse(x => x.x("t1").x("t2"));
        parts = DbTokenParts.Create(source);
        Assert.Null(parts.Head);
        Assert.IsType<DbTokenArgument>(parts.Body); Assert.Equal("x", parts.Body.ToString());
        Assert.IsType<DbTokenInvoke>(parts.Tail); Assert.Equal("x(t1, t2)", parts.Tail.ToString());

        source = parser.Parse(x => x.b.x("t"));
        parts = DbTokenParts.Create(source);
        Assert.Null(parts.Head);
        Assert.IsType<DbTokenIdentifier>(parts.Body); Assert.Equal("x.[b]", parts.Body.ToString());
        Assert.IsType<DbTokenInvoke>(parts.Tail); Assert.Equal("x(t)", parts.Tail.ToString());

        source = parser.Parse(x => x.b1.b2.x("t1").x("t2"));
        parts = DbTokenParts.Create(source);
        Assert.Null(parts.Head);
        Assert.IsType<DbTokenIdentifier>(parts.Body); Assert.Equal("x.[b1].[b2]", parts.Body.ToString());
        Assert.IsType<DbTokenInvoke>(parts.Tail); Assert.Equal("x(t1, t2)", parts.Tail.ToString());
    }

    //[Enforced]
    //[Fact]
    //public static void Test_With_Head_And_Tail()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    DbToken source;
    //    DbTokenParts parts;
    //}
}