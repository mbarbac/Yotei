#if NETSTANDARD2_0
namespace Yotei.Tools.Std20.Tests;
#else
namespace Yotei.Tools;
#endif

// ========================================================
public static class Test_ArgumentNullException
{
    [Fact]
    public static void Test_ThrowIfNull()
    {
        //string? value = null;
        //try { ArgumentNullException.ThrowIfNull(value); Assert.Fail(); }
        //catch (ArgumentNullException) { }
    }
}