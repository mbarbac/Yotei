namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_KnownTags
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new KnownTags(false);
        Assert.Empty(items.IdentifierTags);
        Assert.Null(items.PrimaryKeyTag);
        Assert.Null(items.UniqueValuedTag);
        Assert.Null(items.ReadOnlyTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var items = FakeKnownTags.Create(false);
        Assert.Equal(3, items.IdentifierTags.Count);
        Assert.Equal("SchemaTag", items.IdentifierTags[0]);
        Assert.Equal("TableTag", items.IdentifierTags[1]);
        Assert.Equal("ColumnTag", items.IdentifierTags[2]);
        Assert.Equal("PrimaryTag", items.PrimaryKeyTag);
        Assert.Equal("UniqueTag", items.UniqueValuedTag);
        Assert.Equal("ReadOnlyTag", items.ReadOnlyTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_Equality()
    {
        var source = FakeKnownTags.Create(false);
        var target = FakeKnownTags.Create(false);
        Assert.Equal(source, target);

        target = target.WithPrimaryKeyTag("other");
        Assert.NotEqual(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Methods()
    {
        var source = FakeKnownTags.Create(false);

        var ids = new IdentifierTags(false, "x.y.z");
        var target = source.WithIdentifierTags(ids);
        Assert.NotSame(source, target);
        Assert.Same(ids, target.IdentifierTags);

        target = source.WithPrimaryKeyTag("any");
        Assert.NotSame(source, target);
        Assert.Equal("any", target.PrimaryKeyTag);

        target = source.WithPrimaryKeyTag(null);
        Assert.NotSame(source, target);
        Assert.Null(target.PrimaryKeyTag);

        target = source.WithUniqueValuedTag("any");
        Assert.NotSame(source, target);
        Assert.Equal("any", target.UniqueValuedTag);

        target = source.WithUniqueValuedTag(null);
        Assert.NotSame(source, target);
        Assert.Null(target.UniqueValuedTag);

        target = source.WithReadOnlyTag("any");
        Assert.NotSame(source, target);
        Assert.Equal("any", target.ReadOnlyTag);

        target = source.WithReadOnlyTag(null);
        Assert.NotSame(source, target);
        Assert.Null(target.ReadOnlyTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Methods_Errors()
    {
        var source = FakeKnownTags.Create(false);

        try { _ = source.WithPrimaryKeyTag("SchemaTag"); Assert.Fail(); }
        catch (DuplicateException) { }
        try { _ = source.WithUniqueValuedTag("TableTag"); Assert.Fail(); }
        catch (DuplicateException) { }
        try { _ = source.WithReadOnlyTag("ColumnTag"); Assert.Fail(); }
        catch (DuplicateException) { }

        source = source.WithIdentifierTags(new IdentifierTags(false));
        Assert.Empty(source.IdentifierTags);

        try { _ = source.WithIdentifierTags(new IdentifierTags(false, "PrimaryTag")); Assert.Fail(); }
        catch (DuplicateException) { }
        try { _ = source.WithIdentifierTags(new IdentifierTags(false, "UniqueTag")); Assert.Fail(); }
        catch (DuplicateException) { }
        try { _ = source.WithIdentifierTags(new IdentifierTags(false, "ReadOnlyTag")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_CaseSensitive()
    {
        var source = FakeKnownTags.Create(false);
        var target = source.WithCaseSensitiveTags(true);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.IdentifierTags.Count);
        Assert.Equal(source.IdentifierTags[0], target.IdentifierTags[0]);
        Assert.Equal(source.IdentifierTags[1], target.IdentifierTags[1]);
        Assert.Equal(source.IdentifierTags[2], target.IdentifierTags[2]);
        Assert.Equal(source.PrimaryKeyTag, target.PrimaryKeyTag);
        Assert.Equal(source.UniqueValuedTag, target.UniqueValuedTag);
        Assert.Equal(source.ReadOnlyTag, target.ReadOnlyTag);
    }
}