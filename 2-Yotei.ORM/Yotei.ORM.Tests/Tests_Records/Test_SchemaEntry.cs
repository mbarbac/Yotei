using Tag = Yotei.ORM.Records.Code.MetadataTag;
using Item = Yotei.ORM.Records.Code.MetadataItem;
using Entry = Yotei.ORM.Records.Code.SchemaEntry;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_SchemaEntry
{
    static bool Contains(this ISchemaEntry entry, string name, object? value)
    {
        var item = entry.Find(name);
        return item != null && item.Value.EqualsEx(value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var entry = new Entry(engine);

        Assert.Null(entry.Identifier);
        Assert.Null(entry.IsPrimaryKey);
        Assert.Null(entry.IsUniqueValued);
        Assert.Null(entry.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata_With_KnownTags()
    {
        var engine = new FakeEngine();
        var entry = new Entry(engine, [new Item("Age", 50)]);

        Assert.Null(entry.Identifier);
        Assert.Null(entry.IsPrimaryKey);
        Assert.Null(entry.IsUniqueValued);
        Assert.Null(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));

        entry = new Entry(engine, [
            new Item("TableTag2", "table"),
            new Item("PrimaryKeyTag3", true),
            new Item("Age", 50)]);

        Assert.Equal("[table].", entry.Identifier?.Value);
        Assert.True(entry.IsPrimaryKey);
        Assert.Null(entry.IsUniqueValued);
        Assert.Null(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata_No_KnownTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(true) };
        var entry = new Entry(engine, [new Item("Age", 50)]);

        Assert.Null(entry.Identifier);
        Assert.Null(entry.IsPrimaryKey);
        Assert.Null(entry.IsUniqueValued);
        Assert.Null(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));

        entry = new Entry(engine, [
            new Item("TableTag", "table"),
            new Item("PrimaryKeyTag", true),
            new Item("Age", 50)]);

        Assert.Null(entry.Identifier);
        Assert.Null(entry.IsPrimaryKey);
        Assert.Null(entry.IsUniqueValued);
        Assert.Null(entry.IsReadOnly);

        Assert.True(entry.Contains("TableTag", "table"));
        Assert.True(entry.Contains("PrimaryKeyTag", true));
        Assert.True(entry.Contains("Age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Values_With_KnownTags()
    {
        var engine = new FakeEngine();
        var entry = new Entry(engine, "column", isReadOnly: true);

        Assert.Equal("[column]", entry.Identifier?.Value);
        Assert.Null(entry.IsPrimaryKey);
        Assert.Null(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);

        entry = new Entry(engine, "column", isUniqueValued: true, range: [
            new Item("TableTag3", "table"),
            new Item("Age", 50)]);

        Assert.Equal("[table].[column]", entry.Identifier?.Value);
        Assert.Null(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.Null(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Values_No_KnownTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(true) };
        var entry = new Entry(engine, "column", isReadOnly: true);

        Assert.Equal("[column]", entry.Identifier?.Value);
        Assert.Null(entry.IsPrimaryKey);
        Assert.Null(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);

        entry = new Entry(engine, "column", isUniqueValued: true, range: [
            new Item("TableTag3", "table"),
            new Item("Age", 50)]);

        Assert.Equal("[column]", entry.Identifier?.Value); // No "table" as no well-knows...!
        Assert.Null(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.Null(entry.IsReadOnly);
        Assert.True(entry.Contains("TableTag3", "table"));
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
        Assert.Equal("[table].", target.Identifier?.Value);
        Assert.Equal(source.IsPrimaryKey, target.IsPrimaryKey);
        Assert.Equal(source.IsUniqueValued, target.IsUniqueValued);
        Assert.Equal(source.IsReadOnly, target.IsReadOnly);
        Assert.True(target.Contains("Age", 50));
        Assert.True(target.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Identifier()
    {
        var engine = new FakeEngine();
        var source = new Entry(engine, "table.");

        var target = source.WithIdentifier(new Identifier(engine, "column"));
        Assert.NotSame(source, target);
        Assert.Equal("[column]", target.Identifier?.Value);

        target = source.WithIdentifier(null);
        Assert.NotSame(source, target);
        Assert.Null(target.Identifier);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Special_Identifier()
    {
        var engine = new FakeEngine();
        var source = new Entry(engine, "table.");

        var builder = source.ToBuilder();        
        var id = new Identifier(engine, "other");
        builder.Identifier = id;
        Assert.Equal("[other]", builder.Identifier?.Value);

        var fake = new FakeEngine();
        id = new Identifier(fake, "table.");
        try { builder.Identifier = id; Assert.Fail(); }
        catch (ArgumentException) { }

        
    }

    //[Enforced]
    [Fact]
    public static void Test_With_IsPrimaryKey()
    {
        var engine = new FakeEngine();
        var source = new Entry(engine, "table.");

        var target = source.WithIsPrimaryKey(true);
        Assert.NotSame(source, target);
        Assert.True(target.IsPrimaryKey);

        target = source.WithIsPrimaryKey(false);
        Assert.NotSame(source, target);
        Assert.False(target.IsPrimaryKey);

        target = source.WithIsPrimaryKey(null);
        Assert.NotSame(source, target);
        Assert.Null(target.IsPrimaryKey);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_IsUniqueValued()
    {
        var engine = new FakeEngine();
        var source = new Entry(engine, "table.");

        var target = source.WithIsUniqueValued(true);
        Assert.NotSame(source, target);
        Assert.True(target.IsUniqueValued);

        target = source.WithIsUniqueValued(false);
        Assert.NotSame(source, target);
        Assert.False(target.IsUniqueValued);

        target = source.WithIsUniqueValued(null);
        Assert.NotSame(source, target);
        Assert.Null(target.IsUniqueValued);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_IsReadOnly()
    {
        var engine = new FakeEngine();
        var source = new Entry(engine, "table.");

        var target = source.WithIsReadOnly(true);
        Assert.NotSame(source, target);
        Assert.True(target.IsReadOnly);

        target = source.WithIsReadOnly(false);
        Assert.NotSame(source, target);
        Assert.False(target.IsReadOnly);

        target = source.WithIsReadOnly(null);
        Assert.NotSame(source, target);
        Assert.Null(target.IsReadOnly);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine() { KnownTags = new FakeKnownTags(true) };
        var entry = new Entry(engine, "table.", range: [new Item("Age", 50)]);

        Assert.True(entry.Contains("SchemaTag", null));
        Assert.True(entry.Contains("TableTag", "table"));
        Assert.True(entry.Contains("ColumnTag", null));
        Assert.True(entry.Contains("Age", 50));

        Assert.True(entry.Contains("SCHEMATAG3", null));
        Assert.True(entry.Contains("TABLETAG3", "table"));
        Assert.True(entry.Contains("COLUMNTAG3", null));

        Assert.Null(entry.Find("PrimaryKeyTag"));
        Assert.Null(entry.Find("UniqueValuedTag"));
        Assert.Null(entry.Find("ReadOnlyTag"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Many()
    {
        var engine = new FakeEngine() { KnownTags = new FakeKnownTags(true) };
        var entry = new Entry(engine, "table.", range: [new Item("Age", 50)]);

        var items = entry.Find(["any", "SCHEMATAG2", "TABLETAG3", "Age"]);
        Assert.Equal(3, items.Count);
        Assert.Equal("SchemaTag", items[0].Name); Assert.Null(items[0].Value);
        Assert.Equal("TableTag", items[1].Name); Assert.Equal("table", items[1].Value);
        Assert.Equal("Age", items[2].Name); Assert.Equal(50, items[2].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine() { KnownTags = new FakeKnownTags(true) };
        var source = new Entry(engine, "table.");

        var target = source.Add("Age", 50);
        Assert.NotSame(source, target);
        Assert.True(target.Contains("Age", 50));

        target = source.Add("ColumnTag3", "column");
        Assert.NotSame(source, target);
        Assert.Equal("[table].[column]", target.Identifier?.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Duplicated_WellKnown()
    {
        var engine = new FakeEngine() { KnownTags = new FakeKnownTags(true) };
        var source = new Entry(engine, "table.");

        var target = source.Add("TableTag2", "other");
        Assert.NotSame(source, target);
        Assert.Equal("[other].", target.Identifier?.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Duplicated_Other()
    {
        var engine = new FakeEngine() { KnownTags = new FakeKnownTags(true) };
        var source = new Entry(engine, "table.", range: [new Item("Age", 50)]);

        try { var target = source.Add("Age", 100); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Range()
    {
        var engine = new FakeEngine() { KnownTags = new FakeKnownTags(true) };
        var source = new Entry(engine, "table.");

        var target = source.AddRange([new Item("ColumnTag", "column"), new Item("Age", 50)]);
        Assert.NotSame(source, target);
        Assert.Equal("[table].[column]", target.Identifier?.Value);
        Assert.True(target.Contains("Age", 50));
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Update() => throw null;

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Update_Range() => throw null;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var engine = new FakeEngine() { KnownTags = new FakeKnownTags(true) };
        var source = new Entry(engine, "table.", range: [new Item("Age", 50)]);

        var target = source.Remove("any");
        Assert.Same(source, target);

        target = source.Remove("TableTag");
        Assert.NotSame(source, target);
        Assert.Null(target.Identifier);

        target = source.Remove("Age");
        Assert.NotSame(source, target);
        Assert.False(target.Contains("Age"));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine() { KnownTags = new FakeKnownTags(true) };
        var source = new Entry(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Entry(engine, "column");
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Null(target.Identifier);

        source = new Entry(engine, "column", isPrimaryKey: true, range: [new Item("Age", 50)]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Null(target.Identifier);
        Assert.Null(target.IsPrimaryKey);
        Assert.False(target.Contains("Age"));
    }
}