namespace Yotei.ORM.Tests;

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

        try { _ = new Schema(engine, (SchemaEntry)null!); Assert.Fail(); }
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

        try { _ = new Schema(engine, (IEnumerable<SchemaEntry>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Schema(engine, [xid, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many_With_Duplicates()
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

        try { _ = new Schema(engine, [xid, new SchemaEntry(engine, "Id")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new Schema(engine, [xfirst, new SchemaEntry(engine, "FirstName")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new Schema(engine, [xlast, new SchemaEntry(engine, "x..LastName")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Key()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var xctry = new SchemaEntry(engine, "Country.Id");
        var schema = new Schema(engine, [xid, xfirst, xlast, xid]);

        Assert.Equal(-1, schema.IndexOf("Id"));
        Assert.Equal(-1, schema.IndexOf("Employees.Id"));
        Assert.Equal(0, schema.IndexOf("dbo.Employees.Id"));
        Assert.Equal(0, schema.IndexOf("DBO.EMPLOYEES.ID"));
        Assert.Equal(3, schema.LastIndexOf("dbo.Employees.Id"));
        Assert.Equal(3, schema.LastIndexOf("DBO.EMPLOYEES.ID"));

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

        nums = schema.Match("ID", out unique);
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);
        Assert.Null(unique);

        nums = schema.Match("EMPLOYEES.", out unique);
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(1, nums[1]);
        Assert.Null(unique);

        nums = schema.Match("LASTname", out unique);
        Assert.Single(nums);
        Assert.Equal(2, nums[0]);
        Assert.NotNull(unique);
        Assert.Same(unique, xlast);

        nums = schema.Match("DBO..", out unique);
        Assert.Single(nums);
        Assert.Equal(0, nums[0]);
        Assert.NotNull(unique);
        Assert.Same(unique, xid);

        nums = schema.Match("Country", out unique);
        Assert.Empty(nums);
        Assert.Null(unique);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var xctry = new SchemaEntry(engine, "Country.Id");

        var source = new Schema(engine, [xid, xfirst, xlast, xctry]);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var xctry = new SchemaEntry(engine, "Country.Id");
        var source = new Schema(engine, [xid, xfirst, xlast, xctry]);

        var target = source.GetRange(0, 0);
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

        var source = new Schema(engine, [xid, xfirst, xlast]);
        var target = source.Replace(0, xid);
        Assert.Same(source, target);

        target = source.Replace(0, new SchemaEntry(engine, "dbo.Employees.Id"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("[dbo].[Employees].[Id]", target[0].Identifier.Value);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);

        try { _ = source.Replace(0, new SchemaEntry(engine, "FirstName")); Assert.Fail(); }
        catch (DuplicateException) { }

        source = new Schema(engine, [xid, xfirst, xlast, xid]);
        target = source.Replace(0, xid);
        Assert.Same(source, target);

        try { _ = source.Replace(0, new SchemaEntry(engine, "dbo.Employees.Id")); Assert.Fail(); }
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

        try { source.Add(new SchemaEntry(engine, "")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add(new SchemaEntry(engine, "ID")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Add(new SchemaEntry(engine, "x.LASTNAME")); Assert.Fail(); }
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

        target = source.Insert(3, xid);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xid, target[3]);

        try { source.Insert(3, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Insert(3, new SchemaEntry(engine, "")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Insert(3, new SchemaEntry(engine, "ID")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Insert(3, new SchemaEntry(engine, "x.LASTNAME")); Assert.Fail(); }
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

        target = source.RemoveRange(1, 0);
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
        var target = source.Remove(x => x.Identifier.Value == null);
        Assert.Same(source, target);

        target = source.Remove(x => x.Identifier.Value!.Contains("Id"));
        Assert.NotSame(source, target);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
        Assert.Same(xid, target[2]);

        target = source.RemoveLast(x => x.Identifier.Value!.Contains("Id"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);

        target = source.RemoveAll(x => x.Identifier.Value!.Contains("Id"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var xId = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xFirst = new SchemaEntry(engine, "Employees.FirstName");
        var xLast = new SchemaEntry(engine, "LastName");
        var source = new Schema(engine, [xId, xFirst, xLast]);

        var target = source.Clear();
        Assert.Empty(target);
    }
}