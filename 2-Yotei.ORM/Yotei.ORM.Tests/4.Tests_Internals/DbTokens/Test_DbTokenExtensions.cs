#pragma warning disable IDE0018

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_DbTokenExtensions
{
    //[Enforced]
    [Fact]
    public static void Test_RemoveFirst_Invalid()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x);
        item = source.RemoveFirst(x => x is DbTokenArgument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => x.Coalesce(x.Alpha, x.Beta));
        item = source.RemoveFirst(x => x is DbTokenArgument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveFirst_NotFound()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.One);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveFirst_NotFound_NotHosted()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.One == x.Two);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => (string)x.One);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => x.One = 7);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => !x.One);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveFirst_Found()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        IDbToken? removed = null;
        DbTokenArgument arg;
        DbTokenIdentifier id;
        
        source = DbLambdaParser.Parse(engine, x => x.One);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        arg = Assert.IsType<DbTokenArgument>(item); // Token returned...
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Token removed...
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Token returned...
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Token removed...
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Token returned...
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Token removed...
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Item returned...
        Assert.Equal("[Three]", id.Value);
        id = Assert.IsType<DbTokenIdentifier>(id.Host);
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Item removed...
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveFirst_Found_WithDuplicates()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        IDbToken? removed = null;
        DbTokenArgument arg;
        DbTokenIdentifier id;

        source = DbLambdaParser.Parse(engine, x => x.One.Two.One);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Item returned...
        Assert.Equal("[One]", id.Value);
        id = Assert.IsType<DbTokenIdentifier>(id.Host);
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Item removed...
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveFirst_Found_NotHosted()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        IDbToken? removed = null;
        DbTokenArgument arg;
        DbTokenIdentifier id;

        source = DbLambdaParser.Parse(engine, x => x.One.Two == "any");
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        var xbinary = Assert.IsType<DbTokenBinary>(item); // Token returned...
        var xvalue = Assert.IsType<DbTokenValue>(xbinary.Right);
        Assert.Equal("any", xvalue.Value);
        id = Assert.IsType<DbTokenIdentifier>(xbinary.Left);
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Token removed...
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => (string)x.One.Two);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        var xconvert = Assert.IsType<DbTokenConvert.ToType>(item); // Token returned...
        Assert.Equal(typeof(string), xconvert.Type);
        id = Assert.IsType<DbTokenIdentifier>(xconvert.Target);
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Token removed...
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => x.One.Two = 7);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        var xsetter = Assert.IsType<DbTokenSetter>(item); // Token returned...
        xvalue = Assert.IsType<DbTokenValue>(xsetter.Value);
        Assert.Equal(7, xvalue.Value);
        id = Assert.IsType<DbTokenIdentifier>(xsetter.Target);
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Token removed...
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => !x.One.Two);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        var xunary = Assert.IsType<DbTokenUnary>(item); // Token returned...
        Assert.Equal(ExpressionType.Not, xunary.Operation);
        id = Assert.IsType<DbTokenIdentifier>(xunary.Target);
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Token removed...
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Invalid()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x);
        item = source.RemoveLast(x => x is DbTokenArgument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => x.Coalesce(x.Alpha, x.Beta));
        item = source.RemoveLast(x => x is DbTokenArgument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_NotFound()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.One);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_NotFound_NotHosted()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.One == x.Two);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => (string)x.One);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => x.One = 7);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => !x.One);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Found()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        IDbToken? removed = null;
        DbTokenArgument arg;
        DbTokenIdentifier id;

        source = DbLambdaParser.Parse(engine, x => x.One);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        arg = Assert.IsType<DbTokenArgument>(item); // Token returned...
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Token removed...
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Token returned...
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Token removed...
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Token returned...
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Token removed...
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Item returned...
        Assert.Equal("[Three]", id.Value);
        id = Assert.IsType<DbTokenIdentifier>(id.Host);
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Item removed...
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Found_WithDuplicates()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        IDbToken? removed = null;
        DbTokenArgument arg;
        DbTokenIdentifier id;

        source = DbLambdaParser.Parse(engine, x => x.One.Two.One);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Item returned...
        Assert.Equal("[Two]", id.Value);
        id = Assert.IsType<DbTokenIdentifier>(id.Host);
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Item removed...
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Found_NotHosted()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        IDbToken? removed = null;
        DbTokenArgument arg;
        DbTokenIdentifier id;

        source = DbLambdaParser.Parse(engine, x => x.One.Two == "any");
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        var xbinary = Assert.IsType<DbTokenBinary>(item); // Token returned...
        var xvalue = Assert.IsType<DbTokenValue>(xbinary.Right);
        Assert.Equal("any", xvalue.Value);
        id = Assert.IsType<DbTokenIdentifier>(xbinary.Left);
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Token removed...
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => (string)x.One.Two);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        var xconvert = Assert.IsType<DbTokenConvert.ToType>(item); // Token returned...
        Assert.Equal(typeof(string), xconvert.Type);
        id = Assert.IsType<DbTokenIdentifier>(xconvert.Target);
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Token removed...
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => x.One.Two = 7);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        var xsetter = Assert.IsType<DbTokenSetter>(item); // Token returned...
        xvalue = Assert.IsType<DbTokenValue>(xsetter.Value);
        Assert.Equal(7, xvalue.Value);
        id = Assert.IsType<DbTokenIdentifier>(xsetter.Target);
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Token removed...
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => !x.One.Two);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        var xunary = Assert.IsType<DbTokenUnary>(item); // Token returned...
        Assert.Equal(ExpressionType.Not, xunary.Operation);
        id = Assert.IsType<DbTokenIdentifier>(xunary.Target);
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Token removed...
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Invalid()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        List<IDbToken> removed;

        source = DbLambdaParser.Parse(engine, x => x);
        item = source.RemoveAll(x => x is DbTokenArgument, out removed);
        Assert.Same(source, item);
        Assert.Empty(removed);

        source = DbLambdaParser.Parse(engine, x => x.Coalesce(x.Alpha, x.Beta));
        item = source.RemoveAll(x => x is DbTokenArgument, out removed);
        Assert.Same(source, item);
        Assert.Empty(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_NotFound()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        List<IDbToken> removed;

        source = DbLambdaParser.Parse(engine, x => x.One);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Empty(removed);

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Empty(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_NotFound_NotHosted()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        List<IDbToken> removed;

        source = DbLambdaParser.Parse(engine, x => x.One == x.Two);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Empty(removed);

        source = DbLambdaParser.Parse(engine, x => (string)x.One);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Empty(removed);

        source = DbLambdaParser.Parse(engine, x => x.One = 7);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Empty(removed);

        source = DbLambdaParser.Parse(engine, x => !x.One);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Empty(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Found()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        List<IDbToken> removed;
        DbTokenArgument arg;
        DbTokenIdentifier id;

        source = DbLambdaParser.Parse(engine, x => x.One);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        arg = Assert.IsType<DbTokenArgument>(item); // Token returned...
        Assert.Equal("x", arg.Name);
        Assert.Single(removed); // Tokens removed...
        id = Assert.IsType<DbTokenIdentifier>(removed[0]);
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => x.One.One);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        arg = Assert.IsType<DbTokenArgument>(item); // Token returned...
        Assert.Equal("x", arg.Name);
        Assert.Equal(2, removed.Count); // Tokens removed...
        id = Assert.IsType<DbTokenIdentifier>(removed[0]);
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed[1]);
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => x.One.Two.One);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        id = Assert.IsType<DbTokenIdentifier>(item); // Token returned...
        Assert.Equal("[Two]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        Assert.Equal(2, removed.Count); // Tokens removed...
        id = Assert.IsType<DbTokenIdentifier>(removed[0]);
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed[1]);
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Found_NotHosted()
    {
        var engine = new FakeEngine();
        IDbToken source;
        IDbToken item;
        List<IDbToken> removed;
        DbTokenArgument arg;
        DbTokenIdentifier id;

        source = DbLambdaParser.Parse(engine, x => x.One.Two == "any");
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        var xbinary = Assert.IsType<DbTokenBinary>(item); // Token returned...
        var xvalue = Assert.IsType<DbTokenValue>(xbinary.Right);
        Assert.Equal("any", xvalue.Value);
        id = Assert.IsType<DbTokenIdentifier>(xbinary.Left);
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        Assert.Single(removed); // Token removed...
        id = Assert.IsType<DbTokenIdentifier>(removed[0]);
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => (string)x.One.Two);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        var xconvert = Assert.IsType<DbTokenConvert.ToType>(item); // Token returned...
        Assert.Equal(typeof(string), xconvert.Type);
        id = Assert.IsType<DbTokenIdentifier>(xconvert.Target);
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        Assert.Single(removed); // Token removed...
        id = Assert.IsType<DbTokenIdentifier>(removed[0]);
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => x.One.Two = 7);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        var xsetter = Assert.IsType<DbTokenSetter>(item); // Token returned...
        xvalue = Assert.IsType<DbTokenValue>(xsetter.Value);
        Assert.Equal(7, xvalue.Value);
        id = Assert.IsType<DbTokenIdentifier>(xsetter.Target);
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        Assert.Single(removed); // Token removed...
        id = Assert.IsType<DbTokenIdentifier>(removed[0]);
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = DbLambdaParser.Parse(engine, x => !x.One.Two);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        var xunary = Assert.IsType<DbTokenUnary>(item); // Token returned...
        Assert.Equal(ExpressionType.Not, xunary.Operation);
        id = Assert.IsType<DbTokenIdentifier>(xunary.Target);
        Assert.Equal("[Two]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        Assert.Single(removed); // Token removed...
        id = Assert.IsType<DbTokenIdentifier>(removed[0]);
        Assert.Equal("[One]", id.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
    }
}