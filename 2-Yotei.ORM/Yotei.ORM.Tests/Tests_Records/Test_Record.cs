using Record = Yotei.ORM.Records.Code.Record;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Record
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var record = new Record(engine);
        Assert.Empty(record);
        Assert.Empty(record.Schema);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Schema()
    {
        var engine = new FakeEngine();
        var xId = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xFirst = new SchemaEntry(engine, "Employees.FirstName");
        var xLast = new SchemaEntry(engine, "LastName");
        var xCtry = new SchemaEntry(engine, "Countries.Id");
        var schema = new Schema(engine, [xId, xFirst, xLast, xCtry]);
        var record = new Record(schema);

        Assert.Equal(4, record.Count);
        Assert.Null(record[0]);
        Assert.Null(record[1]);
        Assert.Null(record[2]);
        Assert.Null(record[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Values()
    {
        var values = new object?[] { "007", "James", "Bond", "UK" };
        var engine = new FakeEngine();
        var record = new Record(engine, values);

        Assert.Equal(4, record.Count);
        Assert.Equal("007", record[0]); Assert.Equal("[#0]", record.Schema[0].Identifier.Value);
        Assert.Equal("James", record[1]); Assert.Equal("[#1]", record.Schema[1].Identifier.Value);
        Assert.Equal("Bond", record[2]); Assert.Equal("[#2]", record.Schema[2].Identifier.Value);
        Assert.Equal("UK", record[3]); Assert.Equal("[#3]", record.Schema[3].Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Regular()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);
        builder.Add("007", x => x.dbo.Employees.Id, isPrimaryKey: true);
        builder.Add("James", x => x.Employees.FirstName);
        builder.Add("Bond", x => x.LastName);
        builder.Add("UK", x => x.Countries.Id);
        var record = builder.Create();

        Assert.Equal(4, record.Count);
        Assert.Equal("007", record[0]); Assert.Equal("[dbo].[Employees].[Id]", record.Schema[0].Identifier.Value);
        Assert.Equal("James", record[1]); Assert.Equal("[Employees].[FirstName]", record.Schema[1].Identifier.Value);
        Assert.Equal("Bond", record[2]); Assert.Equal("[LastName]", record.Schema[2].Identifier.Value);
        Assert.Equal("UK", record[3]); Assert.Equal("[Countries].[Id]", record.Schema[3].Identifier.Value);

        try { _ = new Record((ISchema)null!, []); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Record(record.Schema, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        var values = new object?[] { "007", "James", "Bond" };
        try { _ = new Record(record.Schema, values); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);
        builder.Add("007", x => x.dbo.Employees.Id, isPrimaryKey: true);
        builder.Add("James", x => x.Employees.FirstName);
        builder.Add("Bond", x => x.LastName);
        builder.Add("UK", x => x.Countries.Id);
        var source = builder.Create();

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(source.Schema, target.Schema);
        Assert.Equal(source[0], target[0]);
        Assert.Equal(source[1], target[1]);
        Assert.Equal(source[2], target[2]);
        Assert.Equal(source[3], target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);
        builder.Add("007", x => x.dbo.Employees.Id, isPrimaryKey: true);
        builder.Add("James", x => x.Employees.FirstName);
        builder.Add("Bond", x => x.LastName);
        builder.Add("UK", x => x.Countries.Id);
        var record = builder.Create();

        var list = record["Id"].ToList();
        Assert.Equal(2, list.Count);
        Assert.Equal("007", list[0].Key); Assert.Equal("[dbo].[Employees].[Id]", list[0].Value.Identifier.Value);
        Assert.Equal("UK", list[1].Key); Assert.Equal("[Countries].[Id]", list[1].Value.Identifier.Value);
        list = record[x => x.Employees.x].ToList();
        Assert.Equal("007", list[0].Key); Assert.Equal("[dbo].[Employees].[Id]", list[0].Value.Identifier.Value);
        Assert.Equal("James", list[1].Key); Assert.Equal("[Employees].[FirstName]", list[1].Value.Identifier.Value);

        builder.Clear();
        builder.Add("007", x => x.dbo.Employees.Id, isPrimaryKey: true);
        builder.Add("008", builder.Schema[0]);
        record = builder.Create();
        list = record[x => x.Employees.Id].ToList();
        Assert.Equal("007", list[0].Key); Assert.Equal("[dbo].[Employees].[Id]", list[0].Value.Identifier.Value);
        Assert.Equal("008", list[1].Key); Assert.Equal("[dbo].[Employees].[Id]", list[1].Value.Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Unique()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);
        builder.Add("007", x => x.dbo.Employees.Id, isPrimaryKey: true);
        builder.Add("James", x => x.Employees.FirstName);
        builder.Add("Bond", x => x.LastName);
        builder.Add("UK", x => x.Countries.Id);
        var record = builder.Create();

        Assert.False(record.Unique(x => x.Id, out var value, out var entry));
        Assert.Null(value);
        Assert.Null(entry);

        Assert.True(record.Unique(x => x.LastName, out value, out entry));
        Assert.Equal("Bond", value);
        Assert.Equal("[LastName]", entry.Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Schema()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);
        builder.Add("007", x => x.dbo.Employees.Id, isPrimaryKey: true);
        builder.Add("James", x => x.Employees.FirstName);
        builder.Add("Bond", x => x.LastName);
        builder.Add("UK", x => x.Countries.Id);
        var source = builder.Create();

        engine = new FakeEngine();
        var meta = new Schema(engine, [
            new SchemaEntry(engine, "Alpha"),
            new SchemaEntry(engine, "Beta"),
            new SchemaEntry(engine, "Delta"),
            new SchemaEntry(engine, "Gamma"),
        ]);
        var target = source.WithSchema(meta);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.NotSame(source.Schema, target.Schema);
        Assert.Equal(source[0], target[0]); Assert.Equal("[Alpha]", target.Schema[0].Identifier.Value);
        Assert.Equal(source[1], target[1]); Assert.Equal("[Beta]", target.Schema[1].Identifier.Value);
        Assert.Equal(source[2], target[2]); Assert.Equal("[Delta]", target.Schema[2].Identifier.Value);
        Assert.Equal(source[3], target[3]); Assert.Equal("[Gamma]", target.Schema[3].Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);
        builder.Add("007", x => x.dbo.Employees.Id, isPrimaryKey: true);
        builder.Add("James", x => x.Employees.FirstName);
        builder.Add("Bond", x => x.LastName);
        builder.Add("UK", x => x.Countries.Id);
        var source = builder.Create();

        var target = source.GetRange(0, source.Count);
        Assert.Same(source, target);

        target = source.GetRange(0, 0);
        Assert.Empty(target);

        target = source.GetRange(1, 2);
        Assert.Equal("James", target[0]); Assert.Equal("[Employees].[FirstName]", target.Schema[0].Identifier.Value);
        Assert.Equal("Bond", target[1]); Assert.Equal("[LastName]", target.Schema[1].Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Value()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);
        builder.Add("007", x => x.dbo.Employees.Id, isPrimaryKey: true);
        builder.Add("James", x => x.Employees.FirstName);
        builder.Add("Bond", x => x.LastName);
        builder.Add("UK", x => x.Countries.Id);
        var source = builder.Create();

        var target = source.ReplaceValue(0, "007");
        Assert.Same(source, target);

        target = source.ReplaceValue(0, "008");
        Assert.NotSame(source, target);
        Assert.Same(source.Schema, target.Schema);
        Assert.Equal("008", target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Metadata()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);
        builder.Add("007", x => x.dbo.Employees.Id, isPrimaryKey: true);
        builder.Add("James", x => x.Employees.FirstName);
        builder.Add("Bond", x => x.LastName);
        builder.Add("UK", x => x.Countries.Id);
        var source = builder.Create();

        var target = source.ReplaceMetadata(0, source.Schema[0]);
        Assert.Same(source, target);

        target = source.ReplaceMetadata(0, new SchemaEntry(builder.Engine, "alpha"));
        Assert.NotSame(source, target);
        Assert.Equal("[alpha]", target.Schema[0].Identifier.Value);
        Assert.Equal(source[0], target[0]);

        try { source.ReplaceMetadata(0, new SchemaEntry(new FakeEngine(), "any")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);
        builder.Add("007", x => x.dbo.Employees.Id, isPrimaryKey: true);
        builder.Add("James", x => x.Employees.FirstName);
        builder.Add("Bond", x => x.LastName);
        builder.Add("UK", x => x.Countries.Id);
        var source = builder.Create();

        var target = source.Replace(0, "008", new SchemaEntry(builder.Engine, "alpha"));
        Assert.NotSame(source, target);
        Assert.Equal("008", target[0]);
        Assert.Equal("[alpha]", target.Schema[0].Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);
        builder.Add("007", x => x.dbo.Employees.Id, isPrimaryKey: true);
        builder.Add("James", x => x.Employees.FirstName);
        var source = builder.Create();

        var target = source.Add("Bond", new SchemaEntry(engine, "LastName"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("007", target[0]); Assert.Equal("[dbo].[Employees].[Id]", target.Schema[0].Identifier.Value);
        Assert.Equal("James", target[1]); Assert.Equal("[Employees].[FirstName]", target.Schema[1].Identifier.Value);
        Assert.Equal("Bond", target[2]); Assert.Equal("[LastName]", target.Schema[2].Identifier.Value);

        try { source.Add("any", null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add("any", new SchemaEntry(engine)); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add("any", new SchemaEntry(engine, "")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add("any", new SchemaEntry(engine, "x.")); Assert.Fail(); }
        catch (ArgumentException) { }

        target = source.Add("008", source.Schema[0]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("007", target[0]); Assert.Equal("[dbo].[Employees].[Id]", target.Schema[0].Identifier.Value);
        Assert.Equal("James", target[1]); Assert.Equal("[Employees].[FirstName]", target.Schema[1].Identifier.Value);
        Assert.Equal("008", target[2]); Assert.Equal("[dbo].[Employees].[Id]", target.Schema[2].Identifier.Value);

        try { source.Add("any", new SchemaEntry(engine, "id")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange() { }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);
        builder.Add("007", x => x.dbo.Employees.Id, isPrimaryKey: true);
        builder.Add("James", x => x.Employees.FirstName);
        var source = builder.Create();

        var target = source.Insert(2, "Bond", new SchemaEntry(engine, "LastName"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("007", target[0]); Assert.Equal("[dbo].[Employees].[Id]", target.Schema[0].Identifier.Value);
        Assert.Equal("James", target[1]); Assert.Equal("[Employees].[FirstName]", target.Schema[1].Identifier.Value);
        Assert.Equal("Bond", target[2]); Assert.Equal("[LastName]", target.Schema[2].Identifier.Value);

        try { source.Insert(2, "any", null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Insert(2, "any", new SchemaEntry(engine)); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Insert(2, "any", new SchemaEntry(engine, "")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Insert(2, "any", new SchemaEntry(engine, "x.")); Assert.Fail(); }
        catch (ArgumentException) { }

        target = source.Insert(2, "008", source.Schema[0]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("007", target[0]); Assert.Equal("[dbo].[Employees].[Id]", target.Schema[0].Identifier.Value);
        Assert.Equal("James", target[1]); Assert.Equal("[Employees].[FirstName]", target.Schema[1].Identifier.Value);
        Assert.Equal("008", target[2]); Assert.Equal("[dbo].[Employees].[Id]", target.Schema[2].Identifier.Value);

        try { source.Insert(2, "any", new SchemaEntry(engine, "id")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange() { }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);
        builder.Add("007", x => x.dbo.Employees.Id, isPrimaryKey: true);
        builder.Add("James", x => x.Employees.FirstName);
        builder.Add("Bond", x => x.LastName);
        builder.Add("UK", x => x.Countries.Id);
        var source = builder.Create();

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("James", target[0]); Assert.Equal("[Employees].[FirstName]", target.Schema[0].Identifier.Value);
        Assert.Equal("Bond", target[1]); Assert.Equal("[LastName]", target.Schema[1].Identifier.Value);
        Assert.Equal("UK", target[2]); Assert.Equal("[Countries].[Id]", target.Schema[2].Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);
        builder.Add("007", x => x.dbo.Employees.Id, isPrimaryKey: true);
        builder.Add("James", x => x.Employees.FirstName);
        builder.Add("Bond", x => x.LastName);
        builder.Add("UK", x => x.Countries.Id);
        var source = builder.Create();

        var target = source.RemoveRange(0, source.Count);
        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Empty(target.Schema);

        target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("Bond", target[0]); Assert.Equal("[LastName]", target.Schema[0].Identifier.Value);
        Assert.Equal("UK", target[1]); Assert.Equal("[Countries].[Id]", target.Schema[1].Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);
        builder.Add("007", x => x.dbo.Employees.Id, isPrimaryKey: true);
        builder.Add("James", x => x.Employees.FirstName);
        builder.Add("Bond", x => x.LastName);
        builder.Add("UK", x => x.Countries.Id);
        var source = builder.Create();

        var target = source.Remove("ID");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("James", target[0]); Assert.Equal("[Employees].[FirstName]", target.Schema[0].Identifier.Value);
        Assert.Equal("Bond", target[1]); Assert.Equal("[LastName]", target.Schema[1].Identifier.Value);
        Assert.Equal("UK", target[2]); Assert.Equal("[Countries].[Id]", target.Schema[2].Identifier.Value);

        target = source.RemoveLast("ID");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("007", target[0]); Assert.Equal("[dbo].[Employees].[Id]", target.Schema[0].Identifier.Value);
        Assert.Equal("James", target[1]); Assert.Equal("[Employees].[FirstName]", target.Schema[1].Identifier.Value);
        Assert.Equal("Bond", target[2]); Assert.Equal("[LastName]", target.Schema[2].Identifier.Value);

        target = source.RemoveAll("ID");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("James", target[0]); Assert.Equal("[Employees].[FirstName]", target.Schema[0].Identifier.Value);
        Assert.Equal("Bond", target[1]); Assert.Equal("[LastName]", target.Schema[1].Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate() { }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);
        builder.Add("007", x => x.dbo.Employees.Id, isPrimaryKey: true);
        builder.Add("James", x => x.Employees.FirstName);
        builder.Add("Bond", x => x.LastName);
        builder.Add("UK", x => x.Countries.Id);
        var source = builder.Create();

        var target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Empty(target.Schema);
    }
}