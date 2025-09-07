using Record = Yotei.ORM.Records.Code.Record;

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
        var record = CreateLess();
        Assert.Equal(3, record.Count);
        Assert.Null(record.Schema);
        Assert.Equal("007", record[0]);
        Assert.Equal("James", record[1]);
        Assert.Equal("Bond", record[2]);
    }
    static IRecord CreateLess() => new Record(["007", "James", "Bond"]);

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_SchemaFull()
    {
        var record = CreateFull();
        Assert.Equal(3, record.Count);
        Assert.NotNull(record.Schema);
        Assert.Equal("007", record[0]); Assert.Equal("[dbo].[Employees].[Id]", record.Schema![0].Identifier.Value);
        Assert.Equal("James", record[1]); Assert.Equal("[Employees].[FirstName]", record.Schema![1].Identifier.Value);
        Assert.Equal("Bond", record[2]); Assert.Equal("[LastName]", record.Schema![2].Identifier.Value);
    }
    static IRecord CreateFull()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var schema = new Schema(engine, [xid, xfirst, xlast]);

        return new Record(schema, ["007", "James", "Bond"]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Equal_SchemaLess()
    {
        var source = CreateLess();
        var target = CreateLess();
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
        var source = CreateFull();
        var target = CreateFull();
        Assert.True(source.Equals(target));
        Assert.True(target.Equals(source));

        var schema = source.Schema;
        target = new Record(schema!.Clone(), ["007", "James", "Bond"]);
        Assert.True(source.Equals(target));
        Assert.True(target.Equals(source));

        target = new Record(schema, ["008", "James", "Bond"]);
        Assert.False(source.Equals(target));
        Assert.False(target.Equals(source));

        target = new Record(new Schema(schema.Engine, [schema[0], schema[1]]), ["007", "James"]);
        Assert.False(source.Equals(target));
        Assert.False(target.Equals(source));

        schema = (Schema)schema.Replace(0, new SchemaEntry(schema.Engine, "Any"));
        target = new Record(schema, ["007", "James", "Bond"]);
        Assert.False(source.Equals(target));
        Assert.False(target.Equals(source));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone_SchemaLess()
    {
        var source = CreateLess();
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
        var source = CreateFull();
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.NotNull(target.Schema);
        Assert.Same(source.Schema, target.Schema);
        Assert.Equal("007", target[0]); Assert.Equal("[dbo].[Employees].[Id]", target.Schema![0].Identifier.Value);
        Assert.Equal("James", target[1]); Assert.Equal("[Employees].[FirstName]", target.Schema![1].Identifier.Value);
        Assert.Equal("Bond", target[2]); Assert.Equal("[LastName]", target.Schema![2].Identifier.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_WithSchema_SchemaLess()
    {
        var source = CreateLess();
        var target = source.WithSchema(null);
        Assert.NotSame(source, target);
        Assert.Null(target.Schema);

        var schema = CreateFull().Schema!;
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
        var source = CreateFull();
        var target = source.WithSchema(null);
        Assert.NotSame(source, target);
        Assert.Null(target.Schema);

        var schema = source.Schema!.Replace(0, new SchemaEntry(source.Schema.Engine, "Any"));
        target = source.WithSchema(schema);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.NotNull(target.Schema);
        Assert.Equal("007", target[0]); Assert.Equal("[Any]", target.Schema![0].Identifier.Value);
        Assert.Equal("James", target[1]); Assert.Equal("[Employees].[FirstName]", target.Schema![1].Identifier.Value);
        Assert.Equal("Bond", target[2]); Assert.Equal("[LastName]", target.Schema![2].Identifier.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_TryGet()
    {
        var source = CreateFull();
        var found = source.TryGet("LASTNAME", out var value);
        Assert.True(found);
        Assert.Equal("Bond", value);

        source = (Record)source.WithSchema(null);
        try { source.TryGet("Any", out value); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetRange_SchemaLess()
    {
        var source = CreateLess();
        var target = source.GetRange(0, 3);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("James", target[0]);
        Assert.Equal("Bond", target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange_SchemaFull()
    {
        var source = CreateFull();
        var target = source.GetRange(0, 3);
        Assert.Same(source, target);
        Assert.Same(source.Schema, target.Schema);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("James", target[0]); Assert.Same(source.Schema![1], target.Schema![0]);
        Assert.Equal("Bond", target[1]); Assert.Same(source.Schema![2], target.Schema![1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace_SchemaLess()
    {
        var source = CreateLess();
        var target = source.Replace(0, "007");
        Assert.Same(source, target);

        target = source.Replace(0, "008");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("008", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_SchemaFull()
    {
        var source = CreateFull();
        var target = source.Replace(0, "007");
        Assert.Same(source, target);
        Assert.Same(source.Schema, target.Schema);

        target = source.Replace(0, "008");
        Assert.NotSame(source, target);
        Assert.Same(source.Schema, target.Schema);
        Assert.Equal(3, target.Count);
        Assert.Equal("008", target[0]); Assert.Same(source.Schema![0], target.Schema![0]);
        Assert.Equal("James", target[1]); Assert.Same(source.Schema![1], target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(source.Schema![2], target.Schema![2]);

        target = source.Replace(0, "008", null!);
        Assert.NotSame(source, target);
        Assert.Null(target.Schema);
        Assert.Equal(3, target.Count);
        Assert.Equal("008", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);

        var schema = source.Schema!.Replace(0, new SchemaEntry(source.Schema.Engine, "Any"));
        target = source.Replace(0, "008", schema);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("008", target[0]); Assert.Equal("[Any]", target.Schema![0].Identifier.Value);
        Assert.Equal("James", target[1]); Assert.Same(source.Schema![1], target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(source.Schema![2], target.Schema![2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_SchemaLess()
    {
        var source = CreateLess();

        var target = source.Add("UK");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("007", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);
        Assert.Equal("UK", target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_SchemaFull()
    {
        var source = CreateFull();
        var schema = source.Schema!;
        var engine = schema.Engine;

        var target = source.Add("UK", schema.Add(new SchemaEntry(engine, "Country")));
        Assert.NotSame(source, target);
        Assert.NotNull(target.Schema);
        Assert.Equal(4, target.Count);
        Assert.Equal("007", target[0]); Assert.Equal(source.Schema![0], target.Schema![0]);
        Assert.Equal("James", target[1]); Assert.Same(source.Schema![1], target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(source.Schema![2], target.Schema![2]);
        Assert.Equal("UK", target[3]); Assert.Equal("[Country]", target.Schema![3].Identifier.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_AddRange_SchemaLess() => throw null;

    //[Enforced]
    //[Fact]
    //public static void Test_AddRange_SchemaFull() => throw null;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert_SchemaLess()
    {
        var source = CreateLess();

        var target = source.Insert(3, "UK");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("007", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);
        Assert.Equal("UK", target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_SchemaFull()
    {
        var source = CreateFull();
        var schema = source.Schema!;
        var engine = schema.Engine;

        var target = source.Insert(3, "UK", schema.Insert(3, new SchemaEntry(engine, "Country")));
        Assert.NotSame(source, target);
        Assert.NotNull(target.Schema);
        Assert.Equal(4, target.Count);
        Assert.Equal("007", target[0]); Assert.Equal(source.Schema![0], target.Schema![0]);
        Assert.Equal("James", target[1]); Assert.Same(source.Schema![1], target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(source.Schema![2], target.Schema![2]);
        Assert.Equal("UK", target[3]); Assert.Equal("[Country]", target.Schema![3].Identifier.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_InsertRange_SchemaLess() => throw null;

    //[Enforced]
    //[Fact]
    //public static void Test_InsertRange_SchemaFull() => throw null;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt_SchemaLess()
    {
        var source = CreateLess();
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
        var source = CreateFull();
        var target = source.RemoveAt(0);

        Assert.Equal(2, target.Count);
        Assert.Equal("James", target[0]); Assert.Equal("[Employees].[FirstName]", target.Schema![0].Identifier.Value);
        Assert.Equal("Bond", target[1]); Assert.Equal("[LastName]", target.Schema![1].Identifier.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_RemoveRange_SchemaLess() => throw null;

    //[Enforced]
    //[Fact]
    //public static void Test_RemoveRange_SchemaFull() => throw null;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveIdentifier()
    {
        var source = CreateFull();
        var target = source.Remove("Any");
        Assert.Same(source, target);

        target = source.Remove("DBO.EMPLOYEES.ID");
        Assert.Equal(2, target.Count);
        Assert.Equal(2, target.Schema!.Count);
        Assert.Equal("James", target[0]); Assert.Equal("[Employees].[FirstName]", target.Schema![0].Identifier.Value);
        Assert.Equal("Bond", target[1]); Assert.Equal("[LastName]", target.Schema![1].Identifier.Value);

        source = CreateLess();
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

        source = (Record)CreateLess();
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

        source = (Record)CreateFull();
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Empty(target.Schema!);
    }
}