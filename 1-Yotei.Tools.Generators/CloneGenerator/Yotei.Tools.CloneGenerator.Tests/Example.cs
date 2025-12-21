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

[Cloneable]
public partial interface IFake { }

[Cloneable<IFake>]
partial interface IFake { }