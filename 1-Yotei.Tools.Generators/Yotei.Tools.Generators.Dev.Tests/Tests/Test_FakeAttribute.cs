namespace Yotei.Tools.Generators.Dev.Tests;

// ========================================================
//[Enforced]
public static partial class Test_FakeAttribute
{
    public class Target<T> { }

    [Fake(typeof(int), Description = "Hello")]
    public partial class Foo { }
}