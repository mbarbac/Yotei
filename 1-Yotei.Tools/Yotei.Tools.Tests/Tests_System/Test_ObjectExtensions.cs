using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_ObjectExtensions
{
    // [Enforced]
    [Fact]
    public static void Test_ThrowWhenNull()
    {
        object? obj = null;
        Assert.Throws<ArgumentNullException>(() => { obj.ThrowWhenNull(); });
    }
}