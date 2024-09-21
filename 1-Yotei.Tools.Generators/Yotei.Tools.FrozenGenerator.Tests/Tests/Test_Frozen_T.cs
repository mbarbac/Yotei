namespace Yotei.Tools.FrozenGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Frozen_T
{
    public interface IElement<R> { }

    // ----------------------------------------------------

    [IFrozenList(typeof(IElement<>))]
    public partial interface IChain<R>
    {
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test()
    {
    }
}