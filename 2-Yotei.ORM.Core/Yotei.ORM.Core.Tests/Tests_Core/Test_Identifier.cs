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
        var item = Identifier.Create(engine, null);
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
    public static void Test_Single_Match()
    {
        var engine = new FakeEngine();
        var source = Identifier.Create(engine, null);
        var target = Identifier.Create(engine, null);
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "one");
        target = Identifier.Create(engine, null);
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "one");
        target = Identifier.Create(engine, "ONE");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));
    }

    //[Enforced]
    [Fact]
    public static void Test_Single_No_Match()
    {
        var engine = new FakeEngine();

        var source = Identifier.Create(engine, "one");
        var target = Identifier.Create(engine, "two");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Mixed_Match()
    {
        var engine = new FakeEngine();
        var source = Identifier.Create(engine, "one");
        var target = Identifier.Create(engine, "..ONE");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "one");
        target = Identifier.Create(engine, "two.ONE");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, ".one");
        target = Identifier.Create(engine, "THREE.TWO.ONE");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "two.one");
        target = Identifier.Create(engine, "THREE.TWO.ONE");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "two.one");
        target = Identifier.Create(engine, "THREE..ONE");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "one.two.three");
        target = Identifier.Create(engine, "ONE.TWO.");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));
    }

    //[Enforced]
    [Fact]
    public static void Test_Mixed_No_Match()
    {
        var engine = new FakeEngine();
        var source = Identifier.Create(engine, "one");
        var target = Identifier.Create(engine, "ONE.two");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));

        source = Identifier.Create(engine, "one.x");
        target = Identifier.Create(engine, "one.two");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));

        source = Identifier.Create(engine, "x.one");
        target = Identifier.Create(engine, "two.one");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));

        source = Identifier.Create(engine, "one.two.three");
        target = Identifier.Create(engine, "one.x.three");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));

        source = Identifier.Create(engine, "one.two.three");
        target = Identifier.Create(engine, "one.x.");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));

        source = Identifier.Create(engine, "one.two.three");
        target = Identifier.Create(engine, "x.three");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));
    }
}