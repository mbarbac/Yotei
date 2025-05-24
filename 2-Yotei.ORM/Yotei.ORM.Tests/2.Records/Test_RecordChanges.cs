using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

using Record = Yotei.ORM.Code.Record;
using Entry = Yotei.ORM.Code.SchemaEntry;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_RecordChanges
{
    //[Enforced]
    [Fact]
    public static void Test_NoChanges_SchemaLess()
    {
        var source = new Record();
        var target = new Record();
        var record = source.GetChanges(target);
        Assert.Null(record);

        source = new Record(["007", "James", "Bond"]);
        target = new Record(["007", "James"]);
        record = source.GetChanges(target);
        Assert.Null(record);

        record = source.GetChanges(target, orphanSources: true);
        Assert.NotNull(record);
        Assert.Single(record);
        Assert.Equal("Bond", record[0]);

        record = source.GetChanges(target, orphanTargets: true);
        Assert.Null(record);
    }

    //[Enforced]
    [Fact]
    public static void Test_NoChanges_SchemaFull()
    {
        var engine = new Engine();
        var builder = new Record.Builder(engine);

        builder.Add("007", new Entry(engine, "Emp.Id", isPrimaryKey: true));
        builder.Add("James", new Entry(engine, "Emp.First"));
        builder.Add("Bond", new Entry(engine, "Last"));
        builder.Add("UK", new Entry(engine, "Ctry.Id"));
        var source = builder.ToInstance();

        builder.Clear();
        builder.Add("Bond", new Entry(engine, "Last"));
        builder.Add("James", new Entry(engine, "Emp.First"));
        builder.Add("007", new Entry(engine, "Emp.Id"));
        builder.Add(50, new Entry(engine, "Age"));
        var target = builder.ToInstance();

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
    public static void Test_WithChanges_SchemaLess()
    {
        var source = new Record();
        var target = new Record();
        var record = source.GetChanges(target);
        Assert.Null(record);

        source = new Record(["007", "James", "Bond"]);
        target = new Record(["008", "James"]);
        record = source.GetChanges(target);
        Assert.NotNull(record);
        Assert.Single(record);
        Assert.Equal("008", record[0]);

        record = source.GetChanges(target, orphanSources: true);
        Assert.NotNull(record);
        Assert.Equal(2, record.Count);
        Assert.Equal("008", record[0]);
        Assert.Equal("Bond", record[1]);

        record = source.GetChanges(target, orphanTargets: true);
        Assert.NotNull(record);
        Assert.Single(record);
        Assert.Equal("008", record[0]);
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
        var source = builder.ToInstance();

        builder.Clear();
        builder.Add("Bond", new Entry(engine, "Last"));
        builder.Add("James", new Entry(engine, "Emp.First"));
        builder.Add("008", new Entry(engine, "Emp.Id"));
        builder.Add(50, new Entry(engine, "Age"));
        var target = builder.ToInstance();

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

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Fail_Mixed_Schema_Modes()
    {
        var engine = new Engine();
        var builder = new Record.Builder(engine);

        builder.Add("007", new Entry(engine, "Emp.Id", isPrimaryKey: true));
        builder.Add("James", new Entry(engine, "Emp.First"));
        builder.Add("Bond", new Entry(engine, "Last"));
        builder.Add("UK", new Entry(engine, "Ctry.Id"));
        var source = builder.ToInstance();

        var target = new Record(["007", "James", "Bond", "UK"]);

        try { source.GetChanges(target); Assert.Fail(); }
        catch (ArgumentException) { }
    }
}