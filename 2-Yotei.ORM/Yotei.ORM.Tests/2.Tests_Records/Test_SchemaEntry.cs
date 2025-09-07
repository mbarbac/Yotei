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
        var source = new SchemaEntry(engine);

        Assert.Null(source.Identifier.Value);
        Assert.False(source.IsPrimaryKey);
        Assert.False(source.IsUniqueValued);
        Assert.False(source.IsReadOnly);
        Assert.Equal(3, source.Count);
        Assert.Equal(0, source.RawCount);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_Special()
    {
        var engine = new Engine();
        var source = new SchemaEntry(engine);

        Assert.Null(source.Identifier.Value);
        Assert.False(source.IsPrimaryKey);
        Assert.False(source.IsUniqueValued);
        Assert.False(source.IsReadOnly);
        Assert.Equal(0, source.Count);
        Assert.Equal(0, source.RawCount);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Standard()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, "column");
        Assert.Equal("[column]", source.Identifier.Value);
        Assert.False(source.IsPrimaryKey);
        Assert.False(source.IsUniqueValued);
        Assert.False(source.IsReadOnly);
        Assert.Equal(4, source.Count);
        Assert.Equal(1, source.RawCount);

        source = new SchemaEntry(engine, "table.column", isReadOnly: true);
        Assert.Equal("[table].[column]", source.Identifier.Value);
        Assert.False(source.IsPrimaryKey);
        Assert.False(source.IsUniqueValued);
        Assert.True(source.IsReadOnly);
        Assert.Equal(5, source.Count);
        Assert.Equal(3, source.RawCount);

        source = new SchemaEntry(engine, "schema..column", isReadOnly: true, range: [new Entry("Age", 50)]);
        Assert.Equal("[schema]..[column]", source.Identifier.Value);
        Assert.False(source.IsPrimaryKey);
        Assert.False(source.IsUniqueValued);
        Assert.True(source.IsReadOnly);
        Assert.Equal(7, source.Count);
        Assert.Equal(5, source.RawCount);
        Assert.True(source.Contains("Age", 50));

        try { _ = new SchemaEntry(engine, "other.schema.table.column"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Standard_Special()
    {
        var engine = new Engine();
        var source = new SchemaEntry(engine, "column");
        Assert.Equal("[column]", source.Identifier.Value);
        Assert.False(source.IsPrimaryKey);
        Assert.False(source.IsUniqueValued);
        Assert.False(source.IsReadOnly);
        Assert.Equal(0, source.Count);
        Assert.Equal(0, source.RawCount);

        source = new SchemaEntry(engine, "table.column", isReadOnly: true);
        Assert.Equal("[table].[column]", source.Identifier.Value);
        Assert.False(source.IsPrimaryKey);
        Assert.False(source.IsUniqueValued);
        Assert.True(source.IsReadOnly);
        Assert.Equal(0, source.Count);
        Assert.Equal(0, source.RawCount);

        source = new SchemaEntry(engine, "schema..column", isReadOnly: true, range: [new Entry("Age", 50)]);
        Assert.Equal("[schema]..[column]", source.Identifier.Value);
        Assert.False(source.IsPrimaryKey);
        Assert.False(source.IsUniqueValued);
        Assert.True(source.IsReadOnly);
        Assert.Equal(1, source.Count);
        Assert.Equal(1, source.RawCount);
        Assert.True(source.Contains("Age", 50));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, [
            new Entry("SchemaTag", "schema"),
            new Entry("ColumnTag", "column"),
            new Entry("ReadOnlyTag", true),
            new Entry("Age", 50)]);

        Assert.Equal("[schema]..[column]", source.Identifier.Value);
        Assert.False(source.IsPrimaryKey);
        Assert.False(source.IsUniqueValued);
        Assert.True(source.IsReadOnly);
        Assert.Equal(7, source.Count);
        Assert.Equal(4, source.RawCount);
        Assert.True(source.Contains("Age", 50));
        Assert.True(source.Contains("Column3", "column"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata_Special()
    {
        var engine = new Engine();
        var source = new SchemaEntry(engine, [
            new Entry("SchemaTag", "schema"),
            new Entry("ColumnTag", "column"),
            new Entry("ReadOnlyTag", true),
            new Entry("Age", 50)]);

        Assert.Null(source.Identifier.Value);
        Assert.False(source.IsPrimaryKey);
        Assert.False(source.IsUniqueValued);
        Assert.False(source.IsReadOnly);
        Assert.Equal(4, source.Count);
        Assert.Equal(4, source.RawCount);
        Assert.True(source.Contains("Age", 50));
        Assert.False(source.Contains("Column3"));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, "schema..column", isReadOnly: true, range: [new Entry("Age", 50)]);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal("[schema]..[column]", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.True(target.IsReadOnly);
        Assert.Equal(7, target.Count);
        Assert.Equal(5, ((SchemaEntry)target).RawCount);
        Assert.True(target.Contains("Age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone_Special()
    {
        var engine = new Engine();
        var source = new SchemaEntry(engine, "schema..column", isReadOnly: true, range: [new Entry("Age", 50)]);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal("[schema]..[column]", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.True(target.IsReadOnly);
        Assert.Equal(1, target.Count);
        Assert.Equal(1, ((SchemaEntry)target).RawCount);
        Assert.True(target.Contains("Age", 50));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_With_Methods()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, "schema..column", isReadOnly: true, range: [new Entry("Age", 50)]);

        var target = source.WithIdentifier(Identifier.Create(engine, "alpha.beta"));
        Assert.NotSame(source, target);
        Assert.Equal("[alpha].[beta]", target.Identifier.Value);
        Assert.Equal(6, target.Count);
        Assert.Equal(4, ((SchemaEntry)target).RawCount);

        target = source.WithIsPrimaryKey(true);
        Assert.NotSame(source, target);
        Assert.True(target.IsPrimaryKey);
        Assert.Equal(7, target.Count);
        Assert.Equal(6, ((SchemaEntry)target).RawCount);

        target = source.WithIsUniqueValued(true);
        Assert.NotSame(source, target);
        Assert.True(target.IsUniqueValued);
        Assert.Equal(7, target.Count);
        Assert.Equal(6, ((SchemaEntry)target).RawCount);

        target = source.WithIsReadOnly(false);
        Assert.NotSame(source, target);
        Assert.False(target.IsReadOnly);
        Assert.Equal(7, target.Count);
        Assert.Equal(5, ((SchemaEntry)target).RawCount);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Methods_Special()
    {
        var engine = new Engine();
        var source = new SchemaEntry(engine, "schema..column", isReadOnly: true, range: [new Entry("Age", 50)]);

        var target = source.WithIdentifier(Identifier.Create(engine, "alpha.beta"));
        Assert.NotSame(source, target);
        Assert.Equal("[alpha].[beta]", target.Identifier.Value);
        Assert.Equal(1, target.Count);
        Assert.Equal(1, ((SchemaEntry)target).RawCount);

        target = source.WithIsPrimaryKey(true);
        Assert.NotSame(source, target);
        Assert.True(target.IsPrimaryKey);
        Assert.Equal(1, target.Count);
        Assert.Equal(1, ((SchemaEntry)target).RawCount);

        target = source.WithIsUniqueValued(true);
        Assert.NotSame(source, target);
        Assert.True(target.IsUniqueValued);
        Assert.Equal(1, target.Count);
        Assert.Equal(1, ((SchemaEntry)target).RawCount);

        target = source.WithIsReadOnly(false);
        Assert.NotSame(source, target);
        Assert.False(target.IsReadOnly);
        Assert.Equal(1, target.Count);
        Assert.Equal(1, ((SchemaEntry)target).RawCount);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "schema..column", isPrimaryKey: true, range: [new Entry("Age", 50)]);

        Assert.True(source.Contains("TABLETAG", null));
        Assert.True(source.Contains("COLUMN3", "column"));
        Assert.False(source.Contains("Other"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Special()
    {
        var engine = new Engine();
        var source = new SchemaEntry(engine, [
            new Entry("SchemaTag", "schema"),
            new Entry("ColumnTag", "column"),
            new Entry("PrimaryTag", true)]);

        Assert.True(source.Contains("SCHEMATAG", "schema"));
        Assert.False(source.Contains("TableTag"));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find_Range()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "schema..column", isPrimaryKey: true, range: [new Entry("Age", 50)]);

        var item = source.Find(["alpha", "TableTag", "Column3"]);
        Assert.NotNull(item);
        Assert.Equal("TableTag", item.Name);
        Assert.Null(item.Value);

        item = source.Find(["alpha", "beta", "Column3"]);
        Assert.NotNull(item);
        Assert.Equal("ColumnTag", item.Name);
        Assert.Equal("column", item.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Range_Special()
    {
        var engine = new Engine();
        var source = new SchemaEntry(engine, [
            new Entry("SchemaTag", "schema"),
            new Entry("ColumnTag", "column"),
            new Entry("PrimaryTag", true)]);

        var item = source.Find(["alpha", "TableTag", "Column3"]);
        Assert.Null(item);

        item = source.Find(["alpha", "beta", "PRIMARYTAG"]);
        Assert.NotNull(item);
        Assert.Equal("PrimaryTag", item.Name);
        Assert.Equal(true, item.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "schema..column", isPrimaryKey: true, range: [new Entry("Age", 50)]);

        var target = source.Replace("Column3", "other");
        Assert.NotSame(target, source);
        Assert.Equal("[schema]..[other]", target.Identifier.Value);
        Assert.Equal(7, target.Count);
        Assert.Equal(5, ((SchemaEntry)target).RawCount);

        target = source.Replace("PrimaryTag", false);
        Assert.NotSame(target, source);
        Assert.False(target.IsPrimaryKey);
        Assert.Equal(7, target.Count);
        Assert.Equal(5, ((SchemaEntry)target).RawCount);

        try { source.Replace("Any", true); Assert.Fail(); }
        catch (NotFoundException) { }

        target = source.Replace("UniqueTag", true);
        Assert.NotSame(target, source);
        Assert.True(target.IsUniqueValued);
        Assert.Equal(7, target.Count);
        Assert.Equal(6, ((SchemaEntry)target).RawCount);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Special()
    {
        var engine = new Engine();
        var source = new SchemaEntry(
            engine, "schema..column", isPrimaryKey: true, range: [new Entry("Age", 50)]);

        var target = source.Replace("Age", 25);
        Assert.NotSame(target, source);
        Assert.True(target.Contains("Age", 25));
        Assert.Equal(1, target.Count);
        Assert.Equal(1, ((SchemaEntry)target).RawCount);

        try { source.Replace("Any", true); Assert.Fail(); }
        catch (NotFoundException) { }

        try { source.Replace("UniqueTag", true); Assert.Fail(); }
        catch (NotFoundException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, "column");

        var target = source.Add(new Entry("PrimaryTag", true));
        Assert.NotSame(source, target);
        Assert.True(target.IsPrimaryKey);
        Assert.Equal(4, target.Count);
        Assert.Equal(2, ((SchemaEntry)target).RawCount);

        try { _ = source.Add(new Entry("Column3", "other")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.Add(new Entry("SchemaTag", 50)); Assert.Fail(); }
        catch (ArgumentException) { }

        source = new SchemaEntry(engine, "column", isPrimaryKey: true);
        try { _ = source.Add(new Entry("PrimaryTag", false)); Assert.Fail(); }
        catch (DuplicateException) { }

        target = source.Add(new Entry("UniqueTag", true));
        Assert.NotSame(source, target);
        Assert.True(target.IsUniqueValued);
        Assert.Equal(4, target.Count);
        Assert.Equal(3, ((SchemaEntry)target).RawCount);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Special()
    {
        var engine = new Engine();
        var source = new SchemaEntry(engine, "column");

        var target = source.Add(new Entry("PrimaryTag", true));
        Assert.NotSame(source, target);
        Assert.True(target.Contains("PrimaryTag", true));
        Assert.Equal(1, target.Count);
        Assert.Equal(1, ((SchemaEntry)target).RawCount);

        target = source.Add(new Entry("SchemaTag", 50));
        Assert.NotSame(source, target);
        Assert.True(target.Contains("SchemaTag", 50));
        Assert.Equal(1, target.Count);
        Assert.Equal(1, ((SchemaEntry)target).RawCount);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, "column");

        var target = source.AddRange([
            new Entry("SchemaTag", "schema"),
            new Entry("PrimaryTag", true)]);

        Assert.NotSame(source, target);
        Assert.Equal("[schema]..[column]", target.Identifier.Value);
        Assert.True(target.IsPrimaryKey);
        Assert.Equal(6, target.Count);
        Assert.Equal(4, ((SchemaEntry)target).RawCount);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Name()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "schema..column", isPrimaryKey: true, range: [new Entry("Age", 50)]);

        var target = source.Remove("Column3");
        Assert.NotSame(source, target);
        Assert.Equal("[schema]..", target.Identifier.Value);
        Assert.Equal(7, target.Count);
        Assert.Equal(5, ((SchemaEntry)target).RawCount);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "schema..column", isPrimaryKey: true, range: [new Entry("Age", 50)]);

        var target = source.RemoveAll(x => x.Value is bool value && value);
        Assert.NotSame(source, target);
        Assert.False(target.IsPrimaryKey);
        Assert.Equal(7, target.Count);
        Assert.Equal(4, ((SchemaEntry)target).RawCount);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new SchemaEntry(engine, "column", range: [new Entry("Age", 50)]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Null(target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
        Assert.Equal(3, target.Count);
        Assert.Equal(0, ((SchemaEntry)target).RawCount);
    }
}