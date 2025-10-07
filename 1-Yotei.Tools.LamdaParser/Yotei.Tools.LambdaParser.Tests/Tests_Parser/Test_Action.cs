using static Yotei.Tools.Diagnostics.DebugEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Action
{
    //[Enforced]
    [Fact]
    public static void Test_ActionDelegate_NotSupported()
    {

        try
        {
            var action = () => WriteLine(true, "From no arguments action...");
            var node = LambdaParser.Parse(action).Result;
            Assert.Fail();
        }
        catch (NotSupportedException) { }
    }
}