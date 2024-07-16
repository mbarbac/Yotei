namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static partial class Test_FrozenIFace_T
{
    [IFrozenList<int>]
    public partial interface IChain { }

    //[FrozenList<int>]
    //public partial class Chain
    //{
    //    public Chain() { }
    //    public Chain Clone() => throw null;
    //}

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test()
    {
    }
}