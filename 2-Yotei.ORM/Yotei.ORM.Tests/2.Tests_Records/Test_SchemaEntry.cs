using Tag = Yotei.ORM.Records.Code.MetadataTag;
using Entry = Yotei.ORM.Records.Code.MetadataEntry;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_SchemaEntry
{
    /// <summary>
    /// Validates that the given collection contains a pair with the given name and value.
    /// </summary>
    static bool Contains(this SchemaEntry entry, string name, object? value)
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
        var entry = new SchemaEntry(engine);

        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.Equal(3, entry.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine, "column");
        Assert.Equal("[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.Equal(4, entry.Count);

        entry = new SchemaEntry(engine, "table.column", isReadOnly: true);
        Assert.Equal("[table].[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);
        Assert.Equal(5, entry.Count);

        entry = new SchemaEntry(engine, "schema..column", isReadOnly: true, range: [new Entry("Age", 50)]);
        Assert.Equal("[schema]..[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);
        Assert.NotNull(entry.Find("Age"));
        Assert.Equal(7, entry.Count);

        try { _ = new SchemaEntry(engine, "other.schema.table.column"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine, [
            new Entry("TableTag", "table"),
            new Entry("ColumnTag", "column"),
            new Entry("ReadOnlyTag", true),
            new Entry("Age", 50),]);

        Assert.Equal("[table].[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);
        Assert.NotNull(entry.Find("Age"));
        Assert.Equal(6, entry.Count);

        entry = new SchemaEntry(engine, [
            new Entry("SchemaTag", "schema"),
            new Entry("ColumnTag", "column"),
            new Entry("UniqueTag", true),
            new Entry("Age", 50),]);

        Assert.Equal("[schema]..[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.NotNull(entry.Find("Age"));
        Assert.Equal(7, entry.Count);

        entry = new SchemaEntry(engine, [
            new Entry("SchemaTag", "schema"),
            new Entry("UniqueTag", true),]);

        Assert.Equal("[schema]..", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.Equal(6, entry.Count);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "schema..column", isPrimaryKey: true, range: [new Entry("Age", 50)]);
        Assert.Equal(7, source.Count);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal("[schema]..[column]", target.Identifier.Value);
        Assert.True(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
        Assert.NotNull(target.Find("Age"));
        Assert.Equal(7, target.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Methods()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "schema..column", isPrimaryKey: true, range: [new Entry("Age", 50)]);
        Assert.Equal(7, source.Count);

        var target = source.WithIdentifier(Identifier.Create(engine, "alpha.beta"));
        Assert.NotSame(source, target);
        Assert.Equal("[alpha].[beta]", target.Identifier.Value);
        Assert.Equal(6, target.Count);

        target = source.WithIsPrimaryKey(false);
        Assert.NotSame(source, target);
        Assert.False(target.IsPrimaryKey);
        Assert.Equal(7, target.Count);

        target = source.WithIsUniqueValued(true);
        Assert.NotSame(source, target);
        Assert.True(target.IsUniqueValued);
        Assert.Equal(7, target.Count);

        target = source.WithIsReadOnly(true);
        Assert.NotSame(source, target);
        Assert.True(target.IsReadOnly);
        Assert.Equal(7, target.Count);
    }
}