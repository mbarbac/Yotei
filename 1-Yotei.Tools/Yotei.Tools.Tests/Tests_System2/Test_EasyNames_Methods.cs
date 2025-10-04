#pragma warning disable CA1822
#pragma warning disable IDE0060

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyNames_Methods
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyNames_Methods);

    // -----------------------------------------------------

    public class TXA { public class TA { public int Name(byte one, string two) => default; } }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Method()
    {
        // Default...
        var type = typeof(TXA.TA);
        var item = type.GetMethod("Name")!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("Name(Byte, String)", name);
    }
}