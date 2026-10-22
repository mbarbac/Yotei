#pragma warning disable CA1859, IDE0305, IDE0028

using Tag = Yotei.ORM.Records.Code.MetadataTag;
using Item = Yotei.ORM.Records.Code.MetadataItem;
using Element = Yotei.ORM.Records.Code.SchemaElement;
using Builder = Yotei.ORM.Records.Code.SchemaElement.Builder;

namespace Yotei.ORM.Records.Tests;

// ========================================================
[Enforced]
public static class Test_SchemaElement
{
    static bool Contains(this ISchemaElement element, string name, object? value)
    {
        var item = element.Find(name);
        return item is not null && item.Value.EqualsEx(value);
    }
    static bool Contains(this ISchemaElement.IBuilder builder, string name, object? value)
    {
        var item = builder.Find(name);
        return item is not null && item.Value.EqualsEx(value);
    }

    static IEnumerable<IMetadataItem> Enumerate(this ISchemaElement element)
    {
        foreach (var item in element) yield return item;
    }

    static IEngine CreateEngine(bool withTags) => withTags
        ? new FakeEngine() { KnownTags = new FakeKnownTags(ignoreCase: true) }
        : new Engine() { KnownTags = new KnownTags(ignoreCase: true) };

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_NoTags()
    {
        var engine = CreateEngine(withTags: false);
        var element = new Element(engine);

        Assert.Null(element.Identifier);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.Null(element.IsReadOnly);
        Assert.Empty(element);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_WithTags()
    {
        var engine = CreateEngine(withTags: true);
        var element = new Element(engine);

        Assert.Null(element.Identifier);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.Null(element.IsReadOnly);
        Assert.Empty(element);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata_NoTags()
    {
        var engine = CreateEngine(withTags: false);
        var element = new Element(engine, [new Item("Age", 50)]);

        Assert.Null(element.Identifier);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.Null(element.IsReadOnly);
        Assert.Single(element);
        Assert.True(element.Contains("Age", 50));
        Assert.True(element.Contains("AGE", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata_WithTags()
    {
        var engine = CreateEngine(withTags: true);
        var element = new Element(engine, [new Item("Age", 50)]);

        Assert.Null(element.Identifier);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.Null(element.IsReadOnly);
        Assert.Single(element);
        Assert.True(element.Contains("Age", 50));
        Assert.True(element.Contains("AGE", 50));

        element = new Element(engine, [new Item("Age", 50), new Item("ColumnTag", "any")]);
        Assert.Equal("[any]", element.Identifier!.Value);
        Assert.Equal(2, element.Count);
        Assert.True(element.Contains("AGE", 50));
        Assert.True(element.Contains("COLUMNTAG3", "any"));

        element = new Element(engine, [new Item("SchemaTag3", "any"), new Item("ColumnTag", "other")]);
        Assert.Equal("[any]..[other]", element.Identifier!.Value);
        Assert.Equal(3, element.Count);
        Assert.True(element.Contains("SCHEMATAG", "any"));
        Assert.True(element.Contains("TABLETAG2", null));
        Assert.True(element.Contains("COLUMNTAG3", "other"));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Values_NoTags()
    {
        var engine = CreateEngine(withTags: false);
        var element = new Element(engine, "column", isReadOnly: true);

        Assert.Equal("[column]", element.Identifier!.Value);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.True(element.IsReadOnly);
        Assert.Empty(element);

        element = new Element(engine, "column", isReadOnly: true, range: [new Item("Age", 50)]);
        Assert.Equal("[column]", element.Identifier!.Value);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.True(element.IsReadOnly);
        Assert.Single(element);
        Assert.True(element.Contains("AGE", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Values_WithTags()
    {
        var engine = CreateEngine(withTags: true);
        var element = new Element(engine, ".column", isReadOnly: true);

        Assert.Equal("[column]", element.Identifier!.Value);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.True(element.IsReadOnly);
        Assert.Equal(2, element.Count);
        Assert.True(element.Contains("COLUMNTAG3", "column"));
        Assert.True(element.Contains("READONLYTAG3", true));

        element = new Element(engine, "schema..", isReadOnly: true, range: [new Item("Age", 50)]);

        Assert.Equal("[schema]..", element.Identifier!.Value);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.True(element.IsReadOnly);
        Assert.Equal(5, element.Count);
        Assert.True(element.Contains("SCHEMATAG3", "schema"));
        Assert.True(element.Contains("TABLETAG2", null));
        Assert.True(element.Contains("COLUMNTAG", null));
        Assert.True(element.Contains("READONLYTAG3", true));
        Assert.True(element.Contains("AGE", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Values_WithTags_Duplicated_In_Range()
    {
        var engine = CreateEngine(withTags: true);
        var element = new Element(engine, "schema..column", range: [
            new Item("SchemaTag2", "xhema"),
            new Item("ColumnTag3", "xolumn")]);

        Assert.Equal("[xhema]..[xolumn]", element.Identifier!.Value);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.Null(element.IsReadOnly);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Equal_NoTags()
    {
        var engine = CreateEngine(withTags: false);
        var source = new Element(engine);
        var target = new Element(engine);
        Assert.True(source.Equals(target));

        source = new Element(engine, "table.column", isUniqueValued: true, range: [new Item("Age", 50)]);
        target = new Element(engine, "table.column", isUniqueValued: true, range: [new Item("Age", 50)]);
        Assert.True(source.Equals(target));

        target = new Element(engine, "table.column", range: [new Item("Age", 50)]);
        Assert.False(source.Equals(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_Equal_WithTags()
    {
        var engine = CreateEngine(withTags: true);
        var source = new Element(engine);
        var target = new Element(engine);
        Assert.True(source.Equals(target));

        source = new Element(engine, "table.column", isUniqueValued: true, range: [new Item("Age", 50)]);
        target = new Element(engine, range: [new Item("UniqueValuedTag", true), new Item("Age", 50), new Item("COLUMNTAG2", "column"), new Item("TABLETAG", "table")]);
        Assert.True(source.Equals(target));

        target = new Element(engine, range: [new Item("Age", 50), new Item("COLUMNTAG2", "column"), new Item("TABLETAG", "table")]);
        Assert.False(source.Equals(target));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone_NoTags()
    {
        var engine = CreateEngine(withTags: false);
        var source = new Element(engine);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source, target);

        source = new Element(engine, "table.column", isUniqueValued: true, range: [new Item("Age", 50)]);
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone_WithTags()
    {
        var engine = CreateEngine(withTags: true);
        var source = new Element(engine);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source, target);

        source = new Element(engine, "table.column", isUniqueValued: true, range: [new Item("Age", 50)]);
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source, target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_With_Identifier_NoTags()
    {
        var engine = CreateEngine(withTags: false);

        var source = new Element(engine);
        var target = source.WithIdentifier(new Identifier(engine, "table.column"));
        Assert.NotSame(source, target);
        Assert.Equal("[table].[column]", target.Identifier!.Value);

        source = new Element(engine, "schema..");
        target = source.WithIdentifier(new Identifier(engine, "table.column"));
        Assert.NotSame(source, target);
        Assert.Equal("[table].[column]", target.Identifier!.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Identifier_WithTags()
    {
        var engine = CreateEngine(withTags: true);

        var source = new Element(engine);
        var target = source.WithIdentifier(new Identifier(engine, "table.column"));
        Assert.NotSame(source, target);
        Assert.Equal("[table].[column]", target.Identifier!.Value);
        Assert.Equal(2, target.Count);
        Assert.True(target.Contains("TableTag", "table"));
        Assert.True(target.Contains("ColumnTag", "column"));

        source = new Element(engine, "schema..");
        target = source.WithIdentifier(new Identifier(engine, "table.column"));
        Assert.NotSame(source, target);
        Assert.Equal("[table].[column]", target.Identifier!.Value);
        Assert.Equal(2, target.Count);
        Assert.True(target.Contains("TableTag", "table"));
        Assert.True(target.Contains("ColumnTag", "column"));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_With_IsPrimaryKey_NoTags()
    {
        var engine = CreateEngine(withTags: false);

        var source = new Element(engine);
        var target = source.WithIsPrimaryKey(false);
        Assert.NotSame(source, target);
        Assert.False(target.IsPrimaryKey);

        source = new Element(engine, "", isPrimaryKey: true);
        target = source.WithIsPrimaryKey(null);
        Assert.NotSame(source, target);
        Assert.Null(target.IsPrimaryKey);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_IsPrimaryKey_WithTags()
    {
        var engine = CreateEngine(withTags: true);

        var source = new Element(engine);
        var target = source.WithIsPrimaryKey(false);
        Assert.NotSame(source, target);
        Assert.False(target.IsPrimaryKey);
        Assert.Single(target);
        Assert.True(target.Contains("PRIMARYKEYTAG", false));

        source = new Element(engine, [new Item("PrimaryKeyTag", false)]);
        Assert.False(source.IsPrimaryKey);
        target = source.WithIsPrimaryKey(null);
        Assert.NotSame(source, target);
        Assert.Null(target.IsPrimaryKey);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_With_IsUniqueValued_NoTags()
    {
        var engine = CreateEngine(withTags: false);

        var source = new Element(engine);
        var target = source.WithIsUniqueValued(false);
        Assert.NotSame(source, target);
        Assert.False(target.IsUniqueValued);

        source = new Element(engine, "", isUniqueValued: true);
        target = source.WithIsUniqueValued(null);
        Assert.NotSame(source, target);
        Assert.Null(target.IsUniqueValued);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_IsUniqueValued_WithTags()
    {
        var engine = CreateEngine(withTags: true);

        var source = new Element(engine);
        var target = source.WithIsUniqueValued(false);
        Assert.NotSame(source, target);
        Assert.False(target.IsUniqueValued);
        Assert.Single(target);
        Assert.True(target.Contains("UNIQUEVALUEDTAG", false));

        source = new Element(engine, [new Item("UniqueValuedTag", false)]);
        Assert.False(source.IsUniqueValued);
        target = source.WithIsUniqueValued(null);
        Assert.NotSame(source, target);
        Assert.Null(target.IsUniqueValued);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_With_IsReadOnly_NoTags()
    {
        var engine = CreateEngine(withTags: false);

        var source = new Element(engine);
        var target = source.WithIsReadOnly(false);
        Assert.NotSame(source, target);
        Assert.False(target.IsReadOnly);

        source = new Element(engine, "", isReadOnly: true);
        target = source.WithIsReadOnly(null);
        Assert.NotSame(source, target);
        Assert.Null(target.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_IsReadOnly_WithTags()
    {
        var engine = CreateEngine(withTags: true);

        var source = new Element(engine);
        var target = source.WithIsReadOnly(false);
        Assert.NotSame(source, target);
        Assert.False(target.IsReadOnly);
        Assert.Single(target);
        Assert.True(target.Contains("READONLYTAG", false));

        source = new Element(engine, [new Item("ReadOnlyTag", false)]);
        Assert.False(source.IsReadOnly);
        target = source.WithIsReadOnly(null);
        Assert.NotSame(source, target);
        Assert.Null(target.IsReadOnly);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Enumerate_NoTags()
    {
        var engine = CreateEngine(withTags: false);
        var element = new Element(engine);
        var items = element.Enumerate().ToList();
        Assert.Empty(items);

        element = new Element(engine, "column", isReadOnly: true, range: [new Item("Age", 50)]);
        items = element.Enumerate().ToList();
        Assert.Single(items);
        Assert.Equal("Age", items[0].Name); Assert.Equal(50, items[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Enumerate_WithTags()
    {
        var engine = CreateEngine(withTags: true);
        var element = new Element(engine);
        var items = element.Enumerate().ToList();
        Assert.Empty(items);

        element = new Element(engine, "column", isReadOnly: true, range: [new Item("Age", 50)]);
        items = element.Enumerate().ToList();
        Assert.Equal(3, items.Count);
        Assert.NotNull(items.Find(x => x.Name == "Age" && (int)x.Value! == 50));
        Assert.NotNull(items.Find(x => x.Name == "ColumnTag" && (string)x.Value! == "column"));
        Assert.NotNull(items.Find(x => x.Name == "ReadOnlyTag" && (bool)x.Value! == true));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_This_Getter_NoTags()
    {
        var engine = CreateEngine(withTags: false);
        var element = new Element(engine);
        try { _ = element["any"]; Assert.Fail(); }
        catch (NotFoundException) { }

        element = new Element(engine, [new Item("Age", 50)]);
        Assert.Equal(50, element["AGE"]);
    }

    //[Enforced]
    [Fact]
    public static void Test_This_Getter_WithTags()
    {
        var engine = CreateEngine(withTags: true);
        var element = new Element(engine, "column");
        try { _ = element["any"]; Assert.Fail(); }
        catch (NotFoundException) { }

        element = new Element(engine, "column", isUniqueValued: false, range: [new Item("Age", 50)]);
        Assert.Equal(50, element["AGE"]);
        Assert.False((bool)element["UNIQUEVALUEDTAG"]!);
        Assert.Equal("column", element["COLUMNTAG"]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_This_Setter_NoTags()
    {
        var engine = CreateEngine(withTags: false);
        var builder = new Builder(engine);
        
        builder["Age"] = 50;
        Assert.Single(builder);
        Assert.True(builder.Contains("Age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_This_Setter_WithTags()
    {
        var engine = CreateEngine(withTags: true);
        var builder = new Builder(engine, "column");

        builder["SchemaTag"] = "schema";
        Assert.Equal("[schema]..[column]", builder.Identifier!.Value);
        Assert.True(builder.Contains("SchemaTag", "schema"));
        Assert.True(builder.Contains("TableTag", null));
        Assert.True(builder.Contains("ColumnTag", "column"));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find_NoTags()
    {
        var engine = CreateEngine(withTags: false);
        var element = new Element(engine);
        var item = element.Find("any");
        Assert.Null(item);

        element = new Element(engine, [new Item("Age", 50)]);
        item = element.Find("AGE");
        Assert.NotNull(item); Assert.Equal(50, item.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_WithTags()
    {
        var engine = CreateEngine(withTags: true);
        var element = new Element(engine, "table.", isUniqueValued: true, range: [new Item("Age", 50)]);

        var item = element.Find("Any"); Assert.Null(item);
        item = element.Find("TABLETAG"); Assert.Equal("table", item!.Value);
        item = element.Find("COLUMNTAG"); Assert.Null(item!.Value);
        item = element.Find("UNIQUEVALUEDTAG"); Assert.True((bool)item!.Value!);
        item = element.Find("AGE"); Assert.Equal(50, item!.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_NoTags()
    {
        var engine = CreateEngine(withTags: false);
        
        var source = new Element(engine);
        var target = source.Add(new Item("Age", 50));
        Assert.NotSame(source, target);
        Assert.True(target.Contains("AGE", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_WithTags()
    {
        var engine = CreateEngine(withTags: true);

        var source = new Element(engine);
        var target = source.Add("TableTag", "table");
        Assert.NotSame(source, target);
        Assert.Equal("[table].", target.Identifier!.Value);
        Assert.Equal(2, target.Count);
        Assert.True(target.Contains("TABLETAG", "table"));
        Assert.True(target.Contains("COLUMNTAG", null));

        source = new Element(engine, "column", isPrimaryKey: true);
        target = source.Add("SCHEMATAG", "schema");
        Assert.Equal(4, target.Count);
        Assert.Equal("[schema]..[column]", target.Identifier!.Value);
        Assert.True(target.Contains("SCHEMATAG", "schema"));
        Assert.True(target.Contains("TABLETAG", null));
        Assert.True(target.Contains("COLUMNTAG", "column"));
        Assert.True(target.Contains("PRIMARYKEYTAG", true));
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_AddRange_NoTags()

    //[Enforced]
    //[Fact]
    //public static void Test_AddRange_WithTags()

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Update_NoTags()
    {
        var engine = CreateEngine(withTags: false);

        var source = new Element(engine, [new Item("Age", 50)]);
        var target = source.Update("AGE", 60);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.True(target.Contains("age", 60));

        target = source.Update("Any", "other");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.True(target.Contains("age", 50));
        Assert.True(target.Contains("ANY", "other"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Update_WithTags()
    {
        var engine = CreateEngine(withTags: true);

        var source = new Element(engine, "column", isPrimaryKey: true);
        var target = source.Update("COLUMNTAG", "other");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.True(target.Contains("columntag", "other"));
        Assert.True(target.Contains("primarykeytag", true));

        target = source.Update("ReadOnlyTag", true);
        Assert.NotSame(source, target);
        Assert.True(target.IsReadOnly);
        Assert.Equal(3, target.Count);
        Assert.True(target.Contains("columntag", "column"));
        Assert.True(target.Contains("primarykeytag", true));
        Assert.True(target.Contains("readonlytag", true));
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_UpdateRange_NoTags()

    //[Enforced]
    //[Fact]
    //public static void Test_UpdateRange_WithTags()

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_NoTags()
    {
        var engine = CreateEngine(withTags: false);
        var source = new Element(engine);
        var target = source.Remove("any");
        Assert.Same(source, target);

        source = new Element(engine, [new Item("Age", 50)]);
        target = source.Remove("AGE");
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_WithTags()
    {
        var engine = CreateEngine(withTags: true);

        var source = new Element(engine, "schema..column", isPrimaryKey: true);
        var target = source.Remove("SCHEMATAG");
        Assert.NotSame(source, target);
        Assert.Equal("[column]", target.Identifier!.Value);
        Assert.Equal(2, target.Count);
        Assert.True(target.Contains("columntag", "column"));
        Assert.True(target.Contains("primarykeytag", true));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = CreateEngine(withTags: false);
        var source = new Element(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Element(engine, [new Item("Age", 50)]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Null(target.Identifier);
        Assert.Null(target.IsPrimaryKey);
        Assert.Null(target.IsUniqueValued);
        Assert.Null(target.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear_NoTags_WithTags()
    {
        var engine = CreateEngine(withTags: true);

        var source = new Element(engine, "schema..column", isPrimaryKey: true, range: [new Item("Age", 50)]);
        var target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Null(target.Identifier);
        Assert.Null(target.IsPrimaryKey);
        Assert.Null(target.IsUniqueValued);
        Assert.Null(target.IsReadOnly);
    }
}