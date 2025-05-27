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
    public static void Test_RemoveLast_Invalid()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbToken item; DbToken? removed;

        source = parser.Parse(x => x);
        item = source.RemoveLast(x => x is DbTokenArgument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = parser.Parse(x => x.One = x.Two);
        item = source.RemoveLast(x => x is DbTokenSetter, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Argument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbToken item; DbToken? removed;

        source = parser.Parse(x => x.One);
        item = source.RemoveLast(x => x is DbTokenArgument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_NotFound()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbToken item; DbToken? removed;

        source = parser.Parse(x => x.One);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = parser.Parse(x => x.One.Two);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Found()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbToken item; DbToken? removed;
        DbTokenArgument arg;
        DbTokenIdentifier id;

        source = parser.Parse(x => x.One);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Identifier.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        arg = Assert.IsType<DbTokenArgument>(item); // Item returned...
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Item removed...
        Assert.Equal("[One]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = parser.Parse(x => x.One.Two);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Item returned...
        Assert.Equal("[One]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Item removed...
        Assert.Equal("[Two]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = parser.Parse(x => x.One.Two);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Identifier.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Item returned...
        Assert.Equal("[Two]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Item removed...
        Assert.Equal("[One]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = parser.Parse(x => x.One.Two.Three);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Item returned...
        Assert.Equal("[Three]", id.Identifier.Value);
        id = Assert.IsType<DbTokenIdentifier>(id.Host);
        Assert.Equal("[One]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Item removed...
        Assert.Equal("[Two]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Found_Duplicate()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbToken item; DbToken? removed;
        DbTokenArgument arg;
        DbTokenIdentifier id;

        source = parser.Parse(x => x.One.Two.One);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Identifier.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Item returned...
        Assert.Equal("[Two]", id.Identifier.Value);
        id = Assert.IsType<DbTokenIdentifier>(id.Host);
        Assert.Equal("[One]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Item removed...
        Assert.Equal("[One]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveFirst_Invalid()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbToken item; DbToken? removed;

        source = parser.Parse(x => x);
        item = source.RemoveFirst(x => x is DbTokenArgument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = parser.Parse(x => x.One = x.Two);
        item = source.RemoveFirst(x => x is DbTokenSetter, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveFirst_Argument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbToken item; DbToken? removed;

        source = parser.Parse(x => x.One);
        item = source.RemoveFirst(x => x is DbTokenArgument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveFirst_NotFound()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbToken item; DbToken? removed;

        source = parser.Parse(x => x.One);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = parser.Parse(x => x.One.Two);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveFirst_Found()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbToken item; DbToken? removed;
        DbTokenArgument arg;
        DbTokenIdentifier id;

        source = parser.Parse(x => x.One);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Identifier.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        arg = Assert.IsType<DbTokenArgument>(item); // Item returned...
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Item removed...
        Assert.Equal("[One]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = parser.Parse(x => x.One.Two);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Item returned...
        Assert.Equal("[One]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Item removed...
        Assert.Equal("[Two]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = parser.Parse(x => x.One.Two);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Identifier.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Item returned...
        Assert.Equal("[Two]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Item removed...
        Assert.Equal("[One]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = parser.Parse(x => x.One.Two.Three);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Item returned...
        Assert.Equal("[Three]", id.Identifier.Value);
        id = Assert.IsType<DbTokenIdentifier>(id.Host);
        Assert.Equal("[One]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Item removed...
        Assert.Equal("[Two]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveFirst_Found_Duplicate()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbToken item; DbToken? removed;
        DbTokenArgument arg;
        DbTokenIdentifier id;

        source = parser.Parse(x => x.One.Two.One);
        item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Identifier.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Item returned...
        Assert.Equal("[One]", id.Identifier.Value);
        id = Assert.IsType<DbTokenIdentifier>(id.Host);
        Assert.Equal("[Two]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(removed); // Item removed...
        Assert.Equal("[One]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
    }


    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Invalid()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbToken item; List<DbToken> list;

        source = parser.Parse(x => x);
        item = source.RemoveAll(x => x is DbTokenArgument, out list);
        Assert.Same(source, item);
        Assert.Empty(list);

        source = parser.Parse(x => x.One = x.Two);
        item = source.RemoveAll(x => x is DbTokenSetter, out list);
        Assert.Same(source, item);
        Assert.Empty(list);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Argument()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbToken item; List<DbToken> list;

        source = parser.Parse(x => x.One);
        item = source.RemoveAll(x => x is DbTokenArgument, out list);
        Assert.Same(source, item);
        Assert.Empty(list);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_NotFound()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbToken item; List<DbToken> list;

        source = parser.Parse(x => x.One);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Three]", out list);
        Assert.Same(source, item);
        Assert.Empty(list);

        source = parser.Parse(x => x.One.Two);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Three]", out list);
        Assert.Same(source, item);
        Assert.Empty(list);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Found()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbToken item; List<DbToken> list;
        DbTokenArgument arg;
        DbTokenIdentifier id;

        source = parser.Parse(x => x.One);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Identifier.Value == "[One]", out list);
        Assert.NotSame(source, item);
        arg = Assert.IsType<DbTokenArgument>(item); // Item returned...
        Assert.Equal("x", arg.Name);
        Assert.Single(list); // Items removed...
        id = Assert.IsType<DbTokenIdentifier>(list[0]);
        Assert.Equal("[One]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = parser.Parse(x => x.One.One);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Identifier.Value == "[One]", out list);
        Assert.NotSame(source, item);
        arg = Assert.IsType<DbTokenArgument>(item); // Item returned...
        Assert.Equal("x", arg.Name);
        Assert.Equal(2, list.Count); // Items removed...
        id = Assert.IsType<DbTokenIdentifier>(list[0]);
        Assert.Equal("[One]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(list[0]);
        Assert.Equal("[One]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);

        source = parser.Parse(x => x.One.Two.One);
        item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Identifier.Value == "[One]", out list);
        Assert.NotSame(source, item);
        id = Assert.IsType<DbTokenIdentifier>(item); // Item returned...
        Assert.Equal("[Two]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        Assert.Equal(2, list.Count); // Items removed...
        id = Assert.IsType<DbTokenIdentifier>(list[0]);
        Assert.Equal("[One]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
        id = Assert.IsType<DbTokenIdentifier>(list[0]);
        Assert.Equal("[One]", id.Identifier.Value);
        arg = Assert.IsType<DbTokenArgument>(id.Host);
        Assert.Equal("x", arg.Name);
    }
}