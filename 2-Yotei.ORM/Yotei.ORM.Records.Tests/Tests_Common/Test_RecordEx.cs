using Meta = Yotei.ORM.Records.Code.SchemaEntry;

namespace Yotei.ORM.Records.Tests;

// ========================================================
//[Enforced]
public static class Test_RecordEx
{
    //[Enforced]
    [Fact]
    public static void Test_No_Changes()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);

        builder.Add("007", new Meta(engine, "Emp.Id", isPrimaryKey: true));
        builder.Add("James", new Meta(engine, "Emp.FirstName"));
        builder.Add("Bond", new Meta(engine, "LastName"));
        builder.Add("UK", new Meta(engine, "Ctry.Id"));
        var source = builder.ToMetaRecord();

        builder.Clear();
        builder.Add("Bond", new Meta(engine, "LastName"));
        builder.Add("James", new Meta(engine, "Emp.FirstName"));
        builder.Add("007", new Meta(engine, "Emp.Id", isPrimaryKey: true));
        builder.Add(50, new Meta(engine, "Age"));
        var target = builder.ToMetaRecord();

        var record = source.CompareTo(target);
        Assert.Null(record);

        record = source.CompareTo(target, orphanSources: true);
        Assert.NotNull(record);
        Assert.Equal(1, record.Record.Count);
        Assert.Equal("UK", record.Record[0]); Assert.Equal("[Ctry].[Id]", record.Schema[0].Identifier.Value);

        record = source.CompareTo(target, orphanTargets: true);
        Assert.NotNull(record);
        Assert.Equal(1, record.Record.Count);
        Assert.Equal(50, record.Record[0]); Assert.Equal("[Age]", record.Schema[0].Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Changes()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);

        builder.Add("007", new Meta(engine, "Emp.Id", isPrimaryKey: true));
        builder.Add("James", new Meta(engine, "Emp.FirstName"));
        builder.Add("Bond", new Meta(engine, "LastName"));
        builder.Add("UK", new Meta(engine, "Ctry.Id"));
        var source = builder.ToMetaRecord();

        builder.Clear();
        builder.Add("Bond", new Meta(engine, "LastName"));
        builder.Add("James", new Meta(engine, "Emp.FirstName"));
        builder.Add("008", new Meta(engine, "Emp.Id", isPrimaryKey: true));
        builder.Add(50, new Meta(engine, "Age"));
        var target = builder.ToMetaRecord();

        var record = source.CompareTo(target);
        Assert.NotNull(record);
        Assert.Equal(1, record.Record.Count);
        Assert.Equal("008", record.Record[0]); Assert.Equal("[Emp].[Id]", record.Schema[0].Identifier.Value);

        record = source.CompareTo(target, orphanSources: true);
        Assert.NotNull(record);
        Assert.Equal(2, record.Record.Count);
        Assert.Equal("008", record.Record[0]); Assert.Equal("[Emp].[Id]", record.Schema[0].Identifier.Value);
        Assert.Equal("UK", record.Record[1]); Assert.Equal("[Ctry].[Id]", record.Schema[1].Identifier.Value);

        record = source.CompareTo(target, orphanTargets: true);
        Assert.NotNull(record);
        Assert.Equal(2, record.Record.Count);
        Assert.Equal("008", record.Record[0]); Assert.Equal("[Emp].[Id]", record.Schema[0].Identifier.Value);
        Assert.Equal(50, record.Record[1]); Assert.Equal("[Age]", record.Schema[1].Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Duplicates_In_Source_Schema()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);

        builder.Add("007", new Meta(engine, "Emp.Id", isPrimaryKey: true));
        builder.Add("James", new Meta(engine, "Emp.FirstName"));
        builder.Add("008", new Meta(engine, "Emp.Id", isPrimaryKey: true));
        var source = builder.ToMetaRecord();

        builder.Clear();
        builder.Add("James", new Meta(engine, "Emp.FirstName"));
        builder.Add("Bond", new Meta(engine, "LastName"));
        builder.Add("007", new Meta(engine, "Emp.Id", isPrimaryKey: true));
        var target = builder.ToMetaRecord();

        var record = source.CompareTo(target);
        Assert.Null(record);

        record = source.CompareTo(target, orphanSources: true);
        Assert.NotNull(record);
        Assert.Equal(1, record.Record.Count);
        Assert.Equal("008", record.Record[0]); Assert.Equal("[Emp].[Id]", record.Schema[0].Identifier.Value);

        record = source.CompareTo(target, orphanTargets: true);
        Assert.NotNull(record);
        Assert.Equal(1, record.Record.Count);
        Assert.Equal("Bond", record.Record[0]); Assert.Equal("[LastName]", record.Schema[0].Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Duplicates_In_Target_Schema()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);

        builder.Add("James", new Meta(engine, "Emp.FirstName"));
        builder.Add("Bond", new Meta(engine, "LastName"));
        builder.Add("007", new Meta(engine, "Emp.Id", isPrimaryKey: true));
        var source = builder.ToMetaRecord();

        builder.Clear();
        builder.Add("007", new Meta(engine, "Emp.Id", isPrimaryKey: true));
        builder.Add("James", new Meta(engine, "Emp.FirstName"));
        builder.Add("008", new Meta(engine, "Emp.Id", isPrimaryKey: true));
        var target = builder.ToMetaRecord();

        var record = source.CompareTo(target);
        Assert.Null(record);

        record = source.CompareTo(target, orphanSources: true);
        Assert.NotNull(record);
        Assert.Equal(1, record.Record.Count);
        Assert.Equal("Bond", record.Record[0]); Assert.Equal("[LastName]", record.Schema[0].Identifier.Value);

        record = source.CompareTo(target, orphanTargets: true);
        Assert.NotNull(record);
        Assert.Equal(1, record.Record.Count);
        Assert.Equal("008", record.Record[0]); Assert.Equal("[Emp].[Id]", record.Schema[0].Identifier.Value);
    }
}