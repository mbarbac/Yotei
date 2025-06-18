using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_StringExtensions
{
    //[Enforced]
    [Fact]
    public static void Test_NullWhenDynamicName_Null_Source()
    {
        string? source = null;
        string? darg = null;
        bool caseSensitive = false;
        var target = source.NullWhenDynamicName(darg, caseSensitive);
        Assert.Null(target);

        darg = "x";
        target = source.NullWhenDynamicName(darg, caseSensitive);
        Assert.Null(target);
    }

    //[Enforced]
    [Fact]
    public static void Test_NullWhenDynamicName_No_Match()
    {
        string? source = "any";
        string? darg = "x";
        bool caseSensitive = false;
        var target = source.NullWhenDynamicName(darg, caseSensitive);
        Assert.Equal("any", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_NullWhenDynamicName_With_Match()
    {
        string? source = "X";
        string? darg = "x";
        bool caseSensitive = false;
        var target = source.NullWhenDynamicName(darg, caseSensitive);
        Assert.Null(target);
    }
}