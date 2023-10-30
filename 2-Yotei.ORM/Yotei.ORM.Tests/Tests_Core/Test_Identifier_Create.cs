namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Identifier_Create
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
}