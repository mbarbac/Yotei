using System.ComponentModel.DataAnnotations;
using Element = Yotei.ORM.Records.Code.SchemaElement;
using Schema = Yotei.ORM.Records.Code.SchemaDescriptor;

namespace Yotei.ORM.Records.Tests;

// ========================================================
//[Enforced]
public static class Test_SchemaDescriptor
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

        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");
        var xctry = new Element(engine, "Ctry.Id");

        schema = new Schema(engine, [xid, xfirst, xlast, xctry]);
        Assert.Equal(4, schema.Count);
        Assert.Same(xid, schema[0]);
        Assert.Same(xfirst, schema[1]);
        Assert.Same(xlast, schema[2]);
        Assert.Same(xctry, schema[3]);

        schema = new Schema(engine, [xid, new Element(engine, "ID")]);
        Assert.Equal(2, schema.Count);
        Assert.Same(xid, schema[0]);
        Assert.Equal("[ID]", schema[1].Identifier!.Value);

        try { schema = new Schema(engine, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Invalid_Element()
    {
        ISchemaDescriptor schema;
        var engine = new FakeEngine();
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");
        var xctry = new Element(engine, "Ctry.Id");

        try { schema = new Schema(engine, [xid, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { schema = new Schema(engine, [xid, new Element(engine)]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { schema = new Schema(engine, [xid, new Element(engine, "")]); Assert.Fail(); }
        catch (EmptyException) { }

        try { schema = new Schema(engine, [xid, new Element(engine, ".")]); Assert.Fail(); }
        catch (EmptyException) { }

        try { schema = new Schema(engine, [xid, new Element(engine, "any.")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Duplicates()
    {
        var engine = new FakeEngine();
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xlast = new Element(engine, "LastName");

        var schema = new Schema(engine, [xid, xid]);
        Assert.Equal(2, schema.Count);
        Assert.Same(xid, schema[0]);
        Assert.Same(xid, schema[1]);

        try { _ = new Schema(engine, [xid, new Element(engine, "Emp.Id")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new Schema(engine, [xid, new Element(engine, "Id")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new Schema(engine, [xid, xlast, new Element(engine, "Emp.LastName")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");
        var xctry = new Element(engine, "Ctry.Id");

        var source = new Schema(engine, [xid, xfirst, xlast, xctry]);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Same(source.Engine, target.Engine);

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
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");
        var schema = new Schema(engine, [xid, xfirst, xlast, xid]);

        var id = new Identifier(engine, "any");
        var index = schema.IndexOf(id);
        Assert.Equal(-1, index);

        id = new Identifier(engine, "EMP.ID");
        index = schema.IndexOf(id); Assert.Equal(0, index);
        index = schema.LastIndexOf(id); Assert.Equal(3, index);

        var indexes = schema.IndexesOf(id);
        Assert.Equal(2, indexes.Count);
        Assert.Equal(0, indexes[0]);
        Assert.Equal(3, indexes[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_String()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");
        var schema = new Schema(engine, [xid, xfirst, xlast, xid]);

        var index = schema.IndexOf("any");
        Assert.Equal(-1, index);

        index = schema.IndexOf("EMP.ID"); Assert.Equal(0, index);
        index = schema.LastIndexOf("EMP.ID"); Assert.Equal(3, index);

        var indexes = schema.IndexesOf("EMP.ID");
        Assert.Equal(2, indexes.Count);
        Assert.Equal(0, indexes[0]);
        Assert.Equal(3, indexes[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Predicate()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");
        var schema = new Schema(engine, [xid, xfirst, xlast, xid]);

        var index = schema.IndexOf(x => x.Identifier is null); Assert.Equal(-1, index);

        index = schema.IndexOf(x => x.Identifier!.Contains("Id")); Assert.Equal(0, index);
        index = schema.LastIndexOf(x => x.Identifier!.Contains("Id")); Assert.Equal(3, index);

        var indexes = schema.IndexesOf(x => x.Identifier!.Contains("Id"));
        Assert.Equal(2, indexes.Count);
        Assert.Equal(0, indexes[0]);
        Assert.Equal(3, indexes[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Match()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");
        var xctry = new Element(engine, "Ctry.Id");
        var schema = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);

        var indexes = schema.Match("any", out var unique);
        Assert.Empty(indexes);
        Assert.Null(unique);

        indexes = schema.Match("ID", out unique);
        Assert.Equal(3, indexes.Count);
        Assert.Equal(0, indexes[0]);
        Assert.Equal(3, indexes[1]);
        Assert.Equal(4, indexes[2]);
        Assert.Null(unique);

        indexes = schema.Match("FIRSTNAME", out unique);
        Assert.Single(indexes);
        Assert.Equal(1, indexes[0]);
        Assert.Same(xfirst, unique);

        indexes = schema.Match("EMP.", out unique);
        Assert.Equal(3, indexes.Count);
        Assert.Equal(0, indexes[0]);
        Assert.Equal(1, indexes[1]);
        Assert.Equal(4, indexes[2]);
        Assert.Null(unique);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");
        var xctry = new Element(engine, "Ctry.Id");
        var schema = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);

        Assert.False(schema.TryFind(x => x.Identifier is null, out var item));
        Assert.Null(item);

        Assert.True(schema.TryFind(x => x.Identifier!.Contains("ID"), out item));
        Assert.Same(xid, item);

        Assert.True(schema.TryFindLast(x => x.Identifier!.Contains("ID"), out item));
        Assert.Same(xid, item);

        Assert.True(schema.TryFindAll(x => x.Identifier!.Contains("ID"), out var items));
        Assert.Equal(3, items.Count);
        Assert.Same(xid, items[0]);
        Assert.Same(xctry, items[1]);
        Assert.Same(xid, items[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");
        var xctry = new Element(engine, "Ctry.Id");

        var source = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);
        var target = source.GetRange(0, 0);
        Assert.Empty(target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);

        try { source.GetRange(-1, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.GetRange(0, -1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.GetRange(6, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.GetRange(0, 6); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");
        var xctry = new Element(engine, "Ctry.Id");

        var source = new Schema(engine, [xid, xfirst, xlast, xctry, xid]);
        var target = source.Replace(0, xid);
        Assert.Same(source, target);

        target = source.Replace(0, new Element(engine, "Any"));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[Any]", target[0].Identifier!.Value);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xctry, target[3]);
        Assert.Same(xid, target[4]);

        try { source.Replace(0, xid.Clone()); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Replace(-1, new Element(engine, "Any")); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.Replace(5, new Element(engine, "Any")); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");

        var source = new Schema(engine, [xid]);
        var target = source.Add(xfirst);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);

        target = source.Add(xid);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xid, target[1]);

        try { target = source.Add(xid.Clone()); Assert.Fail(); } catch (DuplicateException) { }
        try { target = source.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Invalid()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");
        var xctry = new Element(engine, "Ctry.Id");
        var source = new Schema(engine, [xid, xfirst, xlast, xctry]);

        try { _ = source.Add(new Element(engine, "Id")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.Add(new Element(engine, "Emp.LastName")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");

        var source = new Schema(engine, [xid]);
        var target = source.AddRange([xfirst, xlast, xid]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xid, target[3]);

        try { _ = source.AddRange(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = source.AddRange([xfirst, null!]); Assert.Fail(); } catch (ArgumentNullException) { }
        try { target = source.AddRange([xid.Clone()]); Assert.Fail(); } catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");

        var source = new Schema(engine, [xid]);
        var target = source.Insert(0, xfirst);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xid, target[1]);        

        target = source.Insert(1, xid);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xid, target[1]);

        try { target = source.Insert(0, xid.Clone()); Assert.Fail(); } catch (DuplicateException) { }
        try { target = source.Insert(0, null!); Assert.Fail(); } catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Invalid()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");
        var xctry = new Element(engine, "Ctry.Id");
        var source = new Schema(engine, [xid, xfirst, xlast, xctry]);

        try { _ = source.Insert(0, new Element(engine, "Id")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.Insert(0, new Element(engine, "Emp.LastName")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");

        var source = new Schema(engine, [xid]);
        var target = source.InsertRange(1, [xfirst, xlast, xid]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);
        Assert.Same(xid, target[3]);

        try { _ = source.InsertRange(0, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = source.InsertRange(0, [xfirst, null!]); Assert.Fail(); } catch (ArgumentNullException) { }
        try { target = source.InsertRange(0, [xid.Clone()]); Assert.Fail(); } catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");
        
        var source = new Schema(engine, [xid, xfirst, xlast]);
        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);

        try { source.RemoveAt(-1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveAt(4); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");

        var source = new Schema(engine);
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        source = new Schema(engine, [xid, xfirst, xlast]);
        target = source.RemoveRange(0, 1);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);

        target = source.RemoveRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xid, target[0]);

        target = source.RemoveRange(0, 3);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        try { source.RemoveRange(-1, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(3, 1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(0, 4); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(1, 3); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(2, 2); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Identifier()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");

        var source = new Schema(engine, [xid, xfirst, xlast, xid]);
        var target = source.Remove(new Identifier(engine, "Any"));
        Assert.Same(source, target);

        target = source.Remove(new Identifier(engine, "EMP.ID"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
        Assert.Same(xid, target[2]);

        target = source.RemoveLast(new Identifier(engine, "EMP.ID"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);

        target = source.RemoveAll(new Identifier(engine, "EMP.ID"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_String()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");

        var source = new Schema(engine, [xid, xfirst, xlast, xid]);
        var target = source.Remove(new Identifier(engine, "Any"));
        Assert.Same(source, target);

        target = source.Remove("EMP.ID");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
        Assert.Same(xid, target[2]);

        target = source.RemoveLast("EMP.ID");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);

        target = source.RemoveAll("EMP.ID");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");

        var source = new Schema(engine, [xid, xfirst, xlast, xid]);
        var target = source.Remove(x => x.Identifier is null);
        Assert.Same(source, target);

        target = source.Remove(x => x.Identifier!.Value!.Contains("Id"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xfirst, target[0]);
        Assert.Same(xlast, target[1]);
        Assert.Same(xid, target[2]);

        target = source.RemoveLast(x => x.Identifier!.Value!.Contains("Id"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xid, target[0]);
        Assert.Same(xfirst, target[1]);
        Assert.Same(xlast, target[2]);

        target = source.RemoveAll(x => x.Identifier!.Value!.Contains("Id"));
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
        var engine = new FakeEngine();
        var source = new Schema(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        var xid = new Element(engine, "Emp.Id", isPrimaryKey: true);
        var xfirst = new Element(engine, "Emp.FirstName");
        var xlast = new Element(engine, "LastName");
        
        source = new Schema(engine, [xid, xfirst, xlast, xid]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}