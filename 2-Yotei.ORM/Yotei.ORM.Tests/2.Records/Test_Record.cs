using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

using Record = Yotei.ORM.Records.Code.Record;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_Record
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
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
    public static void Test_Create_Populated()
    {
        var record = new Record([1, "any"]);
        Assert.Equal(2, record.Count);
        Assert.Equal(1, record[0]);
        Assert.Equal("any", record[1]);
        Assert.Null(record.Schema);

        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");

        record = new Record(engine, ["007", "James", "Bond"], [xid, xfirst, xlast]);
        Assert.Equal(3, record.Count);
        Assert.NotNull(record.Schema);
        Assert.Equal("007", record[0]); Assert.Same(xid, record.Schema[0]);
        Assert.Equal("James", record[1]); Assert.Same(xfirst, record.Schema[1]);
        Assert.Equal("Bond", record[2]); Assert.Same(xlast, record.Schema[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Equal_SchemaLess()
    {
        var source = new Record([1, "any"]);
        var target = new Record([1, "any"]);
        Assert.True(source.Equals(target));
        Assert.True(target.Equals(source));

        target = new Record([1]);
        Assert.False(source.Equals(target));
        Assert.False(target.Equals(source));
    }

    //[Enforced]
    [Fact]
    public static void Test_Equal_SchemaFull()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");

        var source = new Record(engine, ["007", "James", "Bond"], [xid, xfirst, xlast]);
        var target = new Record(engine, ["007", "James", "Bond"], [xid, xfirst, xlast]);
        Assert.True(source.Equals(target));
        Assert.True(target.Equals(source));

        target = new Record(engine, ["007", "James"], [xid, xfirst]);
        Assert.False(source.Equals(target));
        Assert.False(target.Equals(source));
    }

    //[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //}
}