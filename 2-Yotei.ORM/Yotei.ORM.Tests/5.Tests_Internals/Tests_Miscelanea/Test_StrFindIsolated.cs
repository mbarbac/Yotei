namespace Yotei.ORM.Tests.Internals.Miscelanea;

// ========================================================
//[Enforced]
public static class Test_StrFindIsolated
{
    //[Enforced]
    [Fact]
    public static void Test_Source_Empty()
    {
        var value = "xx";
        var source = "";
        var index = source.FindIsolated(value, 0); Assert.True(index < 0);
    }

    //[Enforced]
    [Fact]
    public static void Test_Not_Isolated()
    {
        var value = "xx";
        var source = "any";
        var index = source.FindIsolated(value, 0); Assert.True(index < 0);

        source = "xxany";
        index = source.FindIsolated(value, 0); Assert.True(index < 0);

        source = "anyxx";
        index = source.FindIsolated(value, 0); Assert.True(index < 0);
    }

    //[Enforced]
    [Fact]
    public static void Test_Not_Found_From_Index()
    {
        var value = "xx";
        var source = "xx any";
        var index = source.FindIsolated(value, 1); Assert.True(index < 0);

        source = "any xx";
        index = source.FindIsolated(value, 5); Assert.True(index < 0);

        try { index = source.FindIsolated(value, 99); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Isolated()
    {
        var value = "xx";
        var source = "xx";
        var index = source.FindIsolated(value, 0); Assert.Equal(0, index);

        source = "xx ";
        index = source.FindIsolated(value, 0); Assert.Equal(0, index);

        source = " xx";
        index = source.FindIsolated(value, 0); Assert.Equal(1, index);

        source = " xx ";
        index = source.FindIsolated(value, 0); Assert.Equal(1, index);
    }

    //[Enforced]
    [Fact]
    public static void Test_Isolated_From_Index()
    {
        var value = "xx";
        var source = "any xx";
        var index = source.FindIsolated(value, 1); Assert.Equal(4, index);

        source = "any xx other";
        index = source.FindIsolated(value, 1); Assert.Equal(4, index);
    }
}