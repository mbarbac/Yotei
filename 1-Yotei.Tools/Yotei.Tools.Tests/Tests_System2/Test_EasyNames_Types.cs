namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyNames_Types
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyNames_Types);

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_System_Types()
    {
        // Default...
        var item = typeof(string);
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("String", name);
    }
}