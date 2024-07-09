namespace Yotei.Tools.Generators.Dev.Tests;

// ========================================================
//[Enforced]
public static partial class Test_FakeAttribute
{
    public class Target<T> { }

    [Fake]
    public partial class TypeA<T> : Target<T?>
    {
        public T this[T one, int two] => throw new NotImplementedException();
    }
}