#pragma warning disable IDE0028

using Record = Yotei.ORM.Records.Code.Record;
using Entry = Yotei.ORM.Records.Code.SchemaEntry;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_Record
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empy()
    {
        var record = new Record();
        Assert.Empty(record);
        Assert.Null(record.Schema);

        var engine = new FakeEngine();
        record = new Record(engine);
        Assert.Empty(record);
        Assert.Empty(record.Schema!);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Ranges_Empty()
    {
        var record = new Record(new FakeEngine(), [], []);
        Assert.Empty(record);
        Assert.Empty(record.Schema!);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_SchemaLess()
    {
        var record = CreateSchemaLess();
        Assert.Equal(3, record.Count);
        Assert.Null(record.Schema);
        Assert.Equal("007", record[0]);
        Assert.Equal("James", record[1]);
        Assert.Equal("Bond", record[2]);
    }
    static IRecord CreateSchemaLess() => new Record(["007", "James", "Bond"]);

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_SchemaFull()
    {
        var record = CreateSchemaFull();
        Assert.Equal(3, record.Count);
        Assert.NotNull(record.Schema);
        Assert.Equal("007", record[0]); Assert.Equal("[dbo].[Employees].[Id]", record.Schema![0].Identifier.Value);
        Assert.Equal("James", record[1]); Assert.Equal("[Employees].[FirstName]", record.Schema![1].Identifier.Value);
        Assert.Equal("Bond", record[2]); Assert.Equal("[LastName]", record.Schema![2].Identifier.Value);
    }
    static IRecord CreateSchemaFull()
    {
        var engine = new FakeEngine();
        var builder = new Record.Builder(engine);
        builder.Add("007", new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true));
        builder.Add("James", new Entry(engine, "Employees.FirstName"));
        builder.Add("Bond", new Entry(engine, "LastName"));
        return builder.CreateInstance();
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Equal_SchemaLess()
    {
        var source = CreateSchemaLess();
        var target = CreateSchemaLess();
        Assert.True(source.Equals(target));
        Assert.True(target.Equals(source));

        target = new Record([1, "other"]);
        Assert.False(source.Equals(target));
        Assert.False(target.Equals(source));
    }

    //[Enforced]
    [Fact]
    public static void Test_Equal_SchemaFull()
    {
        var source = CreateSchemaFull();
        var target = CreateSchemaFull();
        Assert.True(source.Equals(target));
        Assert.True(target.Equals(source));

        var schema = source.Schema!;
        var engine = schema.Engine;

        target = new Record(engine, ["007", "James", "Bond"], schema);
        Assert.True(source.Equals(target));
        Assert.True(target.Equals(source));

        target = new Record(engine, ["008", "James", "Bond"], schema);
        Assert.False(source.Equals(target));
        Assert.False(target.Equals(source));

        target = new Record(engine, ["007", "James"], [schema![0], schema[1]]);
        Assert.False(source.Equals(target));
        Assert.False(target.Equals(source));

        schema = (Schema)schema.Replace(0, new Entry(schema.Engine, "Any"));
        target = new Record(engine, ["007", "James", "Bond"], schema);
        Assert.False(source.Equals(target));
        Assert.False(target.Equals(source));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone_SchemaLess()
    {
        var source = CreateSchemaLess();
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Null(target.Schema);
        Assert.Equal("007", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone_SchemaFull()
    {
        var source = CreateSchemaFull();
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.NotNull(target.Schema);
        Assert.Equal(source.Schema, target.Schema);
        Assert.Equal("007", target[0]); Assert.Equal("[dbo].[Employees].[Id]", target.Schema![0].Identifier.Value);
        Assert.Equal("James", target[1]); Assert.Equal("[Employees].[FirstName]", target.Schema![1].Identifier.Value);
        Assert.Equal("Bond", target[2]); Assert.Equal("[LastName]", target.Schema![2].Identifier.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_WithSchema_SchemaLess()
    {
        var source = CreateSchemaLess();

        var target = source.WithSchema(null);
        Assert.NotSame(source, target);
        Assert.Null(target.Schema);

        var schema = CreateSchemaFull().Schema!;
        target = source.WithSchema(schema);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.NotNull(target.Schema);
        Assert.Equal("007", target[0]); Assert.Equal("[dbo].[Employees].[Id]", target.Schema![0].Identifier.Value);
        Assert.Equal("James", target[1]); Assert.Equal("[Employees].[FirstName]", target.Schema![1].Identifier.Value);
        Assert.Equal("Bond", target[2]); Assert.Equal("[LastName]", target.Schema![2].Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_WithSchema_SchemaFull()
    {
        var source = CreateSchemaFull();

        var target = source.WithSchema(null);
        Assert.NotSame(source, target);
        Assert.Null(target.Schema);

        var schema = source.Schema!.Replace(0, new Entry(source.Schema.Engine, "Any"));
        target = source.WithSchema(schema);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.NotNull(target.Schema);
        Assert.Equal("007", target[0]); Assert.Equal("[Any]", target.Schema![0].Identifier.Value);
        Assert.Equal("James", target[1]); Assert.Equal("[Employees].[FirstName]", target.Schema![1].Identifier.Value);
        Assert.Equal("Bond", target[2]); Assert.Equal("[LastName]", target.Schema![2].Identifier.Value);

        schema = schema.RemoveAt(0);
        try { _ = source.WithSchema(schema); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_TryGet()
    {
        var source = CreateSchemaFull();
        var found = source.TryGet("LASTNAME", out var value, out var entry);
        Assert.True(found);
        Assert.Equal("Bond", value);
        Assert.Equal("[LastName]", entry!.Identifier.Value);

        source = (Record)source.WithSchema(null);
        try { source.TryGet("Any", out _, out _); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetRange_SchemaLess()
    {
        var source = CreateSchemaLess();
        var target = source.GetRange(0, 3);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("James", target[0]);
        Assert.Equal("Bond", target[1]);

        try { source.GetRange(-1, 2); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { source.GetRange(0, 99); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange_SchemaFull()
    {
        var source = CreateSchemaFull();
        var target = source.GetRange(0, 3);
        Assert.Same(source, target);
        Assert.Same(source.Schema, target.Schema);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("James", target[0]); Assert.Same(source.Schema![1], target.Schema![0]);
        Assert.Equal("Bond", target[1]); Assert.Same(source.Schema![2], target.Schema![1]);

        try { source.GetRange(-1, 2); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { source.GetRange(0, 99); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace_SchemaLess()
    {
        var source = CreateSchemaLess();
        var target = source.Replace(0, "007");
        Assert.Same(source, target);

        target = source.Replace(0, "008");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("008", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);

        try { source.Replace(-1, null); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_SchemaFull()
    {
        var source = CreateSchemaFull();
        var target = source.Replace(0, "007");
        Assert.Same(source, target);
        Assert.Same(source.Schema, target.Schema);

        target = source.Replace(0, "008");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("008", target[0]); Assert.Same(source.Schema![0], target.Schema![0]);
        Assert.Equal("James", target[1]); Assert.Same(source.Schema![1], target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(source.Schema![2], target.Schema![2]);

        target = source.Replace(0, "008", new Entry(source.Schema.Engine, "Any"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("008", target[0]); Assert.Equal("[Any]", target.Schema![0].Identifier.Value);
        Assert.Equal("James", target[1]); Assert.Same(source.Schema![1], target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(source.Schema![2], target.Schema![2]);

        try { source.Replace(0, "008", null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_SchemaLess()
    {
        var source = CreateSchemaLess();

        var target = source.Add("UK");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("007", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);
        Assert.Equal("UK", target[3]);

        var engine = new FakeEngine();
        try { target = source.Add("UK", new Entry(engine, "Country")); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_SchemaFull()
    {
        var source = CreateSchemaFull();
        var schema = source.Schema!;
        var engine = schema.Engine;

        var target = source.Add("UK", new Entry(engine, "Country"));
        Assert.NotSame(source, target);
        Assert.NotNull(target.Schema);
        Assert.Equal(4, target.Count);
        Assert.Equal("007", target[0]); Assert.Equal(source.Schema![0], target.Schema![0]);
        Assert.Equal("James", target[1]); Assert.Same(source.Schema![1], target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(source.Schema![2], target.Schema![2]);
        Assert.Equal("UK", target[3]); Assert.Equal("[Country]", target.Schema![3].Identifier.Value);

        try { target = source.Add("UK"); Assert.Fail(); }
        catch (InvalidOperationException) { }

        try { target = source.Add("UK", null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddRange_SchemaLess()
    {
        var source = CreateSchemaLess();

        var target = source.AddRange(["UK", 50]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("007", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);
        Assert.Equal("UK", target[3]);
        Assert.Equal(50, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_SchemaFull()
    {
        var source = CreateSchemaFull();
        var schema = source.Schema!;
        var engine = schema.Engine;

        Entry[] entries = [new Entry(engine, "Country"), new Entry(engine, "Age")];
        var target = source.AddRange(["UK", 50], entries);
        Assert.NotSame(source, target);
        Assert.NotNull(target.Schema);
        Assert.Equal(5, target.Count);
        Assert.Equal("007", target[0]); Assert.Equal(source.Schema![0], target.Schema![0]);
        Assert.Equal("James", target[1]); Assert.Same(source.Schema![1], target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(source.Schema![2], target.Schema![2]);
        Assert.Equal("UK", target[3]); Assert.Equal("[Country]", target.Schema![3].Identifier.Value);
        Assert.Equal(50, target[4]); Assert.Equal("[Age]", target.Schema![4].Identifier.Value);

        entries = [new Entry(engine, "Country")];
        try { source.AddRange(["UK", 50], entries); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert_SchemaLess()
    {
        var source = CreateSchemaLess();

        var target = source.Insert(3, "UK");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("007", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);
        Assert.Equal("UK", target[3]);

        var engine = new FakeEngine();
        try { target = source.Insert(3, "UK", new Entry(engine, "Country")); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_SchemaFull()
    {
        var source = CreateSchemaFull();
        var schema = source.Schema!;
        var engine = schema.Engine;

        var target = source.Insert(3, "UK", new Entry(engine, "Country"));
        Assert.NotSame(source, target);
        Assert.NotNull(target.Schema);
        Assert.Equal(4, target.Count);
        Assert.Equal("007", target[0]); Assert.Equal(source.Schema![0], target.Schema![0]);
        Assert.Equal("James", target[1]); Assert.Same(source.Schema![1], target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(source.Schema![2], target.Schema![2]);
        Assert.Equal("UK", target[3]); Assert.Equal("[Country]", target.Schema![3].Identifier.Value);

        try { target = source.Insert(3, "UK"); Assert.Fail(); }
        catch (InvalidOperationException) { }

        try { target = source.Insert(3, "UK", null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_SchemaLess()
    {
        var source = CreateSchemaLess();

        var target = source.InsertRange(3, ["UK", 50]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("007", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);
        Assert.Equal("UK", target[3]);
        Assert.Equal(50, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_SchemaFull()
    {
        var source = CreateSchemaFull();
        var schema = source.Schema!;
        var engine = schema.Engine;

        Entry[] entries = [new Entry(engine, "Country"), new Entry(engine, "Age")];
        var target = source.InsertRange(3, ["UK", 50], entries);
        Assert.NotSame(source, target);
        Assert.NotNull(target.Schema);
        Assert.Equal(5, target.Count);
        Assert.Equal("007", target[0]); Assert.Equal(source.Schema![0], target.Schema![0]);
        Assert.Equal("James", target[1]); Assert.Same(source.Schema![1], target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(source.Schema![2], target.Schema![2]);
        Assert.Equal("UK", target[3]); Assert.Equal("[Country]", target.Schema![3].Identifier.Value);
        Assert.Equal(50, target[4]); Assert.Equal("[Age]", target.Schema![4].Identifier.Value);

        entries = [new Entry(engine, "Country")];
        try { source.InsertRange(3, ["UK", 50], entries); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt_SchemaLess()
    {
        var source = CreateSchemaLess();
        var target = source.RemoveAt(0);

        Assert.Equal(2, target.Count);
        Assert.Null(target.Schema);
        Assert.Equal("James", target[0]);
        Assert.Equal("Bond", target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt_SchemaFull()
    {
        var source = CreateSchemaFull();
        var target = source.RemoveAt(0);

        Assert.Equal(2, target.Count);
        Assert.Equal("James", target[0]); Assert.Equal("[Employees].[FirstName]", target.Schema![0].Identifier.Value);
        Assert.Equal("Bond", target[1]); Assert.Equal("[LastName]", target.Schema![1].Identifier.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange_SchemaLess()
    {
        var source = CreateSchemaLess();
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 3);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        target = source.RemoveRange(0, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("Bond", target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange_SchemaFull()
    {
        var source = CreateSchemaFull();
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 3);
        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Empty(target.Schema!);

        target = source.RemoveRange(0, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("Bond", target[0]); Assert.Equal("[LastName]", target.Schema![0].Identifier.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveIdentifier()
    {
        var source = CreateSchemaFull();
        var target = source.Remove("Any");
        Assert.Same(source, target);

        target = source.Remove("DBO.EMPLOYEES.ID");
        Assert.Equal(2, target.Count);
        Assert.Equal(2, target.Schema!.Count);
        Assert.Equal("James", target[0]); Assert.Equal("[Employees].[FirstName]", target.Schema![0].Identifier.Value);
        Assert.Equal("Bond", target[1]); Assert.Equal("[LastName]", target.Schema![1].Identifier.Value);

        source = CreateSchemaLess();
        try { source.Remove("Any"); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear_SchemaLess()
    {
        var source = new Record();
        var target = source.Clear();
        Assert.Same(source, target);

        source = (Record)CreateSchemaLess();
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear_SchemaFull()
    {
        var source = new Record(new FakeEngine());
        var target = source.Clear();
        Assert.Same(source, target);
        Assert.NotNull(target.Schema);
        Assert.Same(source.Schema!, target.Schema!);

        source = (Record)CreateSchemaFull();
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Empty(target.Schema!);
    }
}