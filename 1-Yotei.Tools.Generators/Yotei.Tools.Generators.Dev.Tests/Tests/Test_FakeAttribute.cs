namespace Yotei.Tools.Generators.Dev.Tests;

// ========================================================
//[Enforced]
public static partial class Test_FakeAttribute
{
    public class Target<T> { }

    [Fake]
    public partial class TypeA<T> : Target<T?>
    {
        public T M<S>(T one, S two) => throw new NotImplementedException();
        public T this[T one, int two] => throw new NotImplementedException();
        public int Age = 0;
    }
}