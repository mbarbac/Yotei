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

public class TFace1
{
    public TFace1 WithName() => this;
}

public class AType1 : TFace1
{
    public new TFace1 WithName() => this;
}