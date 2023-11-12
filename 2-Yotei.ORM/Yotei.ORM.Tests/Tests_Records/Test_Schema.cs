using THost = Yotei.ORM.Records.Code.Schema;
using TItem = Yotei.ORM.Records.Code.SchemaEntry;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Schema
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new Engine();
        var schema = new THost(engine);
        Assert.Empty(schema);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var engine = new Engine();
        var xid = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);

        var schema = new THost(engine, xid);
        Assert.Single(schema);
        Assert.Equal("[dbo].[Employees].[Id]", schema[0].Identifier.Value);
        Assert.True(schema[0].IsPrimaryKey);

        try { _ = new THost(engine, (TItem?)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new THost(engine, new TItem(engine, "")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new THost(engine, new TItem(engine, "x.")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var engine = new Engine();
        var xid = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new TItem(engine, "Employees.FirstName");
        var xlast = new TItem(engine, "LastName");
        var xctry = new TItem(engine, "Country.Id");

        var schema = new THost(engine, [xid, xfirst, xlast, xctry]);
        Assert.Equal(4, schema.Count);
        Assert.Equal("[dbo].[Employees].[Id]", schema[0].Identifier.Value);
        Assert.Equal("[Employees].[FirstName]", schema[1].Identifier.Value);
        Assert.Equal("[LastName]", schema[2].Identifier.Value);
        Assert.Equal("[Country].[Id]", schema[3].Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_With_Duplicates()
    {
        var engine = new Engine();
        var xid = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new TItem(engine, "Employees.FirstName");

        var schema = new THost(engine, [xid, xfirst, xid]);
        Assert.Equal(3, schema.Count);
        Assert.Equal("[dbo].[Employees].[Id]", schema[0].Identifier.Value);
        Assert.Equal("[Employees].[FirstName]", schema[1].Identifier.Value);
        Assert.Same(schema[0], schema[2]);

        try
        {
            // Although same identifier, not the same entry instance...
            _ = new THost(engine, [xid, xfirst, new TItem(engine, "dbo.Employees.Id")]);
            Assert.Fail();
        }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new Engine();
        var xid = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new TItem(engine, "Employees.FirstName");
        var xlast = new TItem(engine, "LastName");
        var xctry = new TItem(engine, "Country.Id");
        var schema = new THost(engine, [xid, xfirst, xlast, xctry]);

        Assert.Equal(0, schema.IndexOf("DBO.EMPLOYEES.ID"));
        Assert.Equal(0, schema.IndexOf("EMPLOYEES.ID"));
        Assert.Equal(0, schema.IndexOf("ID"));
        Assert.Equal(0, schema.IndexOf("EMPLOYEES."));

        Assert.Equal(3, schema.LastIndexOf("ID"));
        Assert.Equal(1, schema.LastIndexOf("EMPLOYEES."));

        var list = schema.IndexesOf("ID");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        list = schema.IndexesOf("EMPLOYEES.");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(1, list[1]);

        Assert.Equal(-1, schema.IndexOf("any"));

        try { schema.IndexOf((string?)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { schema.IndexOf(""); Assert.Fail(); }
        catch (EmptyException) { }
    }
}