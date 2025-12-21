namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
////[Enforced]
//public static class Test_
//{
//    //[Enforced]
//    [Fact]
//    public static void Test()
//    {
//    }
//}

[Cloneable(ReturnType = typeof(string))]
public partial interface IFake2 { }

[Cloneable<IFake2>]
partial interface IFake2 { }