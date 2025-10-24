namespace Yotei.Tools.Tests;

// ========================================================
public class Test_ObjectExtensions
{
    [Fact]
    public void Test_ThrowWhenNull()
    {
        string? name = "James";
        name.ThrowWhenNull();

        try { name = null; name.ThrowWhenNull(); Assert.Fail(); }
        catch (ArgumentNullException ex) { }
    }

    // ----------------------------------------------------

    [Fact]
    public void Test_EqualsEx()
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
