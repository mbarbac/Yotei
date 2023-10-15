using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Identifier
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        IIdentifierSinglePart spart;

        var engine = new FakeEngine();
        var item = Identifier.Create(engine);
        spart = Assert.IsAssignableFrom<IIdentifierSinglePart>(item);
        Assert.Null(spart.Value);

        item = Identifier.Create(engine, "");
        spart = Assert.IsAssignableFrom<IIdentifierSinglePart>(item);
        Assert.Null(spart.Value);

        item = Identifier.Create(engine, " [ [ zero ] ] ");
        spart = Assert.IsAssignableFrom<IIdentifierSinglePart>(item);
        Assert.Equal("[zero]", spart.Value);

        item = Identifier.Create(engine, " [ [ one.two ] ] ");
        spart = Assert.IsAssignableFrom<IIdentifierSinglePart>(item);
        Assert.Equal("[one.two]", spart.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Multi()
    {
        IIdentifierMultiPart mpart;

        var engine = new FakeEngine();
        var item = Identifier.Create(engine, "one.two");
        mpart = Assert.IsAssignableFrom<IIdentifierMultiPart>(item);
        Assert.Equal("[one].[two]", mpart.Value);

        item = Identifier.Create(engine, "[one.two].three");
        mpart = Assert.IsAssignableFrom<IIdentifierMultiPart>(item);
        Assert.Equal("[one.two].[three]", mpart.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Match_No_CaseSensitive()
    {
        var engine = new FakeEngine() { CaseSensitiveNames = false };

        var source = Identifier.Create(engine);
        var target = Identifier.Create(engine);
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "one");
        target = Identifier.Create(engine, "ONE");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "one");
        target = Identifier.Create(engine);
        Assert.True(source.Match(target));

        source = Identifier.Create(engine, ".one");
        target = Identifier.Create(engine);
        Assert.True(source.Match(target));

        source = Identifier.Create(engine, "one");
        target = Identifier.Create(engine, ".ONE");
        Assert.True(source.Match(target));

        source = Identifier.Create(engine, "two.one");
        target = Identifier.Create(engine, ".ONE");
        Assert.True(source.Match(target));

        source = Identifier.Create(engine, "three.two.one");
        target = Identifier.Create(engine, "..ONE");
        Assert.True(source.Match(target));

        source = Identifier.Create(engine, "three.two.one");
        target = Identifier.Create(engine, "THREE..ONE");
        Assert.True(source.Match(target));

        source = Identifier.Create(engine, "x..one");
        target = Identifier.Create(engine, "x..");
        Assert.True(source.Match(target));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_No_Match_No_CaseSensitive()
    {
        var engine = new FakeEngine() { CaseSensitiveNames = false };

        var source = Identifier.Create(engine);
        var target = Identifier.Create(engine, "one");
        Assert.False(source.Match(target));

        source = Identifier.Create(engine, "one");
        target = Identifier.Create(engine, "x");
        Assert.False(source.Match(target));

        source = Identifier.Create(engine, "one");
        target = Identifier.Create(engine, "x.one");
        Assert.False(source.Match(target));

        source = Identifier.Create(engine, "two.one");
        target = Identifier.Create(engine, "x.one");
        Assert.False(source.Match(target));

        source = Identifier.Create(engine, "x..one");
        target = Identifier.Create(engine, "x.y.one");
        Assert.False(source.Match(target));

        source = Identifier.Create(engine, "x..one");
        target = Identifier.Create(engine, "y.one");
        Assert.False(source.Match(target));

        source = Identifier.Create(engine, "x..one");
        target = Identifier.Create(engine, "z..");
        Assert.False(source.Match(target));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Match_CaseSensitive()
    {
        var engine = new FakeEngine() { CaseSensitiveNames = true };

        var source = Identifier.Create(engine);
        var target = Identifier.Create(engine);
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        // Values mismatch...
        source = Identifier.Create(engine, "one");
        target = Identifier.Create(engine, "ONE");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));
    }
}