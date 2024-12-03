using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public class Test_ObjectExtensions
{
    // [Enforced]
    [Fact]
    public void Test_ThrowWhenNull()
    {
        object? obj = null;
        Assert.Throws<ArgumentNullException>(() => { obj.ThrowWhenNull(); });
    }
}