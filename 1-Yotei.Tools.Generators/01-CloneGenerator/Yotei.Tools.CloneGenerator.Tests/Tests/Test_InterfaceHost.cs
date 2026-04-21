namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_InterfaceHost
{
    // Easy case...
    [Cloneable] partial interface IFace00 { }

    //[Enforced]
    [Fact]
    public static void Test_IFace00()
    {
        var type = typeof(IFace00);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace00), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }
}