using Record = Yotei.ORM.Code.Record;

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
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine, "Emp.Name", isPrimaryKey: true);
        var record = new Record("James", entry);

        Assert.Single(record);
        Assert.Equal("[Emp].[Name]", record.Schema[0].Identifier.Value);
        Assert.Equal("James", record[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Multiple()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Emp.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var xctry = new SchemaEntry(engine, "Ctry.Id");
        var values = new string?[] { "007", "James", "Bond", "UK" };
        var record = new Record(values, [xid, xfirst, xlast, xctry]);

        Assert.Equal(4, record.Count);
        Assert.Equal("[Emp].[Id]", record.Schema[0].Identifier.Value); Assert.Equal("007", record[0]);
        Assert.Equal("[Emp].[FirstName]", record.Schema[1].Identifier.Value); Assert.Equal("James", record[1]);
        Assert.Equal("[LastName]", record.Schema[2].Identifier.Value); Assert.Equal("Bond", record[2]);
        Assert.Equal("[Ctry].[Id]", record.Schema[3].Identifier.Value); Assert.Equal("UK", record[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Emp.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var xctry = new SchemaEntry(engine, "Ctry.Id");
        var values = new string?[] { "007", "James", "Bond", "UK" };

        var source = new Record(values, [xid, xfirst, xlast, xctry]);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[Emp].[Id]", target.Schema[0].Identifier.Value); Assert.Equal("007", target[0]);
        Assert.Equal("[Emp].[FirstName]", target.Schema[1].Identifier.Value); Assert.Equal("James", target[1]);
        Assert.Equal("[LastName]", target.Schema[2].Identifier.Value); Assert.Equal("Bond", target[2]);
        Assert.Equal("[Ctry].[Id]", target.Schema[3].Identifier.Value); Assert.Equal("UK", target[3]);
    }

    //[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Emp.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var xctry = new SchemaEntry(engine, "Ctry.Id");
    //    var values = new string?[] { "007", "James", "Bond", "UK" };

    //    var source = new Record(values, [xid, xfirst, xlast, xctry]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Emp.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var xctry = new SchemaEntry(engine, "Ctry.Id");
    //    var values = new string?[] { "007", "James", "Bond", "UK" };

    //    var source = new Record(values, [xid, xfirst, xlast, xctry]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Emp.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var xctry = new SchemaEntry(engine, "Ctry.Id");
    //    var values = new string?[] { "007", "James", "Bond", "UK" };

    //    var source = new Record(values, [xid, xfirst, xlast, xctry]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Emp.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var xctry = new SchemaEntry(engine, "Ctry.Id");
    //    var values = new string?[] { "007", "James", "Bond", "UK" };

    //    var source = new Record(values, [xid, xfirst, xlast, xctry]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Emp.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var xctry = new SchemaEntry(engine, "Ctry.Id");
    //    var values = new string?[] { "007", "James", "Bond", "UK" };

    //    var source = new Record(values, [xid, xfirst, xlast, xctry]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Emp.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var xctry = new SchemaEntry(engine, "Ctry.Id");
    //    var values = new string?[] { "007", "James", "Bond", "UK" };

    //    var source = new Record(values, [xid, xfirst, xlast, xctry]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Emp.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var xctry = new SchemaEntry(engine, "Ctry.Id");
    //    var values = new string?[] { "007", "James", "Bond", "UK" };

    //    var source = new Record(values, [xid, xfirst, xlast, xctry]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Emp.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var xctry = new SchemaEntry(engine, "Ctry.Id");
    //    var values = new string?[] { "007", "James", "Bond", "UK" };

    //    var source = new Record(values, [xid, xfirst, xlast, xctry]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Emp.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var xctry = new SchemaEntry(engine, "Ctry.Id");
    //    var values = new string?[] { "007", "James", "Bond", "UK" };

    //    var source = new Record(values, [xid, xfirst, xlast, xctry]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Emp.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var xctry = new SchemaEntry(engine, "Ctry.Id");
    //    var values = new string?[] { "007", "James", "Bond", "UK" };

    //    var source = new Record(values, [xid, xfirst, xlast, xctry]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "Emp.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Emp.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var xctry = new SchemaEntry(engine, "Ctry.Id");
    //    var values = new string?[] { "007", "James", "Bond", "UK" };

    //    var source = new Record(values, [xid, xfirst, xlast, xctry]);
    //}
}