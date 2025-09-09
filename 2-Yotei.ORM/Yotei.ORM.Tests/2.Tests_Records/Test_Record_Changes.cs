#pragma warning disable IDE0028

using Record = Yotei.ORM.Records.Code.Record;
using Entry = Yotei.ORM.Records.Code.SchemaEntry;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_RecordChanges
{

    //[Enforced]
    [Fact]
    public static void Test_NoChanges_SchemaLessFails()
    {
        var source = new Record();
        var target = new Record();
        try { source.GetChanges(target); Assert.Fail(); }
        catch (InvalidOperationException) { }

        var engine = new FakeEngine();
        var schema = new Schema(engine, [new Entry(engine, "Any")]);
        source = new Record(engine, ["Other"], schema);
        target = [];
        try { source.GetChanges(target); Assert.Fail(); }
        catch (InvalidOperationException) { }

        source = [];
        target = new Record(engine, ["Other"], schema);
        try { source.GetChanges(target); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_NoChanges_SchemaFull()
    {
        var engine = new Engine();
        var builder = new Record.Builder(engine);

        builder.Add("007", new Entry(engine, "Emp.Id", isPrimaryKey: true));
        builder.Add("James", new Entry(engine, "Emp.First"));
        builder.Add("Bond", new Entry(engine, "Emp.Last"));
        builder.Add("UK", new Entry(engine, "Ctry.Id"));
        var source = builder.CreateInstance();
        builder.Clear();

        builder.Add("James", new Entry(engine, "Emp.First"));
        builder.Add("Bond", new Entry(engine, "Emp.Last"));
        builder.Add("007", new Entry(engine, "Emp.Id", isPrimaryKey: true));
        builder.Add(50, new Entry(engine, "Age"));
        var target = builder.CreateInstance();
        builder.Clear();

        var record = source.GetChanges(target);
        Assert.Null(record);

        record = source.GetChanges(target, orphanSources: true);
        Assert.NotNull(record);
        Assert.Single(record);
        Assert.Equal("UK", record[0]); Assert.Equal(source.Schema![3], record.Schema![0]);

        record = source.GetChanges(target, orphanTargets: true);
        Assert.NotNull(record);
        Assert.Single(record);
        Assert.Equal(50, record[0]); Assert.Equal(target.Schema![3], record.Schema![0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_WithChanges_SchemaLessFails()
    {
        var source = new Record(["007", "James", "Bond"]);
        var target = new Record(["008", "James"]);
        try { source.GetChanges(target); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_WithChanges_SchemaFull()
    {
        var engine = new Engine();
        var builder = new Record.Builder(engine);
        builder.Add("007", new Entry(engine, "Emp.Id", isPrimaryKey: true));
        builder.Add("James", new Entry(engine, "Emp.First"));
        builder.Add("Bond", new Entry(engine, "Last"));
        builder.Add("UK", new Entry(engine, "Ctry.Id"));
        var source = builder.CreateInstance();

        builder.Clear();
        builder.Add("Bond", new Entry(engine, "Last"));
        builder.Add("James", new Entry(engine, "Emp.First"));
        builder.Add("008", new Entry(engine, "Emp.Id"));
        builder.Add(50, new Entry(engine, "Age"));
        var target = builder.CreateInstance();

        var record = source.GetChanges(target);
        Assert.NotNull(record);
        Assert.Single(record);
        Assert.Equal("008", record[0]); Assert.Equal(source.Schema![0], record.Schema![0]);

        record = source.GetChanges(target, orphanSources: true);
        Assert.NotNull(record);
        Assert.Equal(2, record.Count);
        Assert.Equal("008", record[0]); Assert.Equal(source.Schema![0], record.Schema![0]);
        Assert.Equal("UK", record[1]); Assert.Equal(source.Schema![3], record.Schema![1]);

        record = source.GetChanges(target, orphanTargets: true);
        Assert.NotNull(record);
        Assert.Equal(2, record.Count);
        Assert.Equal("008", record[0]); Assert.Equal(source.Schema![0], record.Schema![0]);
        Assert.Equal(50, record[1]); Assert.Equal(target.Schema![3], record.Schema![1]);
    }
}