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

        var schema = source.Schema;
        target = new Record(["007", "James", "Bond"], schema!);
        Assert.True(source.Equals(target));
        Assert.True(target.Equals(source));

        target = new Record(["008", "James", "Bond"], schema!);
        Assert.False(source.Equals(target));
        Assert.False(target.Equals(source));

        target = new Record(["007", "James"], [schema![0], schema[1]]);
        Assert.False(source.Equals(target));
        Assert.False(target.Equals(source));

        schema = (Schema)schema.Replace(0, new Entry(schema.Engine, "Any"));
        target = new Record(["007", "James", "Bond"], schema);
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
}