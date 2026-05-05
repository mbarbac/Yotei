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


public interface IFake
{
    [Cloneable]
    public string?[]? Name { get; }
}