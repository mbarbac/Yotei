namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
//[Enforced]
//public static partial class Test_
//{
//    //[Enforced]
//    [Fact]
//    public static void Test()
//    {
//    }
//}

public static partial class TFoo
{
    [Cloneable]
    public static void Method(this int one) => throw null!; // 'this' NOT IDENTIFIED
}