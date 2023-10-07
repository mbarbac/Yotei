namespace Yotei.ORM.Core.Tests;

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
    public static void Test_Match_Single_No_CaseSensitive()
    {
        var sensitive = false;
        var engine = new FakeEngine() { CaseSensitiveNames = sensitive };

        var source = Identifier.Create(engine);
        var target = Identifier.Create(engine);
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "one");
        target = Identifier.Create(engine);
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "one");
        target = Identifier.Create(engine, "ONE");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));
    }

    //[Enforced]
    [Fact]
    public static void Test_Match_Single_CaseSensitive()
    {
        var sensitive = true;
        var engine = new FakeEngine() { CaseSensitiveNames = sensitive };

        var source = Identifier.Create(engine);
        var target = Identifier.Create(engine);
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "one");
        target = Identifier.Create(engine);
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "one");
        target = Identifier.Create(engine, "ONE");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Match_Mixed_No_CaseSensitive()
    {
        var sensitive = false;
        var engine = new FakeEngine() { CaseSensitiveNames = sensitive };

        var source = Identifier.Create(engine, "one");
        var target = Identifier.Create(engine, "..ONE");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "one");
        target = Identifier.Create(engine, "x.ONE");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, ".one");
        target = Identifier.Create(engine, "x.y.ONE");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "two.one");
        target = Identifier.Create(engine, "x.TWO.ONE");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "two.one");
        target = Identifier.Create(engine, "x..ONE");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "three.two.one");
        target = Identifier.Create(engine, "THREE.TWO.");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));
    }

    //[Enforced]
    [Fact]
    public static void Test_Match_Mixed_CaseSensitive()
    {
        var sensitive = true;
        var engine = new FakeEngine() { CaseSensitiveNames = sensitive };

        var source = Identifier.Create(engine, "one");
        var target = Identifier.Create(engine, "..ONE");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));

        source = Identifier.Create(engine, "one");
        target = Identifier.Create(engine, "x.ONE");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));

        source = Identifier.Create(engine, ".one");
        target = Identifier.Create(engine, "x.y.ONE");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));

        source = Identifier.Create(engine, "two.one");
        target = Identifier.Create(engine, "x.TWO.ONE");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));

        source = Identifier.Create(engine, "two.one");
        target = Identifier.Create(engine, "x..ONE");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));

        source = Identifier.Create(engine, "three.two.one");
        target = Identifier.Create(engine, "THREE.TWO.");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));
    }
}