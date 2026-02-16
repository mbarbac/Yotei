namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
//public static class Test_
//{
//    //[Enforced]
//    [Fact]
//    public static void Test()
//    {
//    }
//}

public partial interface IFace1
{
    [With]
    string? Name { get; }
}

[InheritsWith]
public partial class Tipo1 : IFace1
{
    public Tipo1() { }
    protected Tipo1(Tipo1 _) { }

    [With]
    public string? Name { get; set; } = default!;
}

[InheritsWith]
public partial class Tipo2 : Tipo1
{
    public Tipo2() { }
    protected Tipo2(Tipo2 _) { }

    [With]
    public new string? Name { get; set; } = default!;
}