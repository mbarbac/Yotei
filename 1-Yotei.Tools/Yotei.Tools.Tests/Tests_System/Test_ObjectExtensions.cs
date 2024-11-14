namespace Yotei.Tools.Tests;

// ========================================================
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