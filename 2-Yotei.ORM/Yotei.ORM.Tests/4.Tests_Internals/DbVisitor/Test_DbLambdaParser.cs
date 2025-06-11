using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_DbLambdaParser
{
    //[Enforced]
    [Fact]
    public static void Test_Expression_ToString()
    {
        var engine = new FakeEngine();
        var parser = new DbLambdaParser(engine);
        IDbToken token;
        DbTokenArgument arg;

        token = parser.Parse(x => "any");
    }
}