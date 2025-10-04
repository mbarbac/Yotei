#pragma warning disable CA1822
#pragma warning disable IDE0060

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyNames_Constructors
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyNames_Constructors);

    // -----------------------------------------------------

    public class TXA { public class TA { public TA(byte one, string two) { } } }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Method()
    {
        // Default...
        var type = typeof(TXA.TA);
        var item = type.GetConstructor([typeof(byte), typeof(string)])!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("new(Byte, String)", name);
    }
}