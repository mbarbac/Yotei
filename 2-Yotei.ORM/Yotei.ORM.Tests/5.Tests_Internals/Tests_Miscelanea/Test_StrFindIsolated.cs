namespace Yotei.ORM.Tests.Internals.Miscelanea;

// ========================================================
//[Enforced]
public static class Test_StrFindIsolated
{
    //[Enforced]
    [Fact]
    public static void Test_Source_Empty()
    {
        var finder = new StrFindIsolated();
        var value = "xx";

        var source = "";
        var index = finder.Find(source, value, 0, false); Assert.True(index < 0);
    }

    //[Enforced]
    [Fact]
    public static void Test_Not_Isolated()
    {
        var finder = new StrFindIsolated();
        var value = "xx";

        var source = "any";
        var index = finder.Find(source, value, 0, false); Assert.True(index < 0);

        source = "xxany";
        index = finder.Find(source, value, 0, false); Assert.True(index < 0);

        source = "anyxx";
        index = finder.Find(source, value, 0, false); Assert.True(index < 0);

        source = "xxanyxx";
        index = finder.Find(source, value, 0, false); Assert.True(index < 0);
    }

    //[Enforced]
    [Fact]
    public static void Test_Not_Found_From_Index()
    {
        var finder = new StrFindIsolated();
        var value = "xx";

        var source = "xx any";
        var index = finder.Find(source, value, 1, false); Assert.True(index < 0);

        source = "any xx";
        index = finder.Find(source, value, 5, false); Assert.True(index < 0);

        try { finder.Find(source, value, 99, false); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Isolated()
    {
        var finder = new StrFindIsolated();
        var value = "xx";

        var source = "xx";
        var index = finder.Find(source, value, 0, false); Assert.Equal(0, index);

        source = "xx ";
        index = finder.Find(source, value, 0, false); Assert.Equal(0, index);

        source = " xx";
        index = finder.Find(source, value, 0, false); Assert.Equal(1, index);

        source = " xx ";
        index = finder.Find(source, value, 0, false); Assert.Equal(1, index);
    }

    //[Enforced]
    [Fact]
    public static void Test_Isolated_From_Index()
    {
        var finder = new StrFindIsolated();
        var value = "xx";

        var source = "any xx";
        var index = finder.Find(source, value, 3, false); Assert.Equal(4, index);

        source = "any xx other";
        index = finder.Find(source, value, 3, false); Assert.Equal(4, index);
    }
}