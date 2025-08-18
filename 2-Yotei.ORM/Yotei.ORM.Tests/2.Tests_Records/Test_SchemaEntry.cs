using Tag = Yotei.ORM.Records.Code.MetadataTag;
using Pair = Yotei.ORM.Records.Code.MetadataEntry;

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

        entry = new SchemaEntry(engine, "schema..column", isReadOnly: true, range: [new Pair("Age", 50)]);
        Assert.Equal("[schema]..[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);
        Assert.NotNull(entry.Find("Age"));
        Assert.Equal(7, entry.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine, [
            new Pair("TableTag", "table"),
            new Pair("ColumnTag", "column"),
            new Pair("ReadOnlyTag", true),
            new Pair("Age", 50),]);

        Assert.Equal("[table].[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);
        Assert.NotNull(entry.Find("Age"));
        Assert.Equal(6, entry.Count);

        entry = new SchemaEntry(engine, [
            new Pair("SchemaTag", "schema"),
            new Pair("ColumnTag", "column"),
            new Pair("UniqueTag", true),
            new Pair("Age", 50),]);

        var id = entry.Identifier;
        Assert.Equal("[schema]..[column]", id.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.NotNull(entry.Find("Age"));
        Assert.Equal(7, entry.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "schema..column", isPrimaryKey: true, range: [new Pair("Age", 50)]);
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
            engine, "schema..column", isPrimaryKey: true, range: [new Pair("Age", 50)]);
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

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(
            engine, "schema..column", isReadOnly: true, range: [new Pair("Age", 50)]);
        Assert.Equal(7, entry.Count);

        var item = entry.Find("COLUMNTAG");
        Assert.NotNull(item);
        Assert.Equal("column", item.Value);

        item = entry.Find("PrimaryTag"); Assert.Null(item);
        item = entry.Find("UniqueTag"); Assert.Null(item);
        item = entry.Find("READONLYTag"); Assert.True((bool)item!.Value!);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Range()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(
            engine, "schema..column", isReadOnly: true, range: [new Pair("Age", 50)]);

        var item = entry.Find(["Whatever", "COLUMNTAG"]);
        Assert.NotNull(item);
        Assert.Equal("column", item.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace_WellKnown_Present()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "table.column", isPrimaryKey: true, range: [new Pair("Age", 50)]);

        var target = source.Replace("TableTag", "other");
        Assert.NotSame(source, target);
        Assert.Equal("[other].[column]", target.Identifier.Value);

        target = source.Replace("PrimaryTag", false);
        Assert.NotSame(source, target);
        Assert.False(target.IsPrimaryKey);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_WellKnown_Not_Present()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine);
        Assert.Equal(3, source.Count);

        var target = source.Replace("ColumnTag", "column");
        Assert.NotSame(source, target);
        Assert.Equal("[column]", target.Identifier.Value);
        Assert.Equal(4, target.Count);

        target = source.Replace("UniqueTag", true);
        Assert.NotSame(source, target);
        Assert.True(target.IsUniqueValued);
        Assert.Equal(3, target.Count);

        //
        //var source = new SchemaEntry(
        //    engine, "table.column", isPrimaryKey: true, range: [new Pair("Age", 50)]);

        //var target = source.Replace("TableTag", "other");
        //Assert.NotSame(source, target);
        //Assert.Equal("[other].[column]", target.Identifier.Value);

        //target = source.Replace("PrimaryTag", false);
        //Assert.NotSame(source, target);
        //Assert.False(target.IsPrimaryKey);
    }
}