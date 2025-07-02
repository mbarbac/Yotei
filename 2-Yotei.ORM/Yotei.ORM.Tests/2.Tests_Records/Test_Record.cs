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
    [Fact]
    public static void Test_Clone_SchemaLess()
    {
        var source = new Record([1, "any"]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal(1, target[0]);
        Assert.Equal("any", target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone_SchemaFull()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");

        var source = new Record(engine, ["007", "James", "Bond"], [xid, xfirst, xlast]);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.NotNull(target.Schema);
        Assert.Equal("007", target[0]); Assert.Same(xid, target.Schema[0]);
        Assert.Equal("James", target[1]); Assert.Same(xfirst, target.Schema[1]);
        Assert.Equal("Bond", target[2]); Assert.Same(xlast, target.Schema[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_WithSchema_SchemaLess()
    {
        var source = new Record(["007", "James", "Bond"]);
        var target = source.WithSchema(null);
        Assert.NotSame(source, target);
        Assert.Null(target.Schema);

        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var schema = new Schema(engine, [xid, xfirst, xlast]);

        target = source.WithSchema(schema);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.NotNull(target.Schema);
        Assert.Equal("007", target[0]); Assert.Same(xid, target.Schema[0]);
        Assert.Equal("James", target[1]); Assert.Same(xfirst, target.Schema[1]);
        Assert.Equal("Bond", target[2]); Assert.Same(xlast, target.Schema[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_WithSchema_SchemaFull()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");

        var source = new Record(engine, ["007", "James", "Bond"], [xid, xfirst, xlast]);
        var target = source.WithSchema(null);
        Assert.NotSame(source, target);
        Assert.Null(target.Schema);

        try { source.WithSchema(new Schema(engine, [xid, xfirst])); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetRange_SchemaLess()
    {
        var source = new Record(["007", "James", "Bond"]);
        var target = source.GetRange(0, 3);
        Assert.Same(source, target);

        target = source.GetRange(0, 0);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("James", target[0]);
        Assert.Equal("Bond", target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange_SchemaFull()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var source = new Record(engine, ["007", "James", "Bond"], [xid, xfirst, xlast]);

        var target = source.GetRange(0, 3);
        Assert.Same(source, target);

        target = source.GetRange(0, 0);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("James", target[0]); Assert.Same(xfirst, target.Schema![0]);
        Assert.Equal("Bond", target[1]); Assert.Same(xlast, target.Schema![1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_SchemaLess()
    {
        var source = new Record(["007", "James", "Bond"]);
        var target = source.Replace(0, "007");
        Assert.Same(source, target);

        target = source.Replace(0, "008");
        Assert.NotSame(source, target);
        Assert.Equal("008", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);

        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id");
        try { source.Replace(0, "008", xid); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_SchemaFull()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var source = new Record(engine, ["007", "James", "Bond"], [xid, xfirst, xlast]);

        var target = source.Replace(0, "007", xid);
        Assert.Same(source, target);

        target = source.Replace(0, "008", xid);
        Assert.NotSame(source, target);
        Assert.Equal("008", target[0]); Assert.Equal("[dbo].[Employees].[Id]", target.Schema![0].Identifier.Value);
        Assert.Equal("James", target[1]); Assert.Same(xfirst, target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(xlast, target.Schema![2]);

        xid = new SchemaEntry(engine, "People.Id", isPrimaryKey: true);
        target = source.Replace(0, "007", xid);
        Assert.Equal("007", target[0]); Assert.Equal("[People].[Id]", target.Schema![0].Identifier.Value);
        Assert.Equal("James", target[1]); Assert.Same(xfirst, target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(xlast, target.Schema![2]);

        // We don't need to specify the entry, it is kept by default...
        target = source.Replace(0, "008");
        Assert.Equal("008", target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_SchemaLess()
    {
        var source = new Record(["007", "James"]);
        var target = source.Add("Bond");
        Assert.NotSame(source, target);
        Assert.Equal("007", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);

        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id");
        try { source.Add("any", xid); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_SchemaFull()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var source = new Record(engine, ["007", "James"], [xid, xfirst]);

        var xlast = new SchemaEntry(engine, "LastName");
        var target = source.Add("Bond", xlast);
        Assert.NotSame(source, target);
        Assert.Equal("007", target[0]); Assert.Same(xid, target.Schema![0]);
        Assert.Equal("James", target[1]); Assert.Same(xfirst, target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(xlast, target.Schema![2]);

        try { source.Add("any", null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add("any"); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_SchemaLess()
    {
        var source = new Record(["007"]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange(["James", "Bond"]);
        Assert.NotSame(source, target);
        Assert.Equal("007", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);

        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id");
        try { source.AddRange(["any"], [xid]); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_SchemaFull()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var source = new Record(engine, ["007"], [xid]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        target = source.AddRange(["James", "Bond"], [xfirst, xlast]);
        Assert.NotSame(source, target);
        Assert.Equal("007", target[0]); Assert.Same(xid, target.Schema![0]);
        Assert.Equal("James", target[1]); Assert.Same(xfirst, target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(xlast, target.Schema![2]);

        try { source.AddRange(["any"], null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.AddRange(["any"]); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_SchemaLess()
    {
        var source = new Record(["007", "James"]);
        var target = source.Insert(2, "Bond");
        Assert.NotSame(source, target);
        Assert.Equal("007", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);

        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id");
        try { source.Insert(0, "any", xid); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_SchemaFull()
    {
        var engine = new FakeEngine();
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var source = new Record(engine, ["James", "Bond"], [xfirst, xlast]);

        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var target = source.Insert(0, "007", xid);
        Assert.NotSame(source, target);
        Assert.Equal("007", target[0]); Assert.Same(xid, target.Schema![0]);
        Assert.Equal("James", target[1]); Assert.Same(xfirst, target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(xlast, target.Schema![2]);

        try { source.Insert(0, "any", null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Insert(0, "any"); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_SchemaLess()
    {
        var source = new Record(["007"]);
        var target = source.InsertRange(1, ["James", "Bond"]);
        Assert.NotSame(source, target);
        Assert.Equal("007", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);

        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id");
        try { source.InsertRange(0, ["any"], [xid]); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_SchemaFull()
    {
        var engine = new FakeEngine();
        var xlast = new SchemaEntry(engine, "LastName");
        var source = new Record(engine, ["Bond"], [xlast]);

        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var target = source.InsertRange(0, ["007", "James"], [xid, xfirst]);
        Assert.NotSame(source, target);
        Assert.Equal("007", target[0]); Assert.Same(xid, target.Schema![0]);
        Assert.Equal("James", target[1]); Assert.Same(xfirst, target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(xlast, target.Schema![2]);

        try { source.InsertRange(0, ["any"], null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.InsertRange(0, ["any"]); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt_SchemaLess()
    {
        var source = new Record(["007", "James", "Bond"]);

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("James", target[0]);
        Assert.Equal("Bond", target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt_SchemaFull()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var source = new Record(engine, ["007", "James", "Bond"], [xid, xfirst, xlast]);

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("James", target[0]); Assert.Same(xfirst, target.Schema![0]);
        Assert.Equal("Bond", target[1]); Assert.Same(xlast, target.Schema![1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange_SchemaLess()
    {
        var source = new Record(["007", "James", "Bond"]);

        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("Bond", target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange_SchemaFull()
    {
        var engine = new FakeEngine();
        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        var source = new Record(engine, ["007", "James", "Bond"], [xid, xfirst, xlast]);

        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("Bond", target[0]); Assert.Same(xlast, target.Schema![0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear_SchemaLess()
    {
        var source = new Record();
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Record(["007", "James", "Bond"]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear_SchemaFull()
    {
        var engine = new FakeEngine();
        var source = new Record(engine);

        var target = source.Clear();
        Assert.Same(source, target);

        var xid = new SchemaEntry(engine, "dbo.Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        source = new Record(engine, ["007", "James", "Bond"], [xid, xfirst, xlast]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}