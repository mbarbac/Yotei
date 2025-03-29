using static Yotei.Tools.Diagnostics.DebugEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Action
{
    //[Enforced]
    [Fact]
    public static void Test_Actions_NotSupported() => Repeater.Repeat(() =>
    {
        try
        {
            var action = () => WriteLine("From no arguments action...");
            var node = LambdaParser.Parse(action).Result;
            Assert.Fail();
        }
        catch (NotSupportedException) { }
    });
}