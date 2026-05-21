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

public class TBar { public int Name; }
public class TFoo<T> : TBar
{
    [Cloneable]
    public new readonly T? Name;
}