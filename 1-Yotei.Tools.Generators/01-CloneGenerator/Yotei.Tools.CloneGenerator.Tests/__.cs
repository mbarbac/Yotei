namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_
{
    [Cloneable(UseVirtual = true)]
    public partial class MyType { }

    //[Enforced]
    [Fact]
    public static void Test()
    {
    }
}