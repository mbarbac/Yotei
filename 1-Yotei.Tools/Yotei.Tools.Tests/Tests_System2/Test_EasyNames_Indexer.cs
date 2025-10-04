#pragma warning disable CA1822
#pragma warning disable IDE0060

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyNames_Indexer
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyNames_Indexer);

    // -----------------------------------------------------

    public class TXA { public class TA { public int this[byte one, string two] => default; } }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Method()
    {
        // Default...
        var type = typeof(TXA.TA);
        var item = type.GetProperty("Item")!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("this[Byte, String]", name);
    }
}