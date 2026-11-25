#if NETSTANDARD2_0
namespace Yotei.Tools.NStd20.Tests;
#else
namespace Yotei.Tools.Tests;
#endif

// ========================================================
public static class Test_ExceptionExtensions
{
    [Fact]
    public static void Test_WithData()
    {
        var name = "any";
        var ex = new ArgumentException().WithData(name);

        Assert.True(ex.Data.Contains("name"));
        Assert.Equal("any", ex.Data["name"]);
    }
}