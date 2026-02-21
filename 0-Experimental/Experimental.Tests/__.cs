namespace Experimental.Tests;

// ========================================================
//[Enforced]
[SuppressMessage("", "CA1822")]
[SuppressMessage("", "IDE0060")]
public static class Test_
{
    public abstract class AType1 { public abstract void MyMethod(int age); }
    public abstract class AType2 : AType1 { public override void MyMethod(int age) { } }

    //[Enforced]
    [Fact]
    public static void Test_AType()
    {
        var type = typeof(AType2);
        var method = type.GetMethod("MyMethod")!;
        Assert.True(method.IsNewAlike);
    }
}