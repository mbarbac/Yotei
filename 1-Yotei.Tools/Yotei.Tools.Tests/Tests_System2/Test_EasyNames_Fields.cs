#pragma warning disable CA1822
#pragma warning disable IDE0060

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyNames_Fields
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyNames_Fields);

    // -----------------------------------------------------

    public class TXA { public class TA { public int Name; } }

    //[Enforced]
    [Fact]
    public static void Test_Standard()
    {
        // Default...
        var type = typeof(TXA.TA);
        var item = type.GetField("Name")!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("Name", name);
    }
}