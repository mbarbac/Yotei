using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_Schema
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var schema = new Schema(engine);
        Assert.Empty(schema);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);

        var schema = new Schema(engine, xid);
        Assert.Single(schema);
        Assert.Same(xid, schema[0]);

        try { _ = new Schema(engine, (ISchemaEntry)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Schema(engine, new SchemaEntry(engine)); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Schema(engine, new SchemaEntry(engine, "")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Schema(engine, new SchemaEntry(engine, "x.")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var xctry = new SchemaEntry(engine, "Country.Id");

        var schema = new Schema(engine, [xid, xfirst, xlast, xctry]);
        Assert.Equal(4, schema.Count);
        Assert.Same(xid, schema[0]);
        Assert.Same(xfirst, schema[1]);
        Assert.Same(xlast, schema[2]);
        Assert.Same(xctry, schema[3]);

        try { _ = new Schema(engine, (IEnumerable<ISchemaEntry>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Schema(engine, [xid, null!]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Duplicates()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");

        var schema = new Schema(engine, [xid, xfirst, xlast, xid]);
        Assert.Equal(4, schema.Count);
        Assert.Same(xid, schema[0]);
        Assert.Same(xfirst, schema[1]);
        Assert.Same(xlast, schema[2]);
        Assert.Same(xid, schema[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find_Key()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var schema = new Schema(engine, [xid, xfirst, xlast, xid]);

        Assert.Equal(-1, schema.IndexOf("Id"));
        Assert.Equal(-1, schema.IndexOf("Employees.Id"));

        Assert.Equal(0, schema.IndexOf("dbo.EMPLOYEES.Id"));
        Assert.Equal(1, schema.IndexOf("Employees.FIRSTname"));
        Assert.Equal(2, schema.IndexOf("LASTname"));
        Assert.Equal(3, schema.LastIndexOf("dbo.Employees.Id"));

        var list = schema.IndexesOf("DBO.EMPLOYEES.ID");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var schema = new Schema(engine, [xid, xfirst, xlast, xid]);

        Assert.Equal(0, schema.IndexOf(x => x.Identifier.Value!.Contains("Id")));
        Assert.Equal(3, schema.LastIndexOf(x => x.Identifier.Value!.Contains("Id")));

        var list = schema.IndexesOf(x => x.Identifier.Value!.Contains("Id"));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        list = schema.IndexesOf(x => x.IsPrimaryKey);
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Match()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var xctry = new SchemaEntry(engine, "Country.Id");
        var schema = new Schema(engine, [xid, xfirst, xlast, xctry]);

        var nums = schema.Match("any", out var unique);
        Assert.Empty(nums);
        Assert.Null(unique);

        nums = schema.Match("Country", out unique);
        Assert.Empty(nums);
        Assert.Null(unique);

        nums = schema.Match("ID", out unique);
        Assert.Equal(2, nums.Count);
        Assert.Equal(0 , nums[0]);
        Assert.Equal(3, nums[1]);
        Assert.Null(unique);

        nums = schema.Match("employees.", out unique);
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(1, nums[1]);
        Assert.Null(unique);

        nums = schema.Match("LASTname", out unique);
        Assert.Single(nums);
        Assert.Equal(2, nums[0]);
        Assert.NotNull(unique);
        Assert.Same(xlast, unique);

        nums = schema.Match("DBO..", out unique);
        Assert.Single(nums);
        Assert.Equal(0, nums[0]);
        Assert.NotNull(unique);
        Assert.Same(xid, unique);
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Clone()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Employees.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var source = new Schema(engine, [xid, xfirst, xlast, xid]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_GetRange()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Employees.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var source = new Schema(engine, [xid, xfirst, xlast, xid]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Replace()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Employees.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var source = new Schema(engine, [xid, xfirst, xlast, xid]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Add()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Employees.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var source = new Schema(engine, [xid, xfirst, xlast, xid]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_AddRange()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Employees.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var source = new Schema(engine, [xid, xfirst, xlast, xid]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Insert()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Employees.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var source = new Schema(engine, [xid, xfirst, xlast, xid]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_InsertRange()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Employees.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var source = new Schema(engine, [xid, xfirst, xlast, xid]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_RemoveAt()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Employees.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var source = new Schema(engine, [xid, xfirst, xlast, xid]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_RemoveRange()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Employees.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var source = new Schema(engine, [xid, xfirst, xlast, xid]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Remove_Item()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Employees.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var source = new Schema(engine, [xid, xfirst, xlast, xid]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Remove_Predicate()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Employees.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var source = new Schema(engine, [xid, xfirst, xlast, xid]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Clear()
    //{
    //    var engine = new FakeEngine();
    //    var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
    //    var xfirst = new SchemaEntry(engine, "Employees.FirstName");
    //    var xlast = new SchemaEntry(engine, "LastName");
    //    var source = new Schema(engine, [xid, xfirst, xlast, xid]);
    //}
}