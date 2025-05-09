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
        Assert.Equal(0, nums[0]);
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
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var source = new Schema(engine, [xid, xfirst, xlast, xid]);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xid, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var source = new Schema(engine, [xid, xfirst, xlast, xid]);

        var target = source.GetRange(0, 0);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        target = source.GetRange(1, 2);
        Assert.Equal(2, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var source = new Schema(engine, [xid, xfirst, xlast, xid]);

        var target = source.Replace(0, xid);
        Assert.Same(source, target);

        target = source.Replace(1, new SchemaEntry(engine, "Employees.FirstName"));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Equal("[Employees].[FirstName]", target[1].Identifier.Value);
        Assert.Same(xlast, target[2]);
        Assert.Same(xid, target[3]);

        // The second id throws the duplication...
        try { source.Replace(0, new SchemaEntry(engine, "dbo.Employees.Id")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Replace(0, new SchemaEntry(engine, "FirstName")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Replace(0, new SchemaEntry(engine, "ID")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var source = new Schema(engine, [xid, xfirst, xlast]);

        var xctry = new SchemaEntry(engine, "Country.Id");
        var target = source.Add(xctry);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);

        target = source.Add(xid);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xid, target[3]);

        try { source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add(new SchemaEntry(new FakeEngine() { UseTerminators = false }, "other")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add(new SchemaEntry(engine, "ID")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Add(new SchemaEntry(engine, "any.LASTNAME")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var source = new Schema(engine, [xid, xfirst]);

        var xlast = new SchemaEntry(engine, "LastName");
        var xctry = new SchemaEntry(engine, "Country.Id");
        var target = source.AddRange([xlast, xctry]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);

        try { source.AddRange(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.AddRange([xid, null!]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var source = new Schema(engine, [xid, xfirst, xlast]);

        var xctry = new SchemaEntry(engine, "Country.Id");
        var target = source.Insert(3, xctry);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);

        target = source.Insert(0, xid);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xid, target[1]);
        Assert.Same(xfirst, target[2]);
        Assert.Same(xlast, target[3]);

        try { source.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Insert(0, new SchemaEntry(new FakeEngine() { UseTerminators = false }, "other")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Insert(0, new SchemaEntry(engine, "ID")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Insert(0, new SchemaEntry(engine, "any.LASTNAME")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var source = new Schema(engine, [xid, xfirst]);

        var xlast = new SchemaEntry(engine, "LastName");
        var xctry = new SchemaEntry(engine, "Country.Id");
        var target = source.InsertRange(2, [xlast, xctry]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);

        try { source.InsertRange(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.InsertRange(0, [xid, null!]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var xctry = new SchemaEntry(engine, "Country.Id");
        var source = new Schema(engine, [xid, xfirst, xlast, xctry]);

        var target = source.RemoveAt(0);
        Assert.Equal(3, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
        Assert.Same(xctry, target[2]);
        Assert.DoesNotContain(xid, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var xctry = new SchemaEntry(engine, "Country.Id");
        var source = new Schema(engine, [xid, xfirst, xlast, xctry]);

        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(1, 2);
        Assert.Equal(2, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xctry, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var source = new Schema(engine, [xid, xfirst, xlast, xid]);

        var target = source.Remove("Id");
        Assert.Same(source, target);

        target = source.Remove("DBO.EMPLOYEES.ID");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
        Assert.Same(xid, target[2]);

        target = source.RemoveLast("DBO.EMPLOYEES.ID");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);

        target = source.RemoveAll("DBO.EMPLOYEES.ID");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var source = new Schema(engine, [xid, xfirst, xlast, xid]);

        var target = source.Remove(x => x.Identifier.Value == "any");
        Assert.Same(source, target);

        target = source.Remove(x => x.Identifier.Value!.Contains("ID", engine.CaseSensitiveNames));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
        Assert.Same(xid, target[2]);

        target = source.RemoveLast(x => x.Identifier.Value!.Contains("ID", engine.CaseSensitiveNames));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);

        target = source.RemoveAll(x => x.Identifier.Value!.Contains("ID", engine.CaseSensitiveNames));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
    }

    [Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new Schema(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var xctry = new SchemaEntry(engine, "Country.Id");
        
        source = new Schema(engine, [xid, xfirst, xlast, xctry]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}