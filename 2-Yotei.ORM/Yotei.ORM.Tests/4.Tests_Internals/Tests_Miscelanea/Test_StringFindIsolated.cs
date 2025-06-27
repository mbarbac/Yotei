using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_StringFindIsolated
{
    //[Enforced]
    [Fact]
    public static void Test_StandAlone()
    {
        var value = "xx";
        var source = "";
        var index = value.FindIsolated(source, 0);
        Assert.True(index < 0);

        source = "xx";
        index = value.FindIsolated(source, 0);
        Assert.Equal(0, index);
    }

    //[Enforced]
    [Fact]
    public static void Test_Not_Isolated()
    {
        var value = "xx";
        var source = "anyxx";
        var index = value.FindIsolated(source, 0);
        Assert.True(index < 0);

        source = "xxany";
        index = value.FindIsolated(source, 0);
        Assert.True(index < 0);
    }

    //[Enforced]
    [Fact]
    public static void Test_Isolated_Standard()
    {
        var value = "xx";
        var source = "xx ";
        var index = value.FindIsolated(source, 0);
        Assert.Equal(0, index);

        source = " xx";
        index = value.FindIsolated(source, 0);
        Assert.Equal(1, index);

        source = "(xx)";
        index = value.FindIsolated(source, 0);
        Assert.Equal(1, index);

        source = "xx(xx)";
        index = value.FindIsolated(source, 1);
        Assert.Equal(3, index);
    }

    //[Enforced]
    [Fact]
    public static void Test_Isolated_Special()
    {
        var value = "#xx";
        var source = "#xx ";
        var index = value.FindIsolated(source, 0);
        Assert.Equal(0, index);

        source = "##xx ";
        index = value.FindIsolated(source, 0);
        Assert.Equal(1, index);
    }
}