#pragma warning disable IDE0028

using Record = Yotei.ORM.Records.Code.Record;
using Entry = Yotei.ORM.Records.Code.SchemaEntry;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Record
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_SchemaLess()
    {
        var record = new Record();
        Assert.Empty(record);
        Assert.Null(record.Schema);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_SchemaReady()
    {
        var engine = new FakeEngine();

        var record = new Record(engine);
        Assert.Empty(record);
        Assert.NotNull(record.Schema);

        var schema = new Schema(engine);
        record = new Record([], schema);
        Assert.Empty(record);
        Assert.NotSame(schema, record.Schema);
        Assert.Equal(schema, record.Schema);

        try { record = new Record([], []); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_SchemaLess()
    {
        var record = new Record(["007", "James", "Bond", "UK"]);
        Assert.Equal(4, record.Count);
        Assert.Equal("007", record[0]);
        Assert.Equal("James", record[1]);
        Assert.Equal("Bond", record[2]);
        Assert.Equal("UK", record[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_SchemaReady()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");
        var schema = new Schema(engine, [xid, xfirst, xlast, xctry]);

        var record = new Record(["007", "James", "Bond", "UK"], schema);
        Assert.Equal(4, record.Count);
        Assert.Equal(schema, record.Schema);
        Assert.Equal("007", record[0]);
        Assert.Equal("James", record[1]);
        Assert.Equal("Bond", record[2]);
        Assert.Equal("UK", record[3]);

        Assert.Equal("007", record["Emp.Id"]);
        Assert.Equal("James", record["Emp.FirstName"]);
        Assert.Equal("Bond", record["LASTNAME"]);
        Assert.Equal("UK", record["ctry.id"]);

        try { record = new Record([], schema); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_With_Duplicates()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var schema = new Schema(engine, [xid, xfirst, xid, xid]);

        var record = new Record(["007", "James", "008", "009"], schema);
        Assert.Equal(4, record.Count);
        Assert.NotNull(record.Schema);
        Assert.Equal("009", record[0]); // Last one wins!
        Assert.Equal("James", record[1]);
        Assert.Equal("009", record[2]);
        Assert.Equal("009", record[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Indexed_Setter_WithDuplicates()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var schema = new Schema(engine, [xid, xfirst, xid, xid]);
        var builder = new Record.Builder(["007", "James", "007", "007"], schema);

        builder[3] = "008";
        Assert.Equal("008", builder[0]);
        Assert.Equal("008", builder[2]);
        Assert.Equal("008", builder[3]);

        builder["Emp.Id"] = "009";
        Assert.Equal("009", builder[0]);
        Assert.Equal("009", builder[2]);
        Assert.Equal("009", builder[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Get()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var schema = new Schema(engine, [xid, xfirst, xid]);
        var record = new Record(["007", "James", "008"], schema);

        var value = record.Get(0, out var entry);
        Assert.Equal("008", value);
        Assert.Same(xid, entry);

        try { new Record().Get(0, out entry); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_TryGet()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var schema = new Schema(engine, [xid, xfirst, xid]);
        var record = new Record(["007", "James", "008"], schema);

        var done = record.TryGet("whatever", out var _, out var _);
        Assert.False(done);

        done = record.TryGet("Emp.Id", out var value, out var entry);
        Assert.True(done);
        Assert.Equal("008", value);
        Assert.Same(xid, entry);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone_SchemaLess()
    {
        var source = new Record();
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Null(target.Schema);

        source = new(["007", "James", "Bond", "UK"]);
        target = source.Clone();

        Assert.Null(target.Schema);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("007", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);
        Assert.Equal("UK", target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone_SchemaReady()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");
        var schema = new Schema(engine, [xid, xfirst, xlast, xctry]);

        var source = new Record(["007", "James", "Bond", "UK"], schema);
        var target = source.Clone();

        Assert.NotSame(source.Schema, target.Schema);
        Assert.Equal(source.Schema, target.Schema);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("007", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);
        Assert.Equal("UK", target[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace_SchemaLess()
    {
        var source = new Record(["007", "James", "Bond", "UK", "008"]);
        var target = source.Replace(0, "009");
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("009", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);
        Assert.Equal("UK", target[3]);
        Assert.Equal("008", target[4]);

        try { source.Replace(0, "any", new Entry(new FakeEngine(), "other")); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_SchemaReady()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");
        var schema = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);

        var source = new Record(["007", "James", "Bond", "UK", "008"], schema);
        var target = source.Replace(0, "009");
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("009", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);
        Assert.Equal("UK", target[3]);
        Assert.Equal("009", target[4]);

        var xother = new Entry(engine, "Other");
        target = source.Replace(0, "010", xother);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("010", target[0]); Assert.Same(xother, target.Schema![0]);
        Assert.Equal("James", target[1]); Assert.Same(xfirst, target.Schema![1]);
        Assert.Equal("Bond", target[2]); Assert.Same(xlast, target.Schema![2]);
        Assert.Equal("UK", target[3]); Assert.Same(xctry, target.Schema![3]);
        Assert.Equal("010", target[4]); Assert.Same(xother, target.Schema![4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_SchemaLess()
    {
        var source = new Record();
        var target = source.Add("007");

        Assert.NotSame(source, target);
        Assert.Null(target.Schema);
        Assert.Single(target);
        Assert.Equal("007", target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_SchemaReady()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");

        var schema = new Schema(engine, [xid]);
        var source = new Record(["007"], schema);

        try { source.Add("James"); Assert.Fail(); }
        catch (InvalidOperationException) { }

        var target = source.Add("James", xfirst);
        Assert.NotSame(source, target);
        Assert.NotNull(target.Schema);
        Assert.Equal(2, target.Count);
        Assert.Equal("007", target[0]); Assert.Same(xid, target.Schema![0]);
        Assert.Equal("James", target[1]); Assert.Same(xfirst, target.Schema![1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddRange_SchemaLess()
    {
        var source = new Record();
        var target = source.AddRange(["007", "James"]);

        Assert.NotSame(source, target);
        Assert.Null(target.Schema);
        Assert.Equal(2, target.Count);
        Assert.Equal("007", target[0]);
        Assert.Equal("James", target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_SchemaReady()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");
        var schema = new Schema(engine, [xid, xfirst, xlast]);

        var source = new Record(["007", "James", "Bond"], schema);
        var target = source.AddRange(["UK", "008"], [xctry, xid]);
        Assert.NotSame(source, target);
        Assert.NotNull(target.Schema);
        Assert.Equal(5, target.Count);
        Assert.Equal("008", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);
        Assert.Equal("UK", target[3]);
        Assert.Equal("008", target[4]);

        try { source.AddRange(["any"]); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert_SchemaLess()
    {
        var source = new Record();
        var target = source.Insert(0, "007");

        Assert.NotSame(source, target);
        Assert.Null(target.Schema);
        Assert.Single(target);
        Assert.Equal("007", target[0]);

        try { source.Insert(1, "any"); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_SchemaReady()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");

        var schema = new Schema(engine, [xid]);
        var source = new Record(["007"], schema);

        try { source.Insert(1, "James"); Assert.Fail(); }
        catch (InvalidOperationException) { }

        var target = source.Insert(1, "James", xfirst);
        Assert.NotSame(source, target);
        Assert.NotNull(target.Schema);
        Assert.Equal(2, target.Count);
        Assert.Equal("007", target[0]); Assert.Same(xid, target.Schema![0]);
        Assert.Equal("James", target[1]); Assert.Same(xfirst, target.Schema![1]);

        try { source.Insert(2, "James", xfirst); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_SchemaLess()
    {
        var source = new Record();
        var target = source.InsertRange(0, ["007", "James"]);

        Assert.NotSame(source, target);
        Assert.Null(target.Schema);
        Assert.Equal(2, target.Count);
        Assert.Equal("007", target[0]);
        Assert.Equal("James", target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_SchemaReady()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");
        var schema = new Schema(engine, [xid, xfirst, xlast]);

        var source = new Record(["007", "James", "Bond"], schema);
        var target = source.InsertRange(3, ["UK", "008"], [xctry, xid]);
        Assert.NotSame(source, target);
        Assert.NotNull(target.Schema);
        Assert.Equal(5, target.Count);
        Assert.Equal("008", target[0]);
        Assert.Equal("James", target[1]);
        Assert.Equal("Bond", target[2]);
        Assert.Equal("UK", target[3]);
        Assert.Equal("008", target[4]);

        try { source.InsertRange(0, ["any"]); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    // ----------------------------------------------------

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
    public static void Test_RemoveAt_SchemaReady()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");
        var schema = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);

        var source = new Record(["007", "James", "Bond", "UK", "008"], schema);
        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.NotNull(target.Schema);
        Assert.Equal(4, target.Count);
        Assert.Equal("James", target[0]); Assert.Same(xfirst, target.Schema![0]);
        Assert.Equal("Bond", target[1]); Assert.Same(xlast, target.Schema![1]);
        Assert.Equal("UK", target[2]); Assert.Same(xctry, target.Schema![2]);
        Assert.Equal("008", target[3]); Assert.Same(xid, target.Schema![3]);

        target = source.RemoveAt(0, removeRedundantEntries: true);
        Assert.NotSame(source, target);
        Assert.NotNull(target.Schema);
        Assert.Equal(3, target.Count);
        Assert.Equal("James", target[0]); Assert.Same(xfirst, target.Schema![0]);
        Assert.Equal("Bond", target[1]); Assert.Same(xlast, target.Schema![1]);
        Assert.Equal("UK", target[2]); Assert.Same(xctry, target.Schema![2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_By_Identifier()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");
        var schema = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);

        var source = new Record(["007", "James", "Bond", "UK", "008"], schema);
        var target = source.Remove("Any");
        Assert.Same(source, target);

        target = source.Remove("Emp.Id");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("James", target[0]); Assert.Same(xfirst, target.Schema![0]);
        Assert.Equal("Bond", target[1]); Assert.Same(xlast, target.Schema![1]);
        Assert.Equal("UK", target[2]); Assert.Same(xctry, target.Schema![2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new Record();
        var target = source.Clear();
        Assert.Same(source, target);

        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");
        var schema = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);

        source = new Record(["007", "James", "Bond", "UK", "008"], schema);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.NotNull(target.Schema); Assert.Empty(target.Schema);
        Assert.Empty(target);

        target = source.Clear(removeSchema: true);
        Assert.NotSame(source, target);
        Assert.Null(target.Schema);
        Assert.Empty(target);
    }
}