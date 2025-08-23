using Entry = Yotei.ORM.Records.Code.SchemaEntry;

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
        var source = new Schema(engine);
        Assert.Empty(source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);

        var source = new Schema(engine, [xid]);
        Assert.Single(source);
        Assert.Same(xid, source[0]);

        try { _ = new Schema(engine, [null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Schema(engine, [new Entry(engine)]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Schema(engine, [new Entry(engine, "")]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Schema(engine, [new Entry(engine, "x.")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Employees.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Country.Id");

        var source = new Schema(engine, [xid, xfirst, xlast, xctry]);
        Assert.Equal(4, source.Count);
        Assert.Same(xid, source[0]);
        Assert.Same(xfirst, source[1]);
        Assert.Same(xlast, source[2]);
        Assert.Same(xctry, source[3]);

        try { _ = new Schema(engine, (IEnumerable<Entry>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Schema(engine, [xid, null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Schema(engine, [xid, xid]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new Schema(engine, [xid, new Entry(engine, "dbo.Employees.Id")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new Schema(engine, [xid, new Entry(engine, "Id")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Employees.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Country.Id");

        var source = new Schema(engine, [xid, xfirst, xlast, xctry]);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);
    }
    
    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find_Key()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Employees.FirstName");
        var xlast = new Entry(engine, "LastName");
        var source = new Schema(engine, [xid, xfirst, xlast]);

        Assert.Equal(-1, source.IndexOf("Id"));
        Assert.Equal(-1, source.IndexOf("Employees.Id"));

        Assert.Equal(0, source.IndexOf("dbo.EMPLOYEES.Id"));
        Assert.Equal(1, source.IndexOf("Employees.FIRSTname"));
        Assert.Equal(2, source.IndexOf("LASTname"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Employees.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Country.Id");
        var source = new Schema(engine, [xid, xfirst, xlast, xctry]);

        Assert.Equal(0, source.IndexOf(x => x.Identifier.Value!.Contains("Id")));
        Assert.Equal(3, source.LastIndexOf(x => x.Identifier.Value!.Contains("Id")));

        var list = source.IndexesOf(x => x.Identifier.Value!.Contains("Id"));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    // ----------------------------------------------------


    //[Enforced]
    [Fact]
    public static void Test_Match()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Employees.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Country.Id");
        var source = new Schema(engine, [xid, xfirst, xlast, xctry]);

        var nums = source.Match("any", out var unique);
        Assert.Empty(nums);
        Assert.Null(unique);

        nums = source.Match("Country", out unique);
        Assert.Empty(nums);
        Assert.Null(unique);

        nums = source.Match("ID", out unique);
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);
        Assert.Null(unique);

        nums = source.Match("employees.", out unique);
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(1, nums[1]);
        Assert.Null(unique);

        nums = source.Match("LASTname", out unique);
        Assert.Single(nums);
        Assert.Equal(2, nums[0]);
        Assert.NotNull(unique);
        Assert.Same(xlast, unique);

        nums = source.Match("DBO..", out unique);
        Assert.Single(nums);
        Assert.Equal(0, nums[0]);
        Assert.NotNull(unique);
        Assert.Same(xid, unique);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Employees.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Country.Id");
        var source = new Schema(engine, [xid, xfirst, xlast, xctry]);

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
        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Employees.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Country.Id");
        var source = new Schema(engine, [xid, xfirst, xlast, xctry]);

        var target = source.Replace(0, xid);
        Assert.Same(source, target);

        target = source.Replace(1, new Entry(engine, "Employees.FirstName"));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Equal("[Employees].[FirstName]", target[1].Identifier.Value);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);

        target = source.Replace(0, new Entry(engine, "dbo.Employees.Id"));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[dbo].[Employees].[Id]", target[0].Identifier.Value);
        Assert.False(target[0].IsPrimaryKey);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);

        try { source.Replace(0, new Entry(engine, "FirstName")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Replace(0, new Entry(engine, "ID")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Employees.FirstName");
        var xlast = new Entry(engine, "LastName");
        var source = new Schema(engine, [xid, xfirst, xlast]);

        var xctry = new Entry(engine, "Country.Id");
        var target = source.Add(xctry);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);

        try { source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add(new Entry(new FakeEngine() { UseTerminators = false }, "other")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add(new Entry(engine, xid)); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Add(new Entry(engine, "ID")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Add(new Entry(engine, "any.LASTNAME")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Employees.FirstName");
        var source = new Schema(engine, [xid, xfirst]);

        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Country.Id");
        var target = source.AddRange([xlast, xctry]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);

        try { source.AddRange(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.AddRange([new Entry(engine, "any"), xid]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.AddRange([new Entry(engine, "any"), null!]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Employees.FirstName");
        var xlast = new Entry(engine, "LastName");
        var source = new Schema(engine, [xid, xfirst, xlast]);

        var xctry = new Entry(engine, "Country.Id");
        var target = source.Insert(3, xctry);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);

        try { source.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Insert(0, new Entry(new FakeEngine() { UseTerminators = false }, "other")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Insert(0, new Entry(engine, xid)); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Insert(0, new Entry(engine, "ID")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Insert(0, new Entry(engine, "any.LASTNAME")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Insert(0, new Entry(engine, xid)); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Employees.FirstName");
        var source = new Schema(engine, [xid, xfirst]);

        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Country.Id");
        var target = source.InsertRange(2, [xlast, xctry]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);

        try { source.InsertRange(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.InsertRange(0, [new Entry(engine, "any"), null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.AddRange([new Entry(engine, "any"), xid]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Employees.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Country.Id");
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
        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Employees.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Country.Id");
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
    public static void Test_Remove_Name()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Employees.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Country.Id");
        var source = new Schema(engine, [xid, xfirst, xlast, xctry]);

        var target = source.Remove("Id");
        Assert.Same(source, target);

        target = source.Remove("DBO.EMPLOYEES.ID");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
        Assert.Same(xctry, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Employees.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Country.Id");
        var source = new Schema(engine, [xid, xfirst, xlast, xctry]);

        var target = source.Remove(x => x.Identifier.Value == "any");
        Assert.Same(source, target);

        target = source.Remove(x => x.Identifier.Value!.Contains("ID", engine.CaseSensitiveNames));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
        Assert.Same(xctry, target[2]);

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

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new Schema(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        var xid = new Entry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Employees.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Country.Id");

        source = new Schema(engine, [xid, xfirst, xlast, xctry]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}