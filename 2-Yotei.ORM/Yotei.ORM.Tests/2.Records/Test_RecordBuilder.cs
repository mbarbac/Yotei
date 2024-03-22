namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_RecordBuilder
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);

        var record = builder.ToRecord(out var schema);
        Assert.Empty(record);
        Assert.Empty(schema);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);

        var builder = new RecordBuilder("007", xid);
        var record = builder.ToRecord(out var schema);
        Assert.Single(record);
        Assert.Single(schema);
        Assert.Equal("007", record[0]); Assert.Equal("[Emp].[Id]", schema[0].Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Multiple()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Emp.First");
        var xlast = new SchemaEntry(engine, "Last");

        var builder = new RecordBuilder(["007", "James", "Bond"], [xid, xfirst, xlast]);
        var record = builder.ToRecord(out var schema);
        Assert.Equal(3, record.Count);
        Assert.Equal(3, schema.Count);
        Assert.Equal("007", record[0]); Assert.Equal("[Emp].[Id]", schema[0].Identifier.Value);
        Assert.Equal("James", record[1]); Assert.Equal("[Emp].[First]", schema[1].Identifier.Value);
        Assert.Equal("Bond", record[2]); Assert.Equal("[Last]", schema[2].Identifier.Value);

        try { builder = new RecordBuilder(null!, []); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { builder = new RecordBuilder([], null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { builder = new RecordBuilder([], []); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { builder = new RecordBuilder([], [null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { builder = new RecordBuilder([], [xid]); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Emp.First");
        var xlast = new SchemaEntry(engine, "Last");

        var builder = new RecordBuilder(["007", "James", "Bond"], [xid, xfirst, xlast]);
        builder.GetRange(0, 3);
        Assert.Equal(3, builder.Count);
        Assert.Equal("007", builder[0].value); Assert.Equal("[Emp].[Id]", builder[0].entry.Identifier.Value);
        Assert.Equal("James", builder[1].value); Assert.Equal("[Emp].[First]", builder[1].entry.Identifier.Value);
        Assert.Equal("Bond", builder[2].value); Assert.Equal("[Last]", builder[2].entry.Identifier.Value);

        builder.GetRange(1, 2);
        Assert.Equal(2, builder.Count);
        Assert.Equal("James", builder[0].value); Assert.Equal("[Emp].[First]", builder[0].entry.Identifier.Value);
        Assert.Equal("Bond", builder[1].value); Assert.Equal("[Last]", builder[1].entry.Identifier.Value);

        try { builder.GetRange(-1, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { builder.GetRange(3, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { builder.GetRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { builder.GetRange(0, 4); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Value()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Emp.First");
        var xlast = new SchemaEntry(engine, "Last");

        var builder = new RecordBuilder(["007", "James", "Bond"], [xid, xfirst, xlast]);
        builder.ReplaceValue(0, null);
        Assert.Equal(3, builder.Count);
        Assert.Null(builder[0].value); Assert.Equal("[Emp].[Id]", builder[0].entry.Identifier.Value);

        try { builder.ReplaceValue(-1, null); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { builder.ReplaceValue(3, null); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Entry()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Emp.First");
        var xlast = new SchemaEntry(engine, "Last");
        var builder = new RecordBuilder(["007", "James", "Bond"], [xid, xfirst, xlast]);

        var xother = new SchemaEntry(engine, "other");
        builder.ReplaceEntry(0, xother);
        Assert.Equal(3, builder.Count);
        Assert.Equal("007", builder[0].value); Assert.Equal("[other]", builder[0].entry.Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Emp.First");
        var builder = new RecordBuilder(["007", "James"], [xid, xfirst]);

        var xlast = new SchemaEntry(engine, "Last");
        builder.Add("Bond", xlast);
        Assert.Equal(3, builder.Count);
        Assert.Equal("007", builder[0].value); Assert.Equal("[Emp].[Id]", builder[0].entry.Identifier.Value);
        Assert.Equal("James", builder[1].value); Assert.Equal("[Emp].[First]", builder[1].entry.Identifier.Value);
        Assert.Equal("Bond", builder[2].value); Assert.Equal("[Last]", builder[2].entry.Identifier.Value);

        try { builder.Add(null, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);        
        var builder = new RecordBuilder("007", xid);

        var xfirst = new SchemaEntry(engine, "Emp.First");
        var xlast = new SchemaEntry(engine, "Last");
        builder.AddRange(["James", "Bond"], [xfirst, xlast]);
        Assert.Equal(3, builder.Count);
        Assert.Equal("007", builder[0].value); Assert.Equal("[Emp].[Id]", builder[0].entry.Identifier.Value);
        Assert.Equal("James", builder[1].value); Assert.Equal("[Emp].[First]", builder[1].entry.Identifier.Value);
        Assert.Equal("Bond", builder[2].value); Assert.Equal("[Last]", builder[2].entry.Identifier.Value);

        try { builder.AddRange(null!, [xfirst]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { builder.AddRange(["007"], null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Emp.First");
        var builder = new RecordBuilder(["007", "James"], [xid, xfirst]);

        var xlast = new SchemaEntry(engine, "Last");
        builder.Insert(2, "Bond", xlast);
        Assert.Equal(3, builder.Count);
        Assert.Equal("007", builder[0].value); Assert.Equal("[Emp].[Id]", builder[0].entry.Identifier.Value);
        Assert.Equal("James", builder[1].value); Assert.Equal("[Emp].[First]", builder[1].entry.Identifier.Value);
        Assert.Equal("Bond", builder[2].value); Assert.Equal("[Last]", builder[2].entry.Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
        var builder = new RecordBuilder("007", xid);

        var xfirst = new SchemaEntry(engine, "Emp.First");
        var xlast = new SchemaEntry(engine, "Last");
        builder.InsertRange(1, ["James", "Bond"], [xfirst, xlast]);
        Assert.Equal(3, builder.Count);
        Assert.Equal("007", builder[0].value); Assert.Equal("[Emp].[Id]", builder[0].entry.Identifier.Value);
        Assert.Equal("James", builder[1].value); Assert.Equal("[Emp].[First]", builder[1].entry.Identifier.Value);
        Assert.Equal("Bond", builder[2].value); Assert.Equal("[Last]", builder[2].entry.Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Emp.First");
        var xlast = new SchemaEntry(engine, "Last");
        var builder = new RecordBuilder(["007", "James", "Bond"], [xid, xfirst, xlast]);

        builder.RemoveAt(0);
        Assert.Equal(2, builder.Count);
        Assert.Equal("James", builder[0].value); Assert.Equal("[Emp].[First]", builder[0].entry.Identifier.Value);
        Assert.Equal("Bond", builder[1].value); Assert.Equal("[Last]", builder[1].entry.Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Emp.First");
        var xlast = new SchemaEntry(engine, "Last");
        var builder = new RecordBuilder(["007", "James", "Bond"], [xid, xfirst, xlast]);

        builder.RemoveRange(1, 0);
        Assert.Equal(3, builder.Count);
        Assert.Equal("007", builder[0].value); Assert.Equal("[Emp].[Id]", builder[0].entry.Identifier.Value);
        Assert.Equal("James", builder[1].value); Assert.Equal("[Emp].[First]", builder[1].entry.Identifier.Value);
        Assert.Equal("Bond", builder[2].value); Assert.Equal("[Last]", builder[2].entry.Identifier.Value);

        builder.RemoveRange(1, 2);
        Assert.Equal(1, builder.Count);
        Assert.Equal("007", builder[0].value); Assert.Equal("[Emp].[Id]", builder[0].entry.Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Emp.First");
        var xlast = new SchemaEntry(engine, "Last");
        var builder = new RecordBuilder(["007", "James", "Bond"], [xid, xfirst, xlast]);

        builder.Clear();
        var record = builder.ToRecord(out var schema);
        Assert.Empty(record);
        Assert.Empty(schema);
    }
}