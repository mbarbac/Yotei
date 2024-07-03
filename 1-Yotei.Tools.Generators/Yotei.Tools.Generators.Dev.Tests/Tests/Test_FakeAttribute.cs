namespace Yotei.Tools.Generators.Dev.Tests;

// ========================================================
//[Enforced]
public static class Test_FakeAttribute
{
    public class GenA<T> { }

    [Fake<GenA<int>>]
    public class TypeA
    {
    }

    //[Enforced]
    //[Fact]
    //public static void Test() { }
}