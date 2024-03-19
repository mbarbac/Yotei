using Record = Yotei.ORM.Code.Record;

namespace Yotei.ORM.Tests;

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
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var record = new Record((object?)null);
        Assert.Single(record);
        Assert.Null(record[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Multiple()
    {
        var record = new Record([]);
        Assert.Empty(record);

        record = new Record(["one", null, "three"]);
        Assert.Equal(3, record.Count);
        Assert.Equal("one", record[0]);
        Assert.Null(record[1]);
        Assert.Equal("three", record[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var source = new Record(["one", null, "three"]);
        var target = source.GetRange(0, 3);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Null(target[0]);
        Assert.Equal("three", target[1]);

        try { source.GetRange(-1, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { source.GetRange(5, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { source.GetRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { source.GetRange(0, 5); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new Record(["one", null, "three"]);
        var target = source.Replace(0, "one");
        Assert.Same(source, target);

        target = source.Replace(1, null);
        Assert.Same(source, target);

        target = source.Replace(1, "two");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);

        try { _ = source.Replace(-1, null); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { _ = source.Replace(3, null); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new Record(["one", null, "three"]);
        var target = source.Add(50);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Null(target[1]);
        Assert.Equal("three", target[2]);
        Assert.Equal(50, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new Record(["one", null, "three"]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange([50, "five"]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Null(target[1]);
        Assert.Equal("three", target[2]);
        Assert.Equal(50, target[3]);
        Assert.Equal("five", target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new Record(["one", null, "three"]);
        var target = source.Insert(3, 50);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Null(target[1]);
        Assert.Equal("three", target[2]);
        Assert.Equal(50, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new Record(["one", null, "three"]);
        var target = source.InsertRange(3, []);
        Assert.Same(source, target);

        target = source.InsertRange(3, [50, "five"]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Null(target[1]);
        Assert.Equal("three", target[2]);
        Assert.Equal(50, target[3]);
        Assert.Equal("five", target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new Record(["one", null, "three"]);
        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Null(target[0]);
        Assert.Equal("three", target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new Record(["one", null, "three"]);
        var target = source.RemoveRange(1, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("one", target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new Record();
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Record(["one", null, "three"]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}