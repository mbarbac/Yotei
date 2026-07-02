using System.Xml.Schema;
using Entry = Yotei.ORM.Records.Code.SchemaEntry;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Schema
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
    public static void Test_Create_Range()
    {
        var engine = new FakeEngine();
        var schema = new Schema(engine, []);
        Assert.Empty(schema);

        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");

        schema = new Schema(engine, [xid, xfirst, xlast, xctry]);
        Assert.Equal(4, schema.Count);
        Assert.Same(xid, schema[0]);
        Assert.Same(xfirst, schema[1]);
        Assert.Same(xlast, schema[2]);
        Assert.Same(xctry, schema[3]);

        schema = new Schema(engine, [xid, xid]);
        Assert.Equal(2, schema.Count);
        Assert.Same(xid, schema[0]);
        Assert.Same(xid, schema[1]);

        schema = new Schema(engine, [xid, new Entry(engine, "EMP.ID")]);
        Assert.Equal(2, schema.Count);
        Assert.Same(xid, schema[0]);
        Assert.Equal("[EMP].[ID]", schema[1].Identifier!.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Errors()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");

        try { _ = new Schema(engine, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Schema(engine, [xid, null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Schema(engine, [xid, new Entry(engine)]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Schema(engine, [xid, new Entry(engine, ".")]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Schema(engine, [xid, new Entry(engine, "any.")]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Schema(engine, [xid, new Entry(engine, "EMP.ID")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new Schema(engine, [xid, new Entry(engine, "ID")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try
        {
            _ = new Schema(engine, [xid, xfirst, xlast, new Entry(engine, "ANY.LastName")]);
            Assert.Fail();
        }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");

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
    public static void Test_IndexOf_Identifier()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");

        List<int> indexes;
        IIdentifier id;
        var schema = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);

        id = new Identifier(engine, "any"); Assert.Equal(-1, schema.IndexOf(id));
        try { schema.IndexOf(new Identifier(new FakeEngine(), "any")); Assert.Fail(); }
        catch (ArgumentException) { }

        id = new Identifier(engine, "Emp.Id");
        Assert.Equal(0, schema.IndexOf(id));
        Assert.Equal(4, schema.LastIndexOf(id));
        indexes = schema.IndexesOf(id);
        Assert.Equal(2, indexes.Count);
        Assert.Equal(0, indexes[0]);
        Assert.Equal(4, indexes[1]);

        id = new Identifier(engine, "EMP.ID");
        Assert.Equal(0, schema.IndexOf(id));
        Assert.Equal(4, schema.LastIndexOf(id));
        indexes = schema.IndexesOf(id);
        Assert.Equal(2, indexes.Count);
        Assert.Equal(0, indexes[0]);
        Assert.Equal(4, indexes[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_String()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");
        var schema = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);
        string id;
        List<int> indexes;

        Assert.Equal(-1, schema.IndexOf("any"));

        id = "Emp.Id";
        Assert.Equal(0, schema.IndexOf(id));
        Assert.Equal(4, schema.LastIndexOf(id));
        indexes = schema.IndexesOf(id);
        Assert.Equal(2, indexes.Count);
        Assert.Equal(0, indexes[0]);
        Assert.Equal(4, indexes[1]);

        id = "EMP.ID";
        Assert.Equal(0, schema.IndexOf(id));
        Assert.Equal(4, schema.LastIndexOf(id));
        indexes = schema.IndexesOf(id);
        Assert.Equal(2, indexes.Count);
        Assert.Equal(0, indexes[0]);
        Assert.Equal(4, indexes[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Predicate()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");
        var schema = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);
        int index;
        List<int> indexes;

        index = schema.IndexOf(x => x.Identifier is null); Assert.Equal(-1, index);

        index = schema.IndexOf(x => x.Identifier!.Contains("Id")); Assert.Equal(0, index);
        index = schema.LastIndexOf(x => x.Identifier!.Contains("Id")); Assert.Equal(4, index);
        indexes = schema.IndexesOf(x => x.Identifier!.Contains("Id"));
        Assert.Equal(3, indexes.Count);
        Assert.Equal(0, indexes[0]);
        Assert.Equal(3, indexes[1]);
        Assert.Equal(4, indexes[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Match()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");
        var schema = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);

        var indexes = schema.Match("any", out var unique);
        Assert.Empty(indexes);
        Assert.Null(unique);

        indexes = schema.Match("Id", out unique);
        Assert.Equal(3, indexes.Count);
        Assert.Equal(0, indexes[0]);
        Assert.Equal(3, indexes[1]);
        Assert.Equal(4, indexes[2]);
        Assert.Null(unique);

        indexes = schema.Match("EMP.", out unique);
        Assert.Equal(3, indexes.Count);
        Assert.Equal(0, indexes[0]);
        Assert.Equal(1, indexes[1]);
        Assert.Equal(4, indexes[2]);
        Assert.Null(unique);

        indexes = schema.Match("FIRSTNAME", out unique);
        Assert.Single(indexes);
        Assert.Equal(1, indexes[0]);
        Assert.NotNull(unique); Assert.Same(xfirst, unique);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");
        var schema = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);

        Assert.False(schema.TryFind(x => x.Identifier is null, out var entry));
        Assert.Null(entry);

        Assert.True(schema.TryFind(x => x.Identifier!.Contains("Id"), out entry));
        Assert.Same(xid, entry);

        Assert.True(schema.TryFindLast(x => x.Identifier!.Contains("Id"), out entry));
        Assert.Same(xid, entry);

        Assert.True(schema.TryFindAll(x => x.Identifier!.Contains("Id"), out var range));
        Assert.Equal(3, range.Count);
        Assert.Same(xid, range[0]);
        Assert.Same(xctry, range[1]);
        Assert.Same(xid, range[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");

        var source = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);
        var target = source.GetRange(1, 3);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
        Assert.Same(xctry, target[2]);

        try { source.GetRange(-1, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.GetRange(0, -1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.GetRange(6, 0); Assert.Fail(); } catch (ArgumentException) { }
        try { source.GetRange(0, 6); Assert.Fail(); } catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");

        var source = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);
        var target = source.Replace(0, xid);
        Assert.Same(source, target);

        try { _ = source.Replace(0, new Entry(engine, "Emp.Id")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.Replace(0, new Entry(engine, "Id")); Assert.Fail(); }
        catch (DuplicateException) { }

        target = source.Replace(0, new Entry(engine, "ANY"));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[ANY]", target[0].Identifier!.Value);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);
        Assert.Same(xid, target[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");

        var source = new Schema(engine);
        var target = source.Add(xid);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xid, target[0]);

        source = new Schema(engine, [xid, xfirst]);
        target = source.Add(new Entry(engine, "Emp.Age"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Equal("[Emp].[Age]", target[2].Identifier!.Value);

        try { source.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Add(new Entry(engine, "ID")); Assert.Fail(); } catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");

        var source = new Schema(engine);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        source = new Schema(engine, [xid, xfirst]);
        target = source.AddRange([xlast, xctry, xid]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);
        Assert.Same(xid, target[4]);

        try { source.AddRange(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.AddRange([null!]); Assert.Fail(); } catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");

        var source = new Schema(engine);
        var target = source.Insert(0, xid);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xid, target[0]);

        source = (Schema)target;
        target = source.Insert(1, xfirst);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);

        source = (Schema)target;
        target = source.Insert(0, xlast);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xlast, target[0]);
        Assert.Same(xid, target[1]);
        Assert.Same(xfirst, target[2]);

        try { source.Insert(0, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Insert(0, new Entry(engine, "ID")); Assert.Fail(); } catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");

        var source = new Schema(engine);
        var target = source.InsertRange(0, []);
        Assert.Same(source, target);

        target = source.InsertRange(0, [xid, xfirst]);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);

        source = (Schema)target;
        target = source.InsertRange(2, [xlast, xctry]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);

        try { source.InsertRange(0, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.InsertRange(0, [null!]); Assert.Fail(); } catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");

        var source = new Schema(engine, [xid, xfirst, xlast]);
        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Schema(engine);
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");
        source = new Schema(engine, [xid, xfirst, xlast, xctry]);
        target = source.RemoveRange(0, 1);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
        Assert.Same(xctry, target[2]);

        target = source.RemoveRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xctry, target[1]);

        try { source.RemoveRange(-1, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(4, 1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(0, 5); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(1, 4); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(2, 3); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Identifier()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");

        var source = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);
        var target = source.Remove(new Identifier(engine, "Any"));
        Assert.Same(source, target);

        target = source.Remove(xid.Identifier!);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
        Assert.Same(xctry, target[2]);
        Assert.Same(xid, target[3]);

        target = source.RemoveLast(xid.Identifier!);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);

        target = source.RemoveAll(xid.Identifier!);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
        Assert.Same(xctry, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_String()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");

        var source = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);
        var target = source.Remove("Any");
        Assert.Same(source, target);

        target = source.Remove("Emp.Id");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
        Assert.Same(xctry, target[2]);
        Assert.Same(xid, target[3]);

        target = source.RemoveLast("Emp.Id");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);

        target = source.RemoveAll("EMP.ID");
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
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        var xctry = new Entry(engine, "Ctry.Id");

        var source = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);
        var target = source.Remove(x => x.Identifier is null);
        Assert.Same(source, target);

        target = source.Remove(x => x.Identifier!.Contains("Id"));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
        Assert.Same(xctry, target[2]);
        Assert.Same(xid, target[3]);

        target = source.RemoveLast(x => x.Identifier!.Contains("Id"));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);

        target = source.RemoveAll(x => x.Identifier!.Contains("Id"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Schema(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        var xid = new Entry(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Entry(engine, "Emp.FirstName");
        var xlast = new Entry(engine, "LastName");
        source = new Schema(engine, [xid, xfirst, xlast]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}