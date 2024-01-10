using Record = Yotei.ORM.Records.Code.Record;

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

        record = new Record([1, 2]);
        Assert.Equal(2, record.Count);
        Assert.Equal(1, record[0]);
        Assert.Equal(2, record[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Contains()
    {
        var record = new Record(["one", "two", "three"]);

        Assert.Contains("one", record);
        Assert.Equal(0, record.IndexOf(x => (x as string)!.Contains('e')));
        Assert.Equal(2, record.LastIndexOf(x => (x as string)!.Contains('e')));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new Record(["one", "two", "three"]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var source = new Record(["one", "two", "three"]);

        var target = source.GetRange(1, 0);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        target = source.GetRange(0, source.Count);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0]);
        Assert.Equal("three", target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new Record(["one", "two", "three"]);

        var target = source.Replace(0, "one");
        Assert.Same(source, target);

        target = source.Replace(1, "one");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("one", target[1]);
        Assert.Equal("three", target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new Record(["one", "two"]);
        var target = source.Add("three");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new Record(["one"]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange(["two", "three"]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new Record(["one", "two"]);
        var target = source.Insert(2, "three");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new Record(["one"]);
        var target = source.InsertRange(1, []);
        Assert.Same(source, target);

        target = source.InsertRange(1, ["two", "three"]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new Record(["one", "two", "three"]);
        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0]);
        Assert.Equal("three", target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new Record(["one", "two", "three"]);
        var target = source.RemoveRange(1, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, source.Count);
        Assert.Empty(target);

        target = source.RemoveRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("one", target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var source = new Record(["one", "two", "three"]);

        var target = source.Remove(x => (x as string)!.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0]);
        Assert.Equal("three", target[1]);

        target = source.RemoveLast(x => (x as string)!.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);

        target = source.RemoveAll(x => (x as string)!.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("two", target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new Record();
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Record(["one", "two", "three"]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}