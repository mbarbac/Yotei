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

[Cloneable]
public partial class TFoo<T>
{
    public void Method(Predicate<StringBuilder?> _) { }
}

//[Cloneable]
//public partial class TBar<T> : TFoo<int?>
//{
//}