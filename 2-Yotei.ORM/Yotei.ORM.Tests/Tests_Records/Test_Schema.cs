using THost = Yotei.ORM.Records.Code.Schema;
using TItem = Yotei.ORM.Records.Code.SchemaEntry;
using TPair = System.Collections.Generic.KeyValuePair<string, object?>;

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
        var schema = new THost(engine);
        Assert.Empty(schema);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var engine = new FakeEngine();
        var xId = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var schema = new THost(engine, xId);

        Assert.Single(schema);
        Assert.Equal("[dbo].[Employees].[Id]", schema[0].Identifier.Value);

        try { _ = new THost(engine, (TItem?)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new THost(engine, new TItem(engine)); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new THost(engine, new TItem(engine, "")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new THost(engine, new TItem(engine, "x.")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var engine = new FakeEngine();
        var xId = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xFirst = new TItem(engine, "Employees.FirstName");
        var xLast = new TItem(engine, "LastName");
        var xCtry = new TItem(engine, "Countries.Id");
        var schema = new THost(engine, [xId, xFirst, xLast, xCtry]);

        Assert.Equal(4, schema.Count);
        Assert.Equal("[dbo].[Employees].[Id]", schema[0].Identifier.Value);
        Assert.Equal("[Employees].[FirstName]", schema[1].Identifier.Value);
        Assert.Equal("[LastName]", schema[2].Identifier.Value);
        Assert.Equal("[Countries].[Id]", schema[3].Identifier.Value);

        try { _ = new THost(engine, (IEnumerable<TItem>?)null!); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new THost(engine, [xId, null!]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_With_Duplicates()
    {
        var engine = new FakeEngine();
        var xId = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xFirst = new TItem(engine, "Employees.FirstName");
        var schema = new THost(engine, [xId, xFirst, xId]);

        Assert.Equal(3, schema.Count);
        Assert.Same(xId, schema[0]);
        Assert.Same(xFirst, schema[1]);
        Assert.Same(xId, schema[2]);

        var xClone = xId.Clone();
        try { _ = new THost(engine, [xId, xClone]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var xId = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xFirst = new TItem(engine, "Employees.FirstName");
        var xLast = new TItem(engine, "LastName");
        var xCtry = new TItem(engine, "Countries.Id");
        var schema = new THost(engine, [xId, xFirst, xLast, xCtry]);

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

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var engine = new FakeEngine();
        var xId = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xFirst = new TItem(engine, "Employees.FirstName", isUniqueValued: true);
        var xLast = new TItem(engine, "LastName", isReadOnly: true);
        var xCtry = new TItem(engine, "Countries.Id", metadata: [new TPair("age", 50)]);
        var schema = new THost(engine, [xId, xFirst, xLast, xCtry]);

        Assert.Equal(0, schema.IndexOf(x => x.IsPrimaryKey));
        Assert.Equal(1, schema.IndexOf(x => x.IsUniqueValued));
        Assert.Equal(2, schema.IndexOf(x => x.IsReadOnly));
        Assert.Equal(3, schema.IndexOf(x => x.Contains("age")));
    }

    //[Enforced]
    [Fact]
    public static void Test_Match()
    {
        var engine = new FakeEngine();
        var xId = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xFirst = new TItem(engine, "Employees.FirstName");
        var xLast = new TItem(engine, "LastName");
        var xCtry = new TItem(engine, "Countries.Id");
        var schema = new THost(engine, [xId, xFirst, xLast, xCtry]);

        var list = schema.Match("DBO.EMPLOYEES.ID", out var unique);
        Assert.Single(list);
        Assert.Same(schema[0], list[0]);
        Assert.Same(schema[0], unique);

        list = schema.Match("EMPLOYEES.ID", out unique);
        Assert.Single(list);
        Assert.Same(schema[0], list[0]);
        Assert.Same(schema[0], unique);

        list = schema.Match("ID", out unique);
        Assert.Equal(2, list.Count);
        Assert.Same(schema[0], list[0]);
        Assert.Same(schema[3], list[1]);
        Assert.Null(unique);

        list = schema.Match("EMPLOYEES.", out unique);
        Assert.Equal(2, list.Count);
        Assert.Same(schema[0], list[0]);
        Assert.Same(schema[1], list[1]);
        Assert.Null(unique);

        list = schema.Match("LASTNAME", out unique);
        Assert.Single(list);
        Assert.Same(schema[2], list[0]);
        Assert.Same(schema[2], unique);

        list = schema.Match("COUNTRIES.", out unique);
        Assert.Single(list);
        Assert.Same(schema[3], list[0]);
        Assert.Same(schema[3], unique);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var xId = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xFirst = new TItem(engine, "Employees.FirstName");
        var xLast = new TItem(engine, "LastName");
        var xCtry = new TItem(engine, "Countries.Id");        
        var source = new THost(engine, [xId, xFirst, xLast, xCtry]);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
        Assert.Same(source[3], target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine();
        var xId = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xFirst = new TItem(engine, "Employees.FirstName");
        var xLast = new TItem(engine, "LastName");
        var xCtry = new TItem(engine, "Countries.Id");
        var source = new THost(engine, [xId, xFirst, xLast, xCtry]);

        var target = source.GetRange(1, 0);
        Assert.Empty(target);

        target = source.GetRange(0, source.Count);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.Same(source[1], target[0]);
        Assert.Same(source[2], target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var xId = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xFirst = new TItem(engine, "Employees.FirstName");
        var xLast = new TItem(engine, "LastName");
        var xCtry = new TItem(engine, "Countries.Id");
        var source = new THost(engine, [xId, xFirst, xLast, xCtry]);

        var xOrg = new TItem(engine, "Organization.Id");
        var target = source.Replace(3, xOrg);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
        Assert.Same(xOrg, target[3]);

        target = source.Replace(3, xId);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
        Assert.Same(xId, target[3]);

        try { source.Replace(3, new TItem(engine, "Id")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var xId = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xFirst = new TItem(engine, "Employees.FirstName");
        var xLast = new TItem(engine, "LastName");
        var source = new THost(engine, [xId, xFirst, xLast]);

        var xCtry = new TItem(engine, "Countries.Id");
        var target = source.Add(xCtry);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
        Assert.Same(xCtry, target[3]);

        target = source.Add(xId);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
        Assert.Same(xId, target[3]);

        try { source.Add(new TItem(engine, "Id")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Add(new TItem(engine, "Employees.LastName")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add(new TItem(engine, "")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add(new TItem(engine, "x.")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange() { }

    //[Enforced]
    [Fact]
    public static void Insert()
    {
        var engine = new FakeEngine();
        var xId = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xFirst = new TItem(engine, "Employees.FirstName");
        var xLast = new TItem(engine, "LastName");
        var source = new THost(engine, [xId, xFirst, xLast]);

        var xCtry = new TItem(engine, "Countries.Id");
        var target = source.Insert(3, xCtry);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
        Assert.Same(xCtry, target[3]);

        target = source.Insert(3, xId);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
        Assert.Same(xId, target[3]);

        try { source.Insert(3, new TItem(engine, "Id")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Insert(3, new TItem(engine, "Employees.LastName")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Insert(3, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Insert(3, new TItem(engine, "")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Insert(3, new TItem(engine, "x.")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange() { }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine();
        var xId = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xFirst = new TItem(engine, "Employees.FirstName");
        var xLast = new TItem(engine, "LastName");
        var xCtry = new TItem(engine, "Countries.Id");
        var source = new THost(engine, [xId, xFirst, xLast, xCtry]);

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(source[1], target[0]);
        Assert.Same(source[2], target[1]);
        Assert.Same(source[3], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var xId = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xFirst = new TItem(engine, "Employees.FirstName");
        var xLast = new TItem(engine, "LastName");
        var xCtry = new TItem(engine, "Countries.Id");
        var source = new THost(engine, [xId, xFirst, xLast, xCtry]);

        var target = source.RemoveRange(1, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, source.Count);
        Assert.Empty(target);

        target = source.RemoveRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xId, target[0]);
        Assert.Same(xCtry, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var engine = new FakeEngine();
        var xId = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xFirst = new TItem(engine, "Employees.FirstName");
        var xLast = new TItem(engine, "LastName");
        var source = new THost(engine, [xId, xFirst, xLast, xId]);

        var target = source.Remove("ID");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xFirst, target[0]);
        Assert.Same(xLast, target[1]);
        Assert.Same(xId, target[2]);

        target = source.RemoveLast("ID");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xId, target[0]);
        Assert.Same(xFirst, target[1]);
        Assert.Same(xLast, target[2]);

        target = source.RemoveAll("ID");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xFirst, target[0]);
        Assert.Same(xLast, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate() { }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var xId = new TItem(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xFirst = new TItem(engine, "Employees.FirstName");
        var xLast = new TItem(engine, "LastName");
        var source = new THost(engine, [xId, xFirst, xLast]);

        var target = source.Clear();
        Assert.Empty(target);
    }
}