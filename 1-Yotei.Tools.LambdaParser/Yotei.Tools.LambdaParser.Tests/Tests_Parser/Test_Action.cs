namespace Yotei.Tools.LambdaParser.Tests;

// ========================================================
////[Enforced]
public static class Test_Action
{
    //[Enforced]
    [Fact]
    public static void Test_ActionDelegate_NotSupported()
    {
        try
        {
            var action = () => { Debug.WriteLine("From no arguments action..."); };
            var node = LambdaParser.Parse(action).Result;
            Assert.Fail();
        }
        catch (NotSupportedException) { }
    }
}