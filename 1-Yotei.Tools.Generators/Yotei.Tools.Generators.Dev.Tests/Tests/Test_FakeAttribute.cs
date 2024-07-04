namespace Yotei.Tools.Generators.Dev.Tests;

// ========================================================
//[Enforced]
public static partial class Test_FakeAttribute
{
    public class Target<T> { }

    [Fake]
    public partial class TypeA<T>
    {
        public Target<T> MethodT() => throw new NotImplementedException();
        public Target<int> MethodInt() => throw new NotImplementedException();
    }

    //[Enforced]
    //[Fact]
    //public static void Test() { }
}