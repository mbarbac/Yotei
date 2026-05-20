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

//[Cloneable]
public partial class TFoo<T>
{
    //public void Method(int?* _) { }

    [Cloneable]
    internal enum TBar { One }
}

//[Cloneable]
//public partial class TFooAttribute { }

//[Cloneable]
//public partial class TBar<T> : TFoo<int?>
//{
//}