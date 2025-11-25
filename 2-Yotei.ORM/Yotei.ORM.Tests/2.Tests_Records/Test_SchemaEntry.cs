using Tag = Yotei.ORM.Records.Code.MetadataTag;
using Entry = Yotei.ORM.Records.Code.SchemaEntry;
using Item = Yotei.ORM.Records.Code.MetadataItem;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_SchemaEntry
{
    static bool Contains(this ISchemaEntry entry, string name, object? value)
    {
        var item = entry.Find(name);
        return item is not null && item.Value.EqualsEx(value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var entry = new Entry(engine);

        Assert.Equal(3, entry.Count);
        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_NotKnownTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var entry = new Entry(engine);

        Assert.Equal(0, entry.Count);
        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Values()
    {
        var engine = new FakeEngine();
        var entry = new Entry(engine, "column", isReadOnly: true);

        Assert.Equal(4, entry.Count);
        Assert.Equal("[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);

        entry = new Entry(engine, "column", isUniqueValued: true);

        Assert.Equal(4, entry.Count);
        Assert.Equal("[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);

        entry = new Entry(engine, "[table].", range: [new Item("Age", 50)]);

        Assert.Equal(6, entry.Count);
        Assert.Equal("[table].", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));

        entry = new Entry(engine, "[schema]..", isPrimaryKey: true, range: [new Item("Age", 50)]);

        Assert.Equal(7, entry.Count);
        Assert.Equal("[schema]..", entry.Identifier.Value);
        Assert.True(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));

        try { _ = new Entry(engine, "one.two.three.four"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Values_NotKnownTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var entry = new Entry(engine, "column", isReadOnly: true, range: [new Item("Age", 50)]);

        Assert.Equal(1, entry.Count);
        Assert.Equal("[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata()
    {
        var engine = new FakeEngine();
        var entry = new Entry(engine, [new Item("Age", 50)]);

        Assert.Equal(4, entry.Count);
        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));

        entry = new Entry(engine, [
            new Item("TableTag2", "table"),
            new Item("PrimaryKeyTag3", true),
            new Item("Age", 50)]);

        Assert.Equal(6, entry.Count);
        Assert.Equal("[table].", entry.Identifier.Value);
        Assert.True(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata_NotKnownTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var entry = new Entry(engine, [new Item("Age", 50)]);

        Assert.Equal(1, entry.Count);
        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));

        entry = new Entry(engine, "table.", range: [new Item("Age", 50)]);

        Assert.Equal(1, entry.Count);
        Assert.Equal("[table].", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new Entry(engine, "table.", isReadOnly: true, range: [new Item("Age", 50)]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Equal("[table].", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.True(target.IsReadOnly);
        Assert.True(target.Contains("Age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone_NotKnownTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new Entry(engine, "table.", isReadOnly: true, range: [new Item("Age", 50)]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("[table].", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.True(target.IsReadOnly);
        Assert.True(target.Contains("Age", 50));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_WithIdentifier()
    {
        var engine = new FakeEngine();
        var source = new Entry(engine, "table.", isReadOnly: true, range: [new Item("Age", 50)]);
        var target = source.WithIdentifier("column");

        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[column]", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.True(target.IsReadOnly);
        Assert.True(target.Contains("Age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_WithIdentifier_NotKnows()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new Entry(engine, "table.", isReadOnly: true, range: [new Item("Age", 50)]);
        var target = source.WithIdentifier("column");

        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("[column]", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.True(target.IsReadOnly);
        Assert.True(target.Contains("Age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_WithOthers()
    {
        var engine = new FakeEngine();
        var source = new Entry(engine, "table.", isReadOnly: true, range: [new Item("Age", 50)]);

        var target = source.WithIsPrimaryKey(true);
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Equal("[table].", target.Identifier.Value);
        Assert.True(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.True(target.IsReadOnly);
        Assert.True(target.Contains("Age", 50));

        target = source.WithIsUniqueValued(true);
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Equal("[table].", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.True(target.IsUniqueValued);
        Assert.True(target.IsReadOnly);
        Assert.True(target.Contains("Age", 50));

        target = source.WithIsReadOnly(false);
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Equal("[table].", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
        Assert.True(target.Contains("Age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_WithOthers_NotKnownTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new Entry(engine, "table.", isReadOnly: true, range: [new Item("Age", 50)]);

        var target = source.WithIsPrimaryKey(true);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("[table].", target.Identifier.Value);
        Assert.True(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.True(target.IsReadOnly);
        Assert.True(target.Contains("Age", 50));

        target = source.WithIsUniqueValued(true);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("[table].", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.True(target.IsUniqueValued);
        Assert.True(target.IsReadOnly);
        Assert.True(target.Contains("Age", 50));

        target = source.WithIsReadOnly(false);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("[table].", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
        Assert.True(target.Contains("Age", 50));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var entry = new Entry(engine, "table.", isReadOnly: true, range: [new Item("Age", 50)]);
        var item = entry.Find("any");
        Assert.Null(item);

        item = entry.Find("TableTag3");
        Assert.NotNull(item);
        Assert.Equal("TableTag", item.Name); Assert.Equal("table", item.Value);

        item = entry.Find("Age");
        Assert.NotNull(item);
        Assert.Equal("Age", item.Name); Assert.Equal(50, item.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_FindRange()
    {
        var engine = new FakeEngine();
        var entry = new Entry(engine, "table.", isReadOnly: true, range: [new Item("Age", 50)]);
        var item = entry.Find("any");
        Assert.Null(item);

        item = entry.Find(["any", "other", "TableTag3"]);
        Assert.NotNull(item);
        Assert.Equal("TableTag", item.Name); Assert.Equal("table", item.Value);

        item = entry.Find(["any", "other", "Age"]);
        Assert.NotNull(item);
        Assert.Equal("Age", item.Name); Assert.Equal(50, item.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_KnownTag() // No duplicates for well-known tags...
    {
        var engine = new FakeEngine();
        var source = new Entry(engine, "column", isReadOnly: true, range: [new Item("Age", 50)]);

        var target = source.Add(new Item("SchemaTag2", null));
        Assert.Same(source, target);

        target = source.Add(new Item("SchemaTag2", "schema"));
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.Equal("[schema]..[column]", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.True(target.IsReadOnly);
        Assert.True(target.Contains("Age", 50));

        target = source.Add(new Item("PrimaryKeyTag3", true));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[column]", target.Identifier.Value);
        Assert.True(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.True(target.IsReadOnly);
        Assert.True(target.Contains("Age", 50));

        target = source.Add(new Item("UniqueValuedTag", true));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[column]", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.True(target.IsUniqueValued);
        Assert.True(target.IsReadOnly);
        Assert.True(target.Contains("Age", 50));

        target = source.Add(new Item("ReadOnlyTag2", false));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[column]", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
        Assert.True(target.Contains("Age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_ArbitraryTag() // Duplicates not allowed...
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new Entry(engine, "table.", isReadOnly: true, range: [new Item("Age", 50)]);

        var target = source.Add(new Item("Name", "any"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.True(target.Contains("Age", 50));
        Assert.True(target.Contains("NAME", "any"));

        try { source.Add(new Item("AGE", 60)); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_AddRange

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_IdentifierTag()
    {
        var engine = new FakeEngine();
        var source = new Entry(engine, "schema..column");
        Assert.Equal("[schema]..[column]", source.Identifier.Value);

        var target = source.Remove("ColumnTag2");
        Assert.NotSame(source, target);
        Assert.Equal("[schema]..", target.Identifier.Value);
        Assert.Equal(6, target.Count);

        target = source.Remove("SchemaTag3");
        Assert.NotSame(source, target);
        Assert.Equal("[column]", target.Identifier.Value);
        Assert.Equal(4, target.Count);

        source = new Entry(engine, "column");
        target = source.Remove("TableTag2"); Assert.Same(source, target);
        target = source.Remove("SchemaTag3"); Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_OtherKnownTags()
    {
        var engine = new FakeEngine();
        var source = new Entry(engine, "column", isReadOnly: true, range: [new Item("Age", 50)]);

        var target = source.Remove("PrimaryKeyTag");
        Assert.Same(source, target);

        target = source.Remove("UniqueValued2");
        Assert.Same(source, target);

        target = source.Remove("ReadOnlyTag3");
        Assert.NotSame(source, target);
        Assert.False(target.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Arbitrary()
    {
        var engine = new FakeEngine();
        var source = new Entry(engine, "column", isReadOnly: true, range: [new Item("Age", 50)]);
        
        var target = source.Remove("any");
        Assert.Same(source, target);

        target = source.Remove("AGE");
        Assert.NotSame(source, target);
        Assert.Null(target.Find("Age"));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate_KnownTags()
    {
        var engine = new FakeEngine();
        var source = new Entry(engine, "table.", isReadOnly: true);

        var target = source.Remove(x => x.Name.Contains("Table"));
        Assert.NotSame(source, target);
        Assert.Null(target.Identifier.Value);

        source = new Entry(engine, "", isPrimaryKey: true, isReadOnly: true);
        Assert.True(source.IsPrimaryKey);
        Assert.True(source.IsReadOnly);

        target = source.Remove(x => x.Name.Contains('y'));
        Assert.NotSame(source, target);
        Assert.False(target.IsPrimaryKey);
        Assert.True(target.IsReadOnly);

        target = source.RemoveLast(x => x.Name.Contains('y'));
        Assert.NotSame(source, target);
        Assert.True(target.IsPrimaryKey);
        Assert.False(target.IsReadOnly);

        target = source.RemoveAll(x => x.Name.Contains('y'));
        Assert.NotSame(source, target);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate_Arbitrary()
    {
        var source = new Entry(
            new FakeEngine() { KnownTags = new KnownTags(false) },
            [new Item("xOne", 50), new Item("zTwo", 50), new Item("xThree", 50)]);

        var target = source.Remove(x => x.Name.Contains('@'));
        Assert.Same(source, target);

        target = source.Remove(x => x.Name.Contains('x'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.False(target.Contains("xOne"));
        Assert.True(target.Contains("zTwo"));
        Assert.True(target.Contains("xThree"));

        target = source.RemoveLast(x => x.Name.Contains('x'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.True(target.Contains("xOne"));
        Assert.True(target.Contains("zTwo"));
        Assert.False(target.Contains("xThree"));

        target = source.RemoveAll(x => x.Name.Contains('x'));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.False(target.Contains("xOne"));
        Assert.True(target.Contains("zTwo"));
        Assert.False(target.Contains("xThree"));
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Clear
}