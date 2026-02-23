namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_InterfaceHost
{
    // Easy case
    partial interface IFace1 { [With] string? Name { get; } }

    //[Enforced]
    [Fact]
    public static void Test_IFace1()
    {
        var type = typeof(IFace1);
        var method = type.GetMethod("WithName")!;
        var pars = method.GetParameters();

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace1), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }
}