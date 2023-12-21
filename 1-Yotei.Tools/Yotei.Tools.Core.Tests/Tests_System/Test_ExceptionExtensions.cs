namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_ExceptionExtensions
{
    //[Enforced]
    [Fact]
    public static void Test_ThrowWhenNull()
    {
        string? name = "James";
        name.ThrowWhenNull();

        try { name = null; name.ThrowWhenNull(); Assert.Fail(); }
        catch (ArgumentNullException ex) { Assert.Equal(nameof(name), ex.ParamName); }
    }

    //[Enforced]
    [Fact]
    public static void Test_WithData()
    {
        var name = "any";
        var ex = new ArgumentException().WithData(name);

        Assert.True(ex.Data.Contains("name"));
        Assert.Equal("any", ex.Data["name"]);        
    }
}