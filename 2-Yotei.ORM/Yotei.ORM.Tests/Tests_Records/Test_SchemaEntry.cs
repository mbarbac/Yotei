namespace Yotei.ORM.Records.Tests;

// ========================================================
//[Enforced]
public static class Test_SchemaEntry
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine);

        Assert.Equal(6, entry.Count);
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[0]));
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[1]));
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[2]));
        Assert.True(entry.Contains(engine.KnownTags.PrimaryKeyTag!));
        Assert.True(entry.Contains(engine.KnownTags.UniqueValuedTag!));
        Assert.True(entry.Contains(engine.KnownTags.ReadOnlyTag!));

        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var entry = new SchemaEntry(engine);

        Assert.Equal(0, entry.Count);

        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(
            engine,
            "one.two",
            isUniqueValued: true,
            metadata: [new MetadataPair("age", 50)]);

        Assert.Equal(7, entry.Count);
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[0]));
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[1]));
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[2]));
        Assert.True(entry.Contains(engine.KnownTags.PrimaryKeyTag!));
        Assert.True(entry.Contains(engine.KnownTags.UniqueValuedTag!));
        Assert.True(entry.Contains(engine.KnownTags.ReadOnlyTag!));
        Assert.True(entry.Contains("age"));

        Assert.Equal("[one].[two]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("age", 50));
    }
    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var entry = new SchemaEntry(
            engine,
            "one.two",
            isUniqueValued: true,
            metadata: [new MetadataPair("age", 50)]);

        Assert.Equal(1, entry.Count);
        Assert.True(entry.Contains("age"));

        Assert.Equal("[one].[two]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("age", 50));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Metadata()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(
            engine,
            metadata: [
                new MetadataPair(engine.KnownTags.IdentifierTags[0], "one"),
                new MetadataPair(engine.KnownTags.IdentifierTags[2], "three"),
                new MetadataPair(engine.KnownTags.UniqueValuedTag!, true),
                new MetadataPair("age", 50)
            ]);

        Assert.Equal(7, entry.Count);
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[0]));
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[1]));
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[2]));
        Assert.True(entry.Contains(engine.KnownTags.PrimaryKeyTag!));
        Assert.True(entry.Contains(engine.KnownTags.UniqueValuedTag!));
        Assert.True(entry.Contains(engine.KnownTags.ReadOnlyTag!));
        Assert.True(entry.Contains("age"));

        Assert.Equal("[one]..[three]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Metadata_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var entry = new SchemaEntry(
            engine,
            "one..three",
            isUniqueValued: true,
            metadata: [
                new MetadataPair("age", 50)
            ]);

        Assert.Equal(1, entry.Count);
        Assert.True(entry.Contains("age"));

        Assert.Equal("[one]..[three]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("age", 50));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two", isUniqueValued: true,
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Clone();
        Assert.NotSame(source, target);

        Assert.Equal(7, target.Count);
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[0]));
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[1]));
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[2]));
        Assert.True(target.Contains(engine.KnownTags.PrimaryKeyTag!));
        Assert.True(target.Contains(engine.KnownTags.UniqueValuedTag!));
        Assert.True(target.Contains(engine.KnownTags.ReadOnlyTag!));
        Assert.True(target.Contains("age"));

        Assert.Equal("[one].[two]", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.True(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
        Assert.True(target.Contains("age", 50));
    }
    //[Enforced]
    [Fact]
    public static void Test_Clone_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine,
            "one.two",
            isUniqueValued: true,
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Clone();
        Assert.NotSame(source, target);

        Assert.Equal(1, target.Count);
        Assert.True(target.Contains("age"));

        Assert.Equal("[one].[two]", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.True(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
        Assert.True(target.Contains("age", 50));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_WithIdentifier()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three", isUniqueValued: true,
            metadata: [new MetadataPair("age", 50)]);

        var target = source.WithIdentifier(new Identifier(engine, "x"));
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.Equal("[x]", target.Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_WithIdentifier_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine, "one.two", isUniqueValued: true,
            metadata: [new MetadataPair("age", 50)]);

        var target = source.WithIdentifier(new Identifier(engine, "x"));
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("[x]", target.Identifier.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_WithPrimaryKey()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.WithIsPrimaryKey(true);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.IsPrimaryKey);
    }

    //[Enforced]
    [Fact]
    public static void Test_WithPrimaryKey_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine, "one.two",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.WithIsPrimaryKey(true);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.True(target.IsPrimaryKey);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_WithUniqueValued()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.WithIsUniqueValued(true);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.IsUniqueValued);
    }

    //[Enforced]
    [Fact]
    public static void Test_WithUniqueValuedNoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine, "one.two",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.WithIsUniqueValued(true);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.True(target.IsUniqueValued);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_WithReadOnly()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.WithIsReadOnly(true);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_WithReadOnly_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine, "one.two",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.WithIsReadOnly(true);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.True(target.IsReadOnly);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace_IdentifierTag()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three", isUniqueValued: true,
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Replace(engine.KnownTags.IdentifierTags[0], "one");
        Assert.Same(source, target);

        target = source.Replace(engine.KnownTags.IdentifierTags[0], "ONE");
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.Equal("[ONE].[two].[three]", target.Identifier.Value);

        target = source.Replace(engine.KnownTags.IdentifierTags[1], null);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.Equal("[one]..[three]", target.Identifier.Value);

        target = source.Replace(engine.KnownTags.IdentifierTags[2], null);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.Equal("[one].[two].", target.Identifier.Value);

        try { _ = source.Replace(engine.KnownTags.IdentifierTags[0], 1); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_PrimaryKeyTag()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Replace(engine.KnownTags.PrimaryKeyTag!, false);
        Assert.Same(source, target);

        target = source.Replace(engine.KnownTags.PrimaryKeyTag!, true);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.IsPrimaryKey);

        try { _ = source.Replace(engine.KnownTags.PrimaryKeyTag!, 1); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_UniqueValuedTag()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Replace(engine.KnownTags.UniqueValuedTag!, false);
        Assert.Same(source, target);

        target = source.Replace(engine.KnownTags.UniqueValuedTag!, true);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.IsUniqueValued);

        try { _ = source.Replace(engine.KnownTags.UniqueValuedTag!, 1); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_ReadOnlyTag()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Replace(engine.KnownTags.ReadOnlyTag!, false);
        Assert.Same(source, target);

        target = source.Replace(engine.KnownTags.ReadOnlyTag!, true);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.IsReadOnly);

        try { _ = source.Replace(engine.KnownTags.ReadOnlyTag!, 1); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace_Other()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Replace("age", 50);
        Assert.Same(source, target);

        target = source.Replace("AGE", 60);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.Contains("AGE", 60));

        target = source.Replace("any", "other");
        Assert.NotSame(source, target);
        Assert.Equal(8, target.Count);
        Assert.True(target.Contains("ANY", "other"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Other_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine, "one.two",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Replace("AGE", 60);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.True(target.Contains("age", 60));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_IdentifierTag()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three",
            metadata: [new MetadataPair("age", 50)]);

        var tag = engine.KnownTags.IdentifierTags[0];
        var target = source.Add(new MetadataPair(tag, "one"));
        Assert.Same(source, target);

        target = source.Add(new MetadataPair(tag, "ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.Equal("[ONE].[two].[three]", target.Identifier.Value);

        try { source.Add(new MetadataPair(tag, 1)); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_PrimaryKeyTag()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three",
            metadata: [new MetadataPair("age", 50)]);

        var tag = engine.KnownTags.PrimaryKeyTag;
        var target = source.Add(new MetadataPair(tag!, false));
        Assert.Same(source, target);

        target = source.Add(new MetadataPair(tag!, true));
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.IsPrimaryKey);

        try { source.Add(new MetadataPair(tag!, 1)); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_UniqueValuedTag()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three",
            metadata: [new MetadataPair("age", 50)]);

        var tag = engine.KnownTags.UniqueValuedTag;
        var target = source.Add(new MetadataPair(tag!, false));
        Assert.Same(source, target);

        target = source.Add(new MetadataPair(tag!, true));
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.IsUniqueValued);

        try { source.Add(new MetadataPair(tag!, 1)); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_ReadOnlyTag()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three",
            metadata: [new MetadataPair("age", 50)]);

        var tag = engine.KnownTags.ReadOnlyTag;
        var target = source.Add(new MetadataPair(tag!, false));
        Assert.Same(source, target);

        target = source.Add(new MetadataPair(tag!, true));
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.IsReadOnly);

        try { source.Add(new MetadataPair(tag!, 1)); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Other()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Add(new MetadataPair("any", "other"));
        Assert.NotSame(source, target);
        Assert.Equal(8, target.Count);
        Assert.True(target.Contains("ANY", "other"));

        try { target.Add(new MetadataPair("any", "another")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Add(new MetadataPair("", null)); Assert.Fail(); }
        catch (EmptyException) { }

        try { source.Add(new MetadataPair("", 1)); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine, "one.two");

        var target = source.Add(new MetadataPair("age", 50));
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.True(target.Contains("age", 50));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Known()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three", isUniqueValued: true,
            metadata: [new MetadataPair("age", 50)]);

        var tag = engine.KnownTags.IdentifierTags[0];
        var target = source.Remove(tag);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.Equal("[two].[three]", target.Identifier.Value);

        tag = engine.KnownTags.IdentifierTags[1];
        target = source.Remove(tag);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.Equal("[one]..[three]", target.Identifier.Value);

        tag = engine.KnownTags.IdentifierTags[2];
        target = source.Remove(tag);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.Equal("[one].[two].", target.Identifier.Value);

        tag = engine.KnownTags.UniqueValuedTag!;
        target = source.Remove(tag!);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.False(target.IsUniqueValued);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Other()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three", isUniqueValued: true,
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Remove("AGE");
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.False(target.Contains("age"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine, "one.two",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Remove("age");
        Assert.NotSame(source, target);
        Assert.Equal(0, target.Count);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(
            engine, "one.two.three", isUniqueValued: true,
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Null(target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine, "one.two",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Equal(0, target.Count);
        Assert.Null(target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
    }
}