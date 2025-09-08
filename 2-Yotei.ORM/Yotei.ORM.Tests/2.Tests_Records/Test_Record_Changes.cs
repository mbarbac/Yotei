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
    public static void Test_NoChanges_SchemaLess() { }

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
}