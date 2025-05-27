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
    public static void Test_RemoveLast_NotFound()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        DbToken source;
        DbToken item; DbToken? removed;

        source = parser.Parse(x => x);
        item = source.RemoveLast(x => x is DbTokenLiteral, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = parser.Parse(x => x.Alpha);
        item = source.RemoveLast(x => x is DbTokenArgument arg && arg.Name == "x", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = parser.Parse(x => x.Alpha.Beta);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Gamma]", out removed);
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
        DbTokenArgument xarg;
        DbTokenIdentifier xid;

        source = parser.Parse(x => x.Alpha);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Alpha]", out removed);
        Assert.NotSame(source, item);
        xarg = Assert.IsType<DbTokenArgument>(item); Assert.Equal("x", xarg.Name);
        xid = Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("[Alpha]", xid.Identifier.Value);

        source = parser.Parse(x => x.Alpha.Beta);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Beta]", out removed);
        Assert.NotSame(source, item);
        xid = Assert.IsType<DbTokenIdentifier>(item); Assert.Equal("[Alpha]", xid.Identifier.Value);
        xid = Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("[Beta]", xid.Identifier.Value);

        source = parser.Parse(x => x.Alpha.Beta);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Alpha]", out removed);
        Assert.NotSame(source, item);
        xid = Assert.IsType<DbTokenIdentifier>(item); Assert.Equal("[Beta]", xid.Identifier.Value);
        xid = Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("[Alpha]", xid.Identifier.Value);

        source = parser.Parse(x => x.Alpha.Beta.Gamma);
        item = source.RemoveLast(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Beta]", out removed);
        Assert.NotSame(source, item);
        xid = Assert.IsType<DbTokenIdentifier>(item); Assert.Equal("[Gamma]", xid.Identifier.Value);
        xid = Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("[Beta]", xid.Identifier.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_RemoveFirst_NotFound()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    DbToken source;
    //    DbToken item; DbToken? removed;

    //    source = parser.Parse(x => x);
    //    item = source.RemoveFirst(x => x is DbTokenLiteral, out removed);
    //    Assert.Same(source, item);
    //    Assert.Null(removed);

    //    source = parser.Parse(x => x.Alpha);
    //    item = source.RemoveFirst(x => x is DbTokenArgument arg && arg.Name == "x", out removed);
    //    Assert.Same(source, item);
    //    Assert.Null(removed);

    //    source = parser.Parse(x => x.Alpha.Beta);
    //    item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Gamma]", out removed);
    //    Assert.Same(source, item);
    //    Assert.Null(removed);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_RemoveFirst_Found()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    DbToken source;
    //    DbToken item; DbToken? removed;
    //    DbTokenArgument xarg;
    //    DbTokenIdentifier xid;

    //    source = parser.Parse(x => x.Alpha);
    //    item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Alpha]", out removed);
    //    Assert.NotSame(source, item);
    //    xarg = Assert.IsType<DbTokenArgument>(item); Assert.Equal("x", xarg.Name);
    //    xid = Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("[Alpha]", xid.Identifier.Value);

    //    source = parser.Parse(x => x.Alpha.Beta);
    //    item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Beta]", out removed);
    //    Assert.NotSame(source, item);
    //    xid = Assert.IsType<DbTokenIdentifier>(item); Assert.Equal("[Alpha]", xid.Identifier.Value);
    //    xid = Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("[Beta]", xid.Identifier.Value);

    //    source = parser.Parse(x => x.Alpha.Beta);
    //    item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Alpha]", out removed);
    //    Assert.NotSame(source, item);
    //    xid = Assert.IsType<DbTokenIdentifier>(item); Assert.Equal("[Beta]", xid.Identifier.Value);
    //    xid = Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("[Alpha]", xid.Identifier.Value);

    //    source = parser.Parse(x => x.Alpha.Beta.Gamma);
    //    item = source.RemoveFirst(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Beta]", out removed);
    //    Assert.NotSame(source, item);
    //    xid = Assert.IsType<DbTokenIdentifier>(item); Assert.Equal("[Gamma]", xid.Identifier.Value);
    //    xid = Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("[Beta]", xid.Identifier.Value);
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_RemoveAll_NotFound()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    DbToken source;
    //    DbToken item;
    //    List<DbToken> removed;

    //    source = parser.Parse(x => x);
    //    item = source.RemoveAll(x => x is DbTokenLiteral, out removed);
    //    Assert.Same(source, item);
    //    Assert.Empty(removed);

    //    source = parser.Parse(x => x.Alpha);
    //    item = source.RemoveAll(x => x is DbTokenArgument arg && arg.Name == "x", out removed);
    //    Assert.Same(source, item);
    //    Assert.Empty(removed);

    //    source = parser.Parse(x => x.Alpha.Beta);
    //    item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Gamma]", out removed);
    //    Assert.Same(source, item);
    //    Assert.Empty(removed);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_RemoveAll_Found()
    //{
    //    var engine = new FakeEngine();
    //    var parser = new DbLambdaParser(engine);
    //    DbToken source;
    //    DbToken item;
    //    List<DbToken> removed;
    //    DbTokenArgument xarg;
    //    DbTokenIdentifier xid;

    //    source = parser.Parse(x => x.Alpha);
    //    item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Alpha]", out removed);
    //    Assert.NotSame(source, item);
    //    xarg = Assert.IsType<DbTokenArgument>(item); Assert.Equal("x", xarg.Name);
    //    Assert.Single(removed);
    //    xid = Assert.IsType<DbTokenIdentifier>(removed[0]); Assert.Equal("[Alpha]", xid.Identifier.Value);

    //    source = parser.Parse(x => x.Alpha.Alpha);
    //    item = source.RemoveAll(x => x is DbTokenIdentifier id && id.Identifier.Value == "[Alpha]", out removed);
    //    Assert.NotSame(source, item);
    //    xarg = Assert.IsType<DbTokenArgument>(item); Assert.Equal("x", xarg.Name);
    //    Assert.Equal(2, removed.Count);
    //    xid = Assert.IsType<DbTokenIdentifier>(removed[0]); Assert.Equal("[Alpha]", xid.Identifier.Value);
    //    xid = Assert.IsType<DbTokenIdentifier>(removed[1]); Assert.Equal("[Alpha]", xid.Identifier.Value);
    //}
}