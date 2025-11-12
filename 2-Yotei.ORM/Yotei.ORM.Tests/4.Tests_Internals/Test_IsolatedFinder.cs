namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public class Test_IsoltatedFinder
{
    //[Enforced]
    [Fact]
    public void Test_Source_Empty()
    {
        var finder = new IsolatedFinder();

        Assert.True(finder.Find("", 0, "") < 0);
        Assert.True(finder.Find("", 0, "xx") < 0);
    }

    //[Enforced]
    [Fact]
    public void Test_Not_Isolated()
    {
        var finder = new IsolatedFinder();
        var value = "xx";

        Assert.True(finder.Find("any", 0, value) < 0);
        Assert.True(finder.Find("xxany", 0, value) < 0);
        Assert.True(finder.Find("anyxx", 0, value) < 0);
        Assert.True(finder.Find("xxanyxx", 0, value) < 0);
    }

    //[Enforced]
    [Fact]
    public void Test_Not_Isolated_From_Index()
    {
        var finder = new IsolatedFinder();
        var value = "xx";

        Assert.True(finder.Find("xx", 1, value) < 0);
        Assert.True(finder.Find("xx any", 1, value) < 0);
        Assert.True(finder.Find("any xx", 5, value) < 0);
    }

    //[Enforced]
    [Fact]
    public void Test_Isolated()
    {
        var finder = new IsolatedFinder();
        var value = "xx";

        Assert.Equal(0, finder.Find("xx", 0, value));
        Assert.Equal(0, finder.Find("xx ", 0, value));
        Assert.Equal(1, finder.Find(" xx", 0, value));
        Assert.Equal(1, finder.Find(" xx ", 0, value));
    }

    //[Enforced]
    [Fact]
    public void Test_Isolated_From_Index()
    {
        var finder = new IsolatedFinder();
        var value = "xx";

        Assert.Equal(4, finder.Find("any xx", 3, value));
        Assert.Equal(4, finder.Find("any xx other", 3, value));

        Assert.Equal(4, finder.Find("any xx", 4, value));
        Assert.Equal(4, finder.Find("any xx other", 4, value));
    }
}