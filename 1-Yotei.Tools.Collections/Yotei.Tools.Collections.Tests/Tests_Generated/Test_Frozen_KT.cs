namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Frozen_KT
{
    [IFrozenList<int, string>]
    [Cloneable]
    public partial interface IChain { }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test()
    {
    }
}