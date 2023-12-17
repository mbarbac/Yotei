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
    public static void Test_Create_Many()
    {
        var record = new Record([null, null]);
        Assert.Equal(2, record.Count);
        Assert.Null(record[0]);
        Assert.Null(record[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
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
    public static void Test_Replace()
    {
        var source = new Record([1, "any"]);
        var target = source.Replace(0, 1);
        Assert.Same(source, target);

        target = source.Replace(1, "any");
        Assert.Same(source, target);

        target = source.Replace(1, "other");
        Assert.NotSame(source, target);
        Assert.Equal(1, target[0]);
        Assert.Equal("other", target[1]);
    }
}