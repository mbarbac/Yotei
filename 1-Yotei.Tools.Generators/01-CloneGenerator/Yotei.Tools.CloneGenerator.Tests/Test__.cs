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

public partial class TFoo<T>
{
    [Cloneable]
    public readonly T? Name = default!; // readonly not identified!
}