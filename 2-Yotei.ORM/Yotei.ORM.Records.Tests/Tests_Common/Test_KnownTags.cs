namespace Yotei.ORM.Records.Tests;

// ========================================================
//[Enforced]
public static class Test_KnownTags
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var tags = new KnownTags(false);

        Assert.Empty(tags);
        Assert.Empty(tags.IdentifierTags);
        Assert.Null(tags.PrimaryKeyTag);
        Assert.Null(tags.UniqueValuedTag);
        Assert.Null(tags.ReadOnlyTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var tags = new FakeKnownTags(false);

        Assert.NotEmpty(tags);
        Assert.Equal(3, tags.IdentifierTags.Count);
        Assert.Equal("SchemaTag", tags.IdentifierTags[0].DefaultName);
        Assert.Equal("TableTag", tags.IdentifierTags[1].DefaultName);
        Assert.Equal("ColumnTag", tags.IdentifierTags[2].DefaultName);
        Assert.NotNull(tags.PrimaryKeyTag); Assert.Equal("PrimaryTag", tags.PrimaryKeyTag.DefaultName);
        Assert.NotNull(tags.UniqueValuedTag); Assert.Equal("UniqueTag", tags.UniqueValuedTag.DefaultName);
        Assert.NotNull(tags.ReadOnlyTag); Assert.Equal("ReadonlyTag", tags.ReadOnlyTag.DefaultName);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var tags = new FakeKnownTags(false);

        Assert.False(tags.Contains("any"));
        Assert.True(tags.Contains("SCHEMATAG"));
        Assert.True(tags.Contains(["any", "UNIQUETAG"]));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new FakeKnownTags(false);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Same(source.IdentifierTags, target.IdentifierTags);
        Assert.Same(source.PrimaryKeyTag, target.PrimaryKeyTag);
        Assert.Same(source.UniqueValuedTag, target.UniqueValuedTag);
        Assert.Same(source.ReadOnlyTag, target.ReadOnlyTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_IdentifierTags()
    {
        var source = new FakeKnownTags(false);

        var items = new IdentifierTags(false);
        var target = source.WithIdentifierTags(items);
        Assert.NotSame(source, target);
        Assert.Same(items, target.IdentifierTags);

        items = new IdentifierTags(false, new MetadataTag(false, "SchemaTag"));
        target = source.WithIdentifierTags(items);
        Assert.NotSame(source, target);
        Assert.Same(items, target.IdentifierTags);

        items = new IdentifierTags(false, new MetadataTag(false, "PRIMARYTAG"));
        try { _ = source.WithIdentifierTags(items); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.WithIdentifierTags(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_PrimaryKeyTag()
    {
        var source = new FakeKnownTags(false);

        var item = new MetadataTag(false, "any");
        var target = source.WithPrimaryKeyTag(item);
        Assert.NotSame(source, target);
        Assert.Same(item, target.PrimaryKeyTag);

        source = target;
        target = source.WithPrimaryKeyTag(item);
        Assert.Same(source, target);
        Assert.Same(item, target.PrimaryKeyTag);

        item = new MetadataTag(false, "SCHEMATAG");
        try { _ = source.WithPrimaryKeyTag(item); Assert.Fail(); }
        catch (DuplicateException) { }

        target = source.WithPrimaryKeyTag(null);
        Assert.NotSame(source, target);
        Assert.Null(target.PrimaryKeyTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_UniqueValuedTag()
    {
        var source = new FakeKnownTags(false);

        var item = new MetadataTag(false, "any");
        var target = source.WithUniqueValuedTag(item);
        Assert.NotSame(source, target);
        Assert.Same(item, target.UniqueValuedTag);

        source = target;
        target = source.WithUniqueValuedTag(item);
        Assert.Same(source, target);
        Assert.Same(item, target.UniqueValuedTag);

        item = new MetadataTag(false, "SCHEMATAG");
        try { _ = source.WithUniqueValuedTag(item); Assert.Fail(); }
        catch (DuplicateException) { }

        target = source.WithUniqueValuedTag(null);
        Assert.NotSame(source, target);
        Assert.Null(target.UniqueValuedTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_ReadOnlyTag()
    {
        var source = new FakeKnownTags(false);

        var item = new MetadataTag(false, "any");
        var target = source.WithReadOnlyTag(item);
        Assert.NotSame(source, target);
        Assert.Same(item, target.ReadOnlyTag);

        source = target;
        target = source.WithReadOnlyTag(item);
        Assert.Same(source, target);
        Assert.Same(item, target.ReadOnlyTag);

        item = new MetadataTag(false, "SCHEMATAG");
        try { _ = source.WithReadOnlyTag(item); Assert.Fail(); }
        catch (DuplicateException) { }

        target = source.WithReadOnlyTag(null);
        Assert.NotSame(source, target);
        Assert.Null(target.ReadOnlyTag);
    }
}