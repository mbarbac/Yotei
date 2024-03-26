namespace Yotei.ORM.Records.Tests;

// ========================================================
//[Enforced]
public static class Test_Engine
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var engine = new FakeEngine();

        Assert.Equal(Code.Engine.CASESENSITIVETAGS, engine.KnownTags.CaseSensitiveTags);
        Assert.Equal(3, engine.KnownTags.IdentifierTags.Count);
        Assert.Equal("SchemaTag", engine.KnownTags.IdentifierTags[0].DefaultName);
        Assert.Equal("TableTag", engine.KnownTags.IdentifierTags[1].DefaultName);
        Assert.Equal("ColumnTag", engine.KnownTags.IdentifierTags[2].DefaultName);
        Assert.NotNull(engine.KnownTags.PrimaryKeyTag); Assert.Equal("PrimaryTag", engine.KnownTags.PrimaryKeyTag.DefaultName);
        Assert.NotNull(engine.KnownTags.UniqueValuedTag); Assert.Equal("UniqueTag", engine.KnownTags.UniqueValuedTag.DefaultName);
        Assert.NotNull(engine.KnownTags.ReadOnlyTag); Assert.Equal("ReadonlyTag", engine.KnownTags.ReadOnlyTag.DefaultName);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Methods()
    {
        var source = new FakeEngine();
        var target = source.WithKnownTags(new KnownTags(true));
        
        Assert.NotSame(source, target);
        Assert.True(target.KnownTags.CaseSensitiveTags);
    }

    //[Enforced]
    [Fact]
    public static void Test_Equals()
    {
        IEngine source = new FakeEngine();
        IEngine target = new FakeEngine();
        Assert.True(source.Equals(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_Equals_Inherited()
    {
        ORM.IEngine source = new FakeEngine();
        ORM.IEngine target = new FakeEngine();
        Assert.True(source.Equals(target));

        target = new ORM.Code.Engine();
        Assert.False(source.Equals(target));
    }
}