#pragma warning disable CA1822
#pragma warning disable IDE0060

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyNames_Properties
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyNames_Properties);

    // -----------------------------------------------------

    public class TXA { public class TA { public int Name { get; set; } } }

    //[Enforced]
    [Fact]
    public static void Test_Standard()
    {
        // Default...
        var type = typeof(TXA.TA);
        var item = type.GetProperty("Name")!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("Name", name);
    }
}