namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================

public partial class Foo<K, T>
{
    [Named]
    public Foo(out K one) { one = default!; }
}

