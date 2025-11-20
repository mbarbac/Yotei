namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_Parameter
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var item = new Parameter("#one", null);
        Assert.Equal("#one", item.Name);
        Assert.Null(item.Value);

        item = new Parameter("#one", 25);
        Assert.Equal("#one", item.Name);
        Assert.Equal(25, item.Value);

        try { _ = new Parameter(null!, "whatever"); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Parameter(string.Empty, "whatever"); Assert.Fail(); }
        catch (EmptyException) { }

        try { _ = new Parameter(" ", "whatever"); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Equality()
    {
        var one = new Parameter("one", 7);
        var two = new Parameter("one", 7);

        Assert.True(one.Equals(two));
        Assert.True(one == two);
        Assert.Equal(one, two);
        Assert.False(ReferenceEquals(one, two));

        two = new Parameter("ONE", 7);
        Assert.False(one.Equals(two));
        Assert.False(one == two);
        Assert.NotEqual(one, two);

        Assert.True(one.Equals(two, caseSensitiveNames: false));
    }
}