namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
//[Enforced]
public static class Test_
{
    //[Enforced]
    [Fact]
    public static void Test()
    {
    }
}

[Cloneable] public class Type1<T> { }
[Cloneable] public class Type2 : Type1<string?> { }