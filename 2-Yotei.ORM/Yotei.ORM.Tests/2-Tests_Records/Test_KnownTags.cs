namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_KnownTags
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var tags = new KnownTags();
        Assert.False(tags.IgnoreCase);
        Assert.Null(tags.IdentifierTags);
        Assert.Null(tags.PrimaryKeyTag);
        Assert.Null(tags.UniqueValuedTag);
        Assert.Null(tags.ReadOnlyTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Default_FakeInstance()
    {
        var tags = new FakeKnownTags();
        Assert.False(tags.IgnoreCase);

        Assert.Equal(3, tags.IdentifierTags!.Value.Length);
        Assert.Equal("SchemaTag", tags.IdentifierTags.Value[0].Default);
        Assert.Equal("TableTag", tags.IdentifierTags.Value[1].Default);
        Assert.Equal("ColumnTag", tags.IdentifierTags.Value[2].Default);

        Assert.NotNull(tags.PrimaryKeyTag); Assert.Equal("PrimaryKeyTag", tags.PrimaryKeyTag.Default);
        Assert.NotNull(tags.UniqueValuedTag); Assert.Equal("UniqueValuedTag", tags.UniqueValuedTag.Default);
        Assert.NotNull(tags.ReadOnlyTag); Assert.Equal("ReadOnlyTag", tags.ReadOnlyTag.Default);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Custom_FakeInstance()
    {
        var tags = new FakeKnownTags(true)
        {
            IdentifierTags = [
                new MetadataTag(true, "Schema"),
                new MetadataTag(true, "Table"),
                new MetadataTag(true, "Column")],

            PrimaryKeyTag = new MetadataTag(true, "Primary"),
            UniqueValuedTag = new MetadataTag(true, "Unique"),
            ReadOnlyTag = new MetadataTag(true, "ReadOnly"),
        };

        Assert.True(tags.IgnoreCase);

        Assert.Equal(3, tags.IdentifierTags!.Value.Length);
        Assert.Equal("Schema", tags.IdentifierTags.Value[0].Default);
        Assert.Equal("Table", tags.IdentifierTags.Value[1].Default);
        Assert.Equal("Column", tags.IdentifierTags.Value[2].Default);

        Assert.NotNull(tags.PrimaryKeyTag); Assert.Equal("Primary", tags.PrimaryKeyTag.Default);
        Assert.NotNull(tags.UniqueValuedTag); Assert.Equal("Unique", tags.UniqueValuedTag.Default);
        Assert.NotNull(tags.ReadOnlyTag); Assert.Equal("ReadOnly", tags.ReadOnlyTag.Default);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new FakeKnownTags();
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(source.IgnoreCase, target.IgnoreCase);
        Assert.Equal(source.IdentifierTags, target.IdentifierTags);
        Assert.Same(source.PrimaryKeyTag, target.PrimaryKeyTag);
        Assert.Same(source.UniqueValuedTag, target.UniqueValuedTag);
        Assert.Same(source.ReadOnlyTag, target.ReadOnlyTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_IdentifierTags()
    {
        var source = new FakeKnownTags();

        var target = source.WithIdentifierTags(null);
        Assert.NotSame(source, target);
        Assert.Null(target.IdentifierTags);

        target = source.WithIdentifierTags([]);
        Assert.NotSame(source, target);
        Assert.Null(target.IdentifierTags);

        target = source.WithIdentifierTags([new MetadataTag(false, "Column")]);
        Assert.NotSame(source, target);
        Assert.NotNull(target.IdentifierTags);
        Assert.Single(target.IdentifierTags);
        Assert.Equal("Column", target.IdentifierTags.Value[0].Default);

        try { target = source.WithIdentifierTags([new MetadataTag(true, "Column")]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { target = source.WithIdentifierTags([new MetadataTag(false, "ReadOnlyTag")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_PrimaryKeyTag()
    {
        var source = new FakeKnownTags();

        var target = source.WithPrimaryKeyTag(null);
        Assert.NotSame(source, target);
        Assert.Null(target.PrimaryKeyTag);

        target = source.WithPrimaryKeyTag(new MetadataTag(false, "Other"));
        Assert.NotSame(source, target);
        Assert.NotNull(target.PrimaryKeyTag);
        Assert.Equal("Other", target.PrimaryKeyTag!.Default);

        try { source.WithPrimaryKeyTag(new MetadataTag(false, "SchemaTag")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_UniqueValuedTag()
    {
        var source = new FakeKnownTags();

        var target = source.WithUniqueValuedTag(null);
        Assert.NotSame(source, target);
        Assert.Null(target.UniqueValuedTag);

        target = source.WithUniqueValuedTag(new MetadataTag(false, "Other"));
        Assert.NotSame(source, target);
        Assert.NotNull(target.UniqueValuedTag);
        Assert.Equal("Other", target.UniqueValuedTag!.Default);

        try { source.WithUniqueValuedTag(new MetadataTag(false, "SchemaTag")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_ReadOnlyTag()
    {
        var source = new FakeKnownTags();

        var target = source.WithReadOnlyTag(null);
        Assert.NotSame(source, target);
        Assert.Null(target.ReadOnlyTag);

        target = source.WithReadOnlyTag(new MetadataTag(false, "Other"));
        Assert.NotSame(source, target);
        Assert.NotNull(target.ReadOnlyTag);
        Assert.Equal("Other", target.ReadOnlyTag!.Default);

        try { source.WithReadOnlyTag(new MetadataTag(false, "SchemaTag")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Enumerate_Names()
    {
        var source = new FakeKnownTags();
        Assert.Equal([
            "SchemaTag", "SchemaTag2", "SchemaTag3",
            "TableTag", "TableTag2", "TableTag3",
            "ColumnTag", "ColumnTag2", "ColumnTag3",
            "PrimaryKeyTag", "PrimaryKeyTag2", "PrimaryKeyTag3",
            "UniqueValuedTag", "UniqueValuedTag2", "UniqueValuedTag3",
            "ReadOnlyTag", "ReadOnlyTag2", "ReadOnlyTag3",
            ],
            source.Names);
    }

    //[Enforced]
    [Fact]
    public static void Test_Contains()
    {
        var source = new FakeKnownTags();
        Assert.False(source.Contains("one"));

        Assert.True(source.Contains("SchemaTag3"));
        Assert.True(source.Contains("TableTag2"));
        Assert.True(source.Contains("ColumnTag"));
        Assert.True(source.Contains("PrimaryKeyTag3"));
        Assert.True(source.Contains("UniqueValuedTag2"));
        Assert.True(source.Contains("ReadOnlyTag"));
    }

    //[Enforced]
    [Fact]
    public static void Test_ContainsAny()
    {
        var source = new FakeKnownTags();
        Assert.False(source.Contains(["one", "two"]));

        Assert.True(source.Contains(["one", "two", "SchemaTag3"]));
        Assert.True(source.Contains(["one", "two", "TableTag2"]));
        Assert.True(source.Contains(["one", "two", "ColumnTag"]));
        Assert.True(source.Contains(["one", "two", "PrimaryKeyTag3"]));
        Assert.True(source.Contains(["one", "two", "UniqueValuedTag2"]));
        Assert.True(source.Contains(["one", "two", "ReadOnlyTag"]));
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var tags = new FakeKnownTags(true)
        {
            IdentifierTags = [
                new MetadataTag(true, "Schema"),
                new MetadataTag(true, "Table"),
                new MetadataTag(true, "Column")],

            PrimaryKeyTag = new MetadataTag(true, "Primary"),
            UniqueValuedTag = new MetadataTag(true, "Unique"),
            ReadOnlyTag = new MetadataTag(true, "ReadOnly"),
        };

        Assert.NotNull(tags.Find("SCHEMA"));
        Assert.NotNull(tags.Find("TABLE"));
        Assert.NotNull(tags.Find("COLUMN"));
        Assert.NotNull(tags.Find("PRIMARY"));
        Assert.NotNull(tags.Find("UNIQUE"));
        Assert.NotNull(tags.Find("READONLY"));
    }
}