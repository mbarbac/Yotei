#if NETSTANDARD2_0
namespace Yotei.Tools.NStd20.Tests;
#else
namespace Yotei.Tools.Tests;
#endif

// ========================================================
public static class Test_ObjectExtensions
{
    [Fact]
    public static void Test_ThrowWhenNull()
    {
        string? name = "James";
        ArgumentNullException.ThrowIfNull(name);

        try { name = null; ArgumentNullException.ThrowIfNull(name); Assert.Fail(""); }
        catch (ArgumentNullException) { }
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_EqualsEx()
    {
        string? x = null;
        string? y = null;
        Assert.True(x.EqualsEx(y)); Assert.True(y.EqualsEx(x));

        x = "";
        y = null;
        Assert.False(x.EqualsEx(y)); Assert.False(y.EqualsEx(x));

        x = "one";
        y = "one";
        Assert.True(x.EqualsEx(y)); Assert.True(y.EqualsEx(x));
    }
}